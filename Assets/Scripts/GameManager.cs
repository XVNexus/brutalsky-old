using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
    public UIController uiController;
    public PlayerController[] players;

    [Header("Game Settings")]
    public float gameResetDelay;

    private bool gameOver = false;
    private float gameResetTimer;

    public void EndGame()
    {
        var winningPlayerNum = 0;
        var winningPlayerColor = new Color();
        for (var i = 0; i < players.Length && winningPlayerNum == 0; i++)
        {
            var player = players[i];
            if (player.health > 0)
            {
                winningPlayerNum = player.playerNum;
                winningPlayerColor = player.GetComponent<SpriteRenderer>().color;
            }
        }
        uiController.ShowWinText(winningPlayerNum, winningPlayerColor);
        Time.timeScale = 0.2f;
        gameOver = true;
    }

    public void ReloadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
        Time.fixedDeltaTime = 0.02f * value;
    }

    void Start()
    {
        gameResetTimer = gameResetDelay;
    }

    void Update()
    {
        if (gameOver)
        {
            SetTimeScale(gameResetTimer / gameResetDelay);
            gameResetTimer = Mathf.Max(gameResetTimer - Time.deltaTime / Time.timeScale, 0f);
            if (gameResetTimer == 0f)
            {
                ReloadGame();
            }
        }
    }
}
