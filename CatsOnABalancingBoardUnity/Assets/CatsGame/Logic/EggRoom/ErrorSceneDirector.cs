using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorSceneDirector {
    const string ErrorSceneName = "ErrorScene";

    static bool redirecting;

    // I love this but killing the game on logerror is a bad idea
    // e.g. dogcheck doesn't happen on error it's in specific circumstances like invalid scene or invalid save data I think
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() {
        //Application.logMessageReceived += HandleLog;
        //SceneManager.sceneLoaded += (a, b) => redirecting = false;
    }

    static void HandleLog(string condition, string stackTrace, LogType type) {
        if (redirecting || !(type == LogType.Error || type == LogType.Exception)) {
            return;
        }

        redirecting = true;
        SceneManager.LoadScene(ErrorSceneName);
    }
}
