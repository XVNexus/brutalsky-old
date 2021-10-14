using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
    public UIController uiController;
    public PlayerController[] players;

    [Header("Game Settings")]
    public float gameResetDelay;
    public float defaultFixedDeltaTime;
    public float fpsLabelUpdateInterval;
    public float fpsLabelSmoothing;

    public float TimeScaleTarget { get; private set; }
    public float TimeScaleTransitionSpeed { get; private set; }
    public TimeScaleTransitionFunctionType TimeScaleTransitionFunction { get; private set; }
    public bool Paused { get; private set; }
    public bool GameOver { get; private set; }

    private float gameResetTimer;
    private float fpsTarget;
    private float fpsSmoothed = 0f;

    void Start()
    {
        gameResetTimer = gameResetDelay;
        if (fpsLabelUpdateInterval > 0f)
        {
            InvokeRepeating("UpdateFps", fpsLabelUpdateInterval, fpsLabelUpdateInterval);
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    public void Pause()
    {
        SetTimeScale(0f, 5f);
        Paused = true;
    }

    public void Unpause()
    {
        SetTimeScale(1f, 5f);
        Paused = false;
    }

    public void EndGame()
    {
        if (!GameOver)
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
            SetTimeScale(.2f, 2f, TimeScaleTransitionFunctionType.DISTANCE);
            GameOver = true;
        }
    }

    public void ReloadGame()
    {
        SetTimeScale(1f);
        SceneManager.LoadScene("Main");
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
        Debug.Log(value);
        if (value > 0f)
        {
            Time.fixedDeltaTime = defaultFixedDeltaTime * value;
        }
        else
        {
            Time.fixedDeltaTime = defaultFixedDeltaTime;
        }
    }

    public void SetTimeScale(float value, float transitionSpeed, TimeScaleTransitionFunctionType transitionFunction = TimeScaleTransitionFunctionType.LINEAR)
    {
        TimeScaleTarget = value;
        TimeScaleTransitionSpeed = transitionSpeed;
        TimeScaleTransitionFunction = transitionFunction;
    }

    public void ChangePlayerHealth(int playerNum, int delta)
    {
        var playerFound = false;
        for (var i = 0; i < players.Length && !playerFound; i++)
        {
            var player = players[i];
            if (player.playerNum == playerNum)
            {
                player.ChangeHealth(delta);
                playerFound = true;
            }
        }
    }

    void Update()
    {
        UpdateTimeScale();
        if (fpsLabelUpdateInterval == 0f)
        {
            UpdateFps();
        }

        if (GameOver)
        {
            if (gameResetTimer > 0f)
            {
                gameResetTimer = Mathf.Max(gameResetTimer - Time.deltaTime / Time.timeScale, 0f);
            }
            else
            {
                ReloadGame();
            }
        }
    }

    private void UpdateTimeScale()
    {
        var timeScale = Time.timeScale;
        var deltaTime = timeScale > 0 ? Time.deltaTime / timeScale : 1f / Application.targetFrameRate;
        var timeScaleTargetDelta = TimeScaleTarget - timeScale;

        // Calculate new time scale
        switch (TimeScaleTransitionFunction)
        {
            case TimeScaleTransitionFunctionType.LINEAR:
                if (Mathf.Abs(timeScaleTargetDelta) < TimeScaleTransitionSpeed * deltaTime)
                {
                    timeScale = TimeScaleTarget;
                }
                else
                {
                    timeScale += Mathf.Sign(timeScaleTargetDelta) * TimeScaleTransitionSpeed * deltaTime;
                }
                break;
            case TimeScaleTransitionFunctionType.DISTANCE:
                if (Mathf.Abs(timeScaleTargetDelta) < .01f)
                {
                    timeScale = TimeScaleTarget;
                }
                else
                {
                    timeScale += timeScaleTargetDelta * TimeScaleTransitionSpeed * deltaTime;
                }
                break;
        }

        // If new time scale is different from old one, update Time.timeScale
        if (timeScale != Time.timeScale)
        {
            SetTimeScale(timeScale);
        }

        // Update fps label
        if (fpsLabelSmoothing > 0f)
        {
            fpsSmoothed += (fpsTarget - fpsSmoothed) * deltaTime * 1f / fpsLabelSmoothing;
        }
        else
        {
            fpsSmoothed = fpsTarget;
        }
        uiController.SetFpsLabel(Mathf.RoundToInt(fpsSmoothed));
    }

    private void UpdateFps()
    {
        var deltaTime = Time.timeScale > 0 ? Time.deltaTime / Time.timeScale : 1 / fpsSmoothed;
        fpsTarget = 1f / deltaTime;
    }
}

public enum TimeScaleTransitionFunctionType
{
    LINEAR,
    DISTANCE
}
