using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GoHome : MonoBehaviour {
    public const string gameSceneName = "TomPrototype2";

    void Update() {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
