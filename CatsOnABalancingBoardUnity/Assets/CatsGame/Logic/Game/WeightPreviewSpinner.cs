using OMC;
using Unity.Mathematics;
using UnityEngine;

public class WeightPreviewSpinner : MonoBehaviour
{
    public float spinSpeed = 60f;
    private GameObject currentPreview;
    private Transform previewPivot;
    public bool useUnscaledDeltaTime = false;

    void Awake()
    {
        previewPivot = new GameObject("Spinner Pivot").transform;
        previewPivot.SetParent(transform,false);
    }

    void Update() {
        if (previewPivot) {
            previewPivot.Rotate(Vector3.up, spinSpeed * (useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime), Space.Self);
        }
        if (currentPreview) {
            currentPreview.transform.localPosition = Vector3.zero;
            currentPreview.transform.localRotation = Quaternion.identity;
        }
    }

    public void TrySetPreview(GameObject prefab) {
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
