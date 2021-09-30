using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public VisualElement containerMenu;
    public Button buttonContinue;
    public Button buttonRestart;
    public Button buttonSettings;
    public Button buttonHelp;
    public Button buttonExit;

    public VisualElement containerWinText;
    public Label labelWinText;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        containerMenu = root.Q<VisualElement>("container-menu");
        buttonContinue = containerMenu.Q<Button>("button-continue");
        buttonRestart = containerMenu.Q<Button>("button-restart");
        buttonSettings = containerMenu.Q<Button>("button-settings");
        buttonHelp = containerMenu.Q<Button>("button-help");
        buttonExit = containerMenu.Q<Button>("button-exit");
        buttonContinue.clicked += ActContinue;
        buttonRestart.clicked += ActRestart;
        buttonSettings.clicked += ActSettings;
        buttonHelp.clicked += ActHelp;
        buttonExit.clicked += ActExit;
        containerMenu.visible = false;

        containerWinText = root.Q<VisualElement>("container-win-text");
        labelWinText = containerWinText.Q<Label>("label-win-text");
    }

    public void ShowWinText(int playerNum, Color playerColor)
    {
        labelWinText.text = $"Player {playerNum} wins!";
        labelWinText.style.color = playerColor;
    }

    void ActContinue()
    {
        containerMenu.visible = false;
    }

    void ActRestart()
    {
        SceneManager.LoadScene("Main");
    }

    void ActSettings()
    {

    }

    void ActHelp()
    {

    }

    void ActExit()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            containerMenu.visible = !containerMenu.visible;
        }
    }
}
