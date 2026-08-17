using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static OMC.WeightBehaviour;

namespace OMC {
    public class WeightTypeRegistry : MonoBehaviour {
        public static bool loaded { get; private set; }

        //[SerializeField]
        //private WeightDef[] _weightDefs;

        [NonSerialized]
        public static WeightDef[] weightDefs;

        private static Dictionary<WeightType, WeightDef> typeToDef = new();

        private void Awake() {
            if (loaded) {
                Destroy(this);
                return;
            }

            //weightDefs = _weightDefs;
            weightDefs = Resources.LoadAll<WeightDef>("");
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

            float roll = UnityEngine.Random.Range(0, total);
            float cum = 0f;

            foreach (var weight in selection) {
                cum += weight.probabilityBias;
                if (roll < cum) {
                    return weight;
                }
            }

            return selection[^1];
        }

        public static WeightDef[] GetRandomWeightDefs(int amount) // currently has nothing to do with rarity
        {
            List<WeightDef> temp = new(weightDefs);
            WeightDef[] defs = new WeightDef[amount];
            for (int i = 0; i < amount; i++)
            {
                int j = UnityEngine.Random.Range(0,temp.Count);
                defs[i] = temp[j];
                temp.RemoveAt(j);
            }
            return defs;
        }
    }
}
