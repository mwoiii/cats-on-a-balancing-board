using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public static MainMenuController instance;

    [SerializeField]
    private float loadGameDelay = 0.2f;

    [SerializeField]
    private GameObject pawPrefab;

    public string gameSceneName;

    private bool isLoadingGame;

    public void Awake() {
        RandomPawZone.ResetZones();
        instance = this;
    }

    public void StartGame() {
        if (!isLoadingGame) {
            isLoadingGame = true;
            StartCoroutine(BeginLoadNextScene());
        }
    }

    public void PlaceRandomPaw() {
        if (pawPrefab) {
            GameObject paw = Instantiate(pawPrefab, this.transform);
            paw.transform.position = RandomPawZone.GetRandomZonePosition();
            paw.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }
    }

    //public void OpenMenu(GameObject currentMenu) {
    //    currentMenu.SetActive(true);
    //    mainMenu.SetActive(false);
    //}

    //public void CloseMenu(GameObject currentMenu) {
    //    currentMenu.SetActive(false);
    //    mainMenu.SetActive(true);
    //}

    public void QuitGame() {
        Application.Quit();
    }

    public IEnumerator BeginLoadNextScene() {
        yield return new WaitForSeconds(loadGameDelay);
        if (gameSceneName != null) {
            SceneManager.LoadScene(gameSceneName);
        }
        isLoadingGame = false;
    }
}
