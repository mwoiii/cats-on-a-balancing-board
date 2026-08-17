using UnityEngine;

namespace OMC {
    public class NextUpBoxBehaviour : MonoBehaviour {
        public WeightPreviewSpinner spinner;

        private WeightDropper weightDropper;


        void Start() {
            weightDropper = WeightDropper.instance;
            if (!weightDropper) {
                Debug.LogError("No WeightDropper found for NextUpBoxBehaviour! This is not allowed!");
                Destroy(this);
                return;
            }

            weightDropper.OnNextPrefab += spinner.TrySetPreview;
            spinner.TrySetPreview(weightDropper.nextPrefab);
        }

        void OnDestroy() {
            if (weightDropper) {
                weightDropper.OnNextPrefab -= spinner.TrySetPreview;
            }
        }
    }
}
