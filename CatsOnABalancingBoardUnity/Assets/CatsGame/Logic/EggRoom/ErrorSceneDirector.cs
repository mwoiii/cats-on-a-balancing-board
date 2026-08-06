using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorSceneDirector
{
    const string ErrorSceneName = "ErrorScene";

    static bool redirecting;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() 
    {
        Application.logMessageReceived += HandleLog;
        SceneManager.sceneLoaded += (a, b) => redirecting = false;
    }

    static void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (redirecting || !(type == LogType.Error || type == LogType.Exception)){return;}

        redirecting = true;
        SceneManager.LoadScene(ErrorSceneName);
    }
}
