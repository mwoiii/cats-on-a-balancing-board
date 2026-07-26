using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogicScript : MonoBehaviour
{
    public float gameTime = 0.0f;
    public static bool gameRunning = true;

    private void Update()
    {
        if (gameRunning)
        {
            gameTime += Time.deltaTime;
        }
    }

    void Start()
    {
        gameRunning = true;
    }

    public void gameOver()
    {
        gameRunning = false;
        HUDController.instance.InitiateGameOver();
        Debug.Log("Game Over");
        Debug.Log(gameTime);
    }
}
