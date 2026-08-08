using UnityEngine;

namespace OMC {
    public class NextUpBoxBehaviour : MonoBehaviour {
        public Transform previewPivot;

        public float spinSpeed = 60f;

        public int previewLayer = 10;

        private WeightDropper weightDropper;

        private GameObject currentPreview;

        void Start() {
            weightDropper = WeightDropper.instance;
            if (!weightDropper) {
                Debug.LogError("No WeightDropper found for NextUpBoxBehaviour! This is not allowed!");
                Destroy(this);
                return;
            }

            weightDropper.OnNextPrefab += TrySetPreview;
            TrySetPreview(weightDropper.nextPrefab);
        }

        void Update() {
            if (previewPivot) {
                previewPivot.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
            }
            if (currentPreview) {
                currentPreview.transform.localPosition = Vector3.zero;
                currentPreview.transform.localRotation = Quaternion.identity;
            }
        }

        void OnDestroy() {
            if (weightDropper) {
                weightDropper.OnNextPrefab -= TrySetPreview;
            }
        }

        void TrySetPreview(GameObject prefab) {
            if (currentPreview) {
                Destroy(currentPreview);
            }

            if (!prefab) {
                return;
            }

            bool hasDisplay = false;
            WeightBehaviour weightBehaviour;
            if (prefab.TryGetComponent(out weightBehaviour) && weightBehaviour.displayPrefab) {
                hasDisplay = true;
            }

            currentPreview = Instantiate(prefab, previewPivot.position, Quaternion.identity, previewPivot);
            currentPreview.transform.localPosition = Vector3.zero;
            currentPreview.transform.localRotation = Quaternion.identity;

            if (!hasDisplay) {
                currentPreview.GetComponent<WeightBehaviour>().type = WeightBehaviour.WeightType.None;
                foreach (var sub in currentPreview.GetComponentsInChildren<WeightSubBehaviourBase>())
                {
                    Destroy(sub);
                }
                foreach (var rigidbody in currentPreview.GetComponentsInChildren<Rigidbody>()) {
                    Destroy(rigidbody);
                }
                foreach (var collider in currentPreview.GetComponentsInChildren<Collider>()) {
                    Destroy(collider);
                }
            }
        }
    }
}
