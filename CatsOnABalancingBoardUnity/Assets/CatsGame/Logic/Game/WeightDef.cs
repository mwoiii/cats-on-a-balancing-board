using System.Collections.Generic;
using UnityEngine;
using static OMC.WeightBehaviour;


namespace OMC {
    [CreateAssetMenu(menuName = "OMC/WeightDef")]
    public class WeightDef : ScriptableObject {
        public WeightType weightType;

        public float force = 20f;

        public float probabilityBias = 1;

        public float rarity = 0f;

        public float multAdd = 0f;

        public float baseAdd = 0f;

        public GameObject blockPrefab;

        public GameObject spherePrefab;

        public Sprite sprite;

        [HideInInspector]
        public GameObject[] shapePrefabs;

        public void Init() {
            List<GameObject> shapeList = new List<GameObject>();
            if (blockPrefab) {
                shapeList.Add(blockPrefab);
            }
            if (spherePrefab) {
                shapeList.Add(spherePrefab);
            }
            shapePrefabs = shapeList.ToArray();

            foreach (GameObject prefab in shapePrefabs) {
                WeightBehaviour behaviour = prefab.GetComponent<WeightBehaviour>();
                if (!behaviour) {
                    behaviour = prefab.AddComponent<WeightBehaviour>();
                }
                behaviour.type = weightType;
            }
        }

        public GameObject GetRandomShapePrefab() {
            if (shapePrefabs.Length == 0) {
                Debug.LogError($"{name} has no shape prefabs! Returning null!");
                return null;
            }
            return shapePrefabs[Random.Range(0, shapePrefabs.Length)];
        }
    }
}
