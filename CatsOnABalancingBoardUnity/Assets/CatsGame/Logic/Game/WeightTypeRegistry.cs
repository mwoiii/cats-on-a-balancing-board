using System.Collections.Generic;
using UnityEngine;
using static OMC.WeightBehaviour;

namespace OMC {
    public class WeightTypeRegistry : MonoBehaviour {
        public static bool loaded { get; private set; }

        [SerializeField]
        private WeightDef[] _weightDefs;

        public static WeightDef[] weightDefs;

        private static Dictionary<WeightType, WeightDef> typeToDef = new();

        private void Awake() {
            if (loaded) {
                Destroy(this);
                return;
            }

            weightDefs = _weightDefs;
            Init();
        }

        private void Init() {
            foreach (WeightDef weightDef in weightDefs) {
                if (!weightDef) {
                    Debug.LogError("Detected null WeightDef in registry! Skipping...");
                    continue;
                }
                weightDef.Init();
                typeToDef[weightDef.weightType] = weightDef;
            }

            loaded = true;
        }

        public static WeightDef GetWeightDef(WeightType type) {
            if (!typeToDef.ContainsKey(type)) {
                return null;
            }

            return typeToDef[type];
        }

        public static WeightDef GetRandomWeight(List<WeightDef> selection) {
            if (selection.Count == 0) {
                return null;
            }

            float total = 0f;
            foreach (var weight in selection) {
                total += weight.probabilityBias;
            }

            float roll = Random.Range(0, total);
            float cum = 0f;

            foreach (var weight in selection) {
                cum += weight.probabilityBias;
                if (roll < cum) {
                    return weight;
                }
            }

            return selection[^1];
        }
    }
}
