using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OMC.UI {
    public class MenuController : MonoBehaviour {

        public static MenuController instance;

        [SerializeField]
        private float selectOptionDelay = 0.2f;

        [SerializeField]
        private GameObject pawPrefab;

        public const string gameSceneName = "TomPrototype2";

        public const string menuSceneName = "Menu";

        private bool selectedOption;

        public void Awake() {
            RandomPawZone.ResetZones();
            instance = this;
        }

        public void OpenGame() {
            StartCoroutine(BeginSelectOption(() => {
                if (gameSceneName != null) {
                    if (GameLogicScript.instance && GameLogicScript.score > 1000 && Random.Range(0f, 1f) > 0.995f) {
                        SceneManager.LoadScene("EggPrototype");
                    } else {
                        SceneManager.LoadScene(gameSceneName);
                    }
                }
            }));
        }

        public void OpenMenu() {
            StartCoroutine(BeginSelectOption(() => {
                if (gameSceneName != null) {
                    SceneManager.LoadScene(menuSceneName);
                }
            }));
        }

        public void QuitGame() {
            StartCoroutine(BeginSelectOption(() => {
                Application.Quit();
            }));
        }

        public IEnumerator BeginSelectOption(System.Action action) {
            if (!selectedOption) {
                selectedOption = true;
                yield return new WaitForSeconds(selectOptionDelay);
                action();
                selectedOption = false;
            }
        }

        public void PlaceRandomPaw() {
            if (pawPrefab) {
                GameObject paw = Instantiate(pawPrefab, this.transform);
                paw.transform.position = RandomPawZone.GetRandomZonePosition();
                paw.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            }
        }
    }
}
