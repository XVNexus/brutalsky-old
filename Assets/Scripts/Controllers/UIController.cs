using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIWindow
{
    public string id { get; private set; }
    public VisualElement container { get; private set; }
    //TODO: Add more usable parent and children editor
    public UIWindow parent { get; set; }
    public UIWindow[] children { get; set; }
    public int layer { get; private set; }
    public bool Active { get => container.visible; private set => container.visible = value; }

    public UIWindow(string id, int layer, VisualElement container, UIWindow parent = null, UIWindow[] children = null, bool active = false)
    {
        this.id = id;
        this.layer = layer;
        this.container = container;
        this.parent = parent;
        this.children = children != null ? children : new UIWindow[] { };
        Active = active;
    }

    public void SetActive(bool value)
    {
        if (value)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    public void Open()
    {
        Active = true;
        container.visible = true;
    }

    public void Close()
    {
        Active = false;
        CloseAllChildren();
    }

    public void Toggle()
    {
        SetActive(!Active);
    }

    public void CloseAllChildren()
    {
        foreach (var window in children)
        {
            window.Close();
        }
    }
}

public class UIManager
{
    private List<UIWindow> windows;

    public UIManager(UIWindow[] windows)
    {
        this.windows = new List<UIWindow>();
        foreach (var window in windows)
        {
            this.windows.Add(window);
        }
    }

    public void OpenWindow(UIWindow window)
    {
        foreach (var activeWindow in GetActiveWindows())
        {
            if (activeWindow.layer == window.layer)
            {
                activeWindow.Close();
            }
        }
        window.Open();
    }

    public void CloseWindow(UIWindow window)
    {
        window.Close();
    }

    public void ToggleWindow(UIWindow window)
    {
        window.Toggle();
    }

    public void CloseAllWindows()
    {
        foreach (var window in windows)
        {
            if (window.Active)
            {
                CloseWindow(window);
            }
        }
    }

    public UIWindow GetWindow(int index)
    {
        return windows[index];
    }

    public UIWindow GetWindow(string id)
    {
        foreach (var window in windows)
        {
            if (window.id == id)
            {
                return window;
            }
        }
        return null;
    }

    public UIWindow GetWindow(VisualElement container)
    {
        foreach (var window in windows)
        {
            if (window.container == container)
            {
                return window;
            }
        }
        return null;
    }

    public UIWindow GetTopWindow()
    {
        UIWindow result = null;
        var activeWindows = GetActiveWindows();
        if (activeWindows.Length > 0)
        {
            result = activeWindows[0];
            for (var i = 1; i < activeWindows.Length; i++)
            {
                var activeWindow = activeWindows[i];
                if (activeWindow.layer > result.layer)
                {
                    result = activeWindow;
                }
            }
        }
        return result;
    }

    public UIWindow[] GetActiveWindows()
    {
        var result = new List<UIWindow>();
        foreach (var window in windows)
        {
            if (window.Active)
            {
                result.Add(window);
            }
        }
        return result.ToArray();
    }

    public UIWindow[] GetAllWindows()
    {
        return windows.ToArray();
    }

    public bool IsAnyWindowOpen()
    {
        foreach (var window in windows)
        {
            if (window.Active)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAllWindowsClosed()
    {
        return !IsAnyWindowOpen();
    }

    public bool IsActiveWindowValid(UIWindow window)
    {
        var result = true;

        // Part 1: Check if window's parent is active
        if (window.parent != null)
        {
            result = window.parent.Active;
        }

        // Part 2: Check if there are no other active windows in the same layer (e.g. main menu and hacks menu open at the same time)
        var activeWindows = GetActiveWindows();
        for (var i = 0; i < activeWindows.Length && result; i++)
        {
            var activeWindow = activeWindows[i];
            if (activeWindow.Active && activeWindow.layer == window.layer)
            {
                result = false;
            }
        }

        return result;
    }
}

public class UIController : MonoBehaviour
{
    [Header("Object References")]
    public GameManager gameManager;

    // Win text group
    public VisualElement containerWinText;
    public Label labelWinText;

    // Menu group
    public VisualElement containerMenu;
    public Button buttonContinue;
    public Button buttonRestart;
    public Button buttonSettings;
    public Button buttonHelp;
    public Button buttonExit;

    // Settings group
    public VisualElement containerSettings;
    public Foldout foldoutControls;
    public TextField textFieldP1MoveUp;
    public TextField textFieldP1MoveDown;
    public TextField textFieldP1MoveLeft;
    public TextField textFieldP1MoveRight;
    public TextField textFieldP1Ability;
    public TextField textFieldP2MoveUp;
    public TextField textFieldP2MoveDown;
    public TextField textFieldP2MoveLeft;
    public TextField textFieldP2MoveRight;
    public TextField textFieldP2Ability;
    public Foldout foldoutGraphics;
    public Toggle toggleCameraShake;
    public Toggle toggleParticlesPlayer;
    public Toggle toggleParticlesAmbient;

    // Hacks menu group
    public VisualElement containerHacks;
    public Button buttonHealPlayer1;
    public Button buttonHealPlayer2;
    public Button buttonKillPlayer1;
    public Button buttonKillPlayer2;
    public Button buttonClose;

    private List<VisualElement> activeContainers = new List<VisualElement>();
    private UIManager uiManager;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Win text group
        containerWinText = root.Q<VisualElement>("container-win-text");
        labelWinText = containerWinText.Q<Label>("label-win-text");

        // Menu group
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

        // Settings group
        containerSettings = root.Q<VisualElement>("container-settings");
        foldoutControls = root.Q<Foldout>("foldout-controls");
        textFieldP1MoveUp = root.Q<TextField>("text-field-p1-move-up");
        textFieldP1MoveDown = root.Q<TextField>("text-field-p1-move-down");
        textFieldP1MoveLeft = root.Q<TextField>("text-field-p1-move-left");
        textFieldP1MoveRight = root.Q<TextField>("text-field-p1-move-right");
        textFieldP1Ability = root.Q<TextField>("text-field-p1-ability");
        textFieldP2MoveUp = root.Q<TextField>("text-field-p2-move-up");
        textFieldP2MoveDown = root.Q<TextField>("text-field-p2-move-down");
        textFieldP2MoveLeft = root.Q<TextField>("text-field-p2-move-left");
        textFieldP2MoveRight = root.Q<TextField>("text-field-p2-move-right");
        textFieldP2Ability = root.Q<TextField>("text-field-p2-ability");
        foldoutGraphics = root.Q<Foldout>("foldout-graphics");
        toggleCameraShake = root.Q<Toggle>("toggle-camera-shake");
        toggleParticlesPlayer = root.Q<Toggle>("toggle-particles-player");
        toggleParticlesAmbient = root.Q<Toggle>("toggle-particles-ambient");

        // Hacks menu group
        containerHacks = root.Q<VisualElement>("container-hacks");
        buttonHealPlayer1 = containerHacks.Q<Button>("button-heal-player-1");
        buttonHealPlayer2 = containerHacks.Q<Button>("button-heal-player-2");
        buttonKillPlayer1 = containerHacks.Q<Button>("button-kill-player-1");
        buttonKillPlayer2 = containerHacks.Q<Button>("button-kill-player-2");
        buttonClose = containerHacks.Q<Button>("button-close");

        buttonHealPlayer1.clicked += ActHealPlayer1;
        buttonHealPlayer2.clicked += ActHealPlayer2;
        buttonKillPlayer1.clicked += ActKillPlayer1;
        buttonKillPlayer2.clicked += ActKillPlayer2;
        buttonClose.clicked += ActClose;

        // Hide menus by default
        containerWinText.visible = false;
        containerMenu.visible = false;
        containerHacks.visible = false;
        containerSettings.visible = false;
        foldoutControls.value = false;
        foldoutGraphics.value = false;

        // Set up UI manager
        var uiWindowMenu = new UIWindow("menu", 1, containerMenu);
        var uiWindowSettings = new UIWindow("menu.settings", 2, containerSettings);
        var uiWindowHacks = new UIWindow("hacks", 1, containerHacks);
        uiWindowMenu.children = new UIWindow[] { uiWindowSettings };
        uiWindowSettings.parent = uiWindowMenu;

        uiManager = new UIManager(new UIWindow[] { uiWindowMenu, uiWindowSettings, uiWindowHacks });
    }

    public void ShowWinText(int playerNum, Color playerColor)
    {
        containerWinText.visible = true;
        labelWinText.text = $"Player {playerNum} wins!";
        labelWinText.style.color = playerColor;
    }

    void ActContinue()
    {
        uiManager.CloseWindow(uiManager.GetWindow("menu"));
    }

    void ActRestart()
    {
        SceneManager.LoadScene("Main");
    }

    void ActSettings()
    {
        uiManager.OpenWindow(uiManager.GetWindow("menu.settings"));
    }

    void ActHelp()
    {

    }

    void ActExit()
    {
        Application.Quit();
    }

    void ActHealPlayer1()
    {
        gameManager.ChangePlayerHealth(1, 100);
    }

    void ActHealPlayer2()
    {
        gameManager.ChangePlayerHealth(2, 100);
    }

    void ActKillPlayer1()
    {
        gameManager.ChangePlayerHealth(1, -100);
    }

    void ActKillPlayer2()
    {
        gameManager.ChangePlayerHealth(2, -100);
    }

    void ActClose()
    {
        uiManager.CloseWindow(uiManager.GetWindow("hacks"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiManager.GetActiveWindows().Length > 0)
            {
                uiManager.CloseWindow(uiManager.GetTopWindow());
            }
            else
            {
                uiManager.OpenWindow(uiManager.GetWindow("menu"));
            }
        }
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            if (!uiManager.GetWindow("hacks").Active)
            {
                uiManager.OpenWindow(uiManager.GetWindow("hacks"));
            }
            else
            {
                uiManager.CloseWindow(uiManager.GetWindow("hacks"));
            }
        }
    }
}
