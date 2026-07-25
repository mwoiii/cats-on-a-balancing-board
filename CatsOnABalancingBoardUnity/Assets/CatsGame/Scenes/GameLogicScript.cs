using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogicScript : MonoBehaviour
{
    public float gameTime = 0.0f;

    private void Update()
    {
        gameTime += Time.deltaTime;
    }

    public void gameOver()
    {
        Debug.Log("Game Over");
        Debug.Log(gameTime);
    }
    
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
