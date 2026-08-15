using System.Collections;
using TMPro;
using UnityEngine;

namespace OMC.UI {
    public class DialogueController : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI textInput;

        public float defaultDelay = 0.05f;

        private void OnEnable() {
            if (!textInput) {
                textInput = GetComponent<TextMeshProUGUI>();
            }

            if (textInput) {
                textInput.maxVisibleCharacters = 0;
                StartCoroutine(WriteText());
            }
        }

        private IEnumerator WriteText() {
            for (int i = 0; i < textInput.text.Length; i++) {
                if (textInput) {
                    textInput.maxVisibleCharacters = i + 1;
                    yield return new WaitForSeconds(defaultDelay);
                }
            }
        }
    }
}
