using UnityEngine;

namespace OMC {
    public class NextUpBoxBehaviour : MonoBehaviour {
        public Transform previewPivot;

        public float spinSpeed = 60f;

        public int previewLayer = 10;

        WeightDropper weightDropper;

        GameObject currentPreview;

        void Start() {
            weightDropper = FindAnyObjectByType<WeightDropper>();
            if (weightDropper == null) { return; }

            weightDropper.OnNextPrefab += SetPreview;
            SetPreview(weightDropper.nextPrefab);
        }

        void Update() {
            if (previewPivot != null) { previewPivot.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self); }
            if (currentPreview != null) {
                currentPreview.transform.localPosition = Vector3.zero;
                currentPreview.transform.localRotation = Quaternion.identity;
            }
        }

        void OnDestroy() {
            if (weightDropper != null) { weightDropper.OnNextPrefab -= SetPreview; }
        }

        void SetPreview(GameObject prefab) {
            if (currentPreview != null) { Destroy(currentPreview); }
            if (prefab == null) { return; }

            currentPreview = Instantiate(prefab, previewPivot.position, Quaternion.identity, previewPivot);
            currentPreview.transform.localPosition = Vector3.zero;
            currentPreview.transform.localRotation = Quaternion.identity;
            currentPreview.GetComponent<WeightBehaviour>().type = WeightBehaviour.WeightType.None;

            foreach (var a in currentPreview.GetComponentsInChildren<Rigidbody>()) { Destroy(a); }
            foreach (var a in currentPreview.GetComponentsInChildren<Collider>()) { Destroy(a); }

            SetLayerRecursively(currentPreview, previewLayer);
        }

        void SetLayerRecursively(GameObject obj, int layer) {
            obj.layer = layer;
            foreach (Transform child in obj.transform) {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
