using OMC;
using OMC.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInput : MonoBehaviour
{
    public GameObject gameOverScreen;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (gameOverScreen.activeInHierarchy)
            {
                GameLogicScript.instance.Unpause();
            }
            else if (!gameOverScreen.activeInHierarchy)
            {
                GameLogicScript.instance.Pause();
            }
        }
    }
}
