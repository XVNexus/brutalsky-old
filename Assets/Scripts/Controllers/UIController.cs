using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIWindow
{
    public string Id { get; private set; }
    public int Layer { get; private set; }
    public VisualElement Container { get; private set; }
    private UIWindow parent;
    public UIWindow Parent
    {
        get => parent;
        private set
        {
            parent = value;
            if (!value.children.Contains(this))
            {
                value.children.Add(this);
            }
        }
    }
    private List<UIWindow> children;
    public List<UIWindow> Children
    {
        get => children;
        private set
        {
            children = value;
            foreach (var child in value)
            {
                child.parent = this;
            }
        }
    }
    public bool Active { get => Container.visible; private set => Container.visible = value; }

    public UIWindow(string id, int layer, VisualElement container, UIWindow parent = null, List<UIWindow> children = null, bool active = false)
    {
        Id = id;
        Layer = layer;
        Container = container;
        if (parent != null)
        {
            Parent = parent;
        }
        Children = children != null ? children : new List<UIWindow>();
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
        Container.visible = true;
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
            if (activeWindow.Layer == window.Layer)
            {
                activeWindow.Close();
            }
        }
        window.Open();
    }

    public void OpenWindow(string id)
    {
        OpenWindow(GetWindow(id));
    }

    public void CloseWindow(UIWindow window)
    {
        window.Close();
    }

    public void CloseWindow(string id)
    {
        CloseWindow(GetWindow(id));
    }

    public void ToggleWindow(UIWindow window)
    {
        window.Toggle();
    }

    public void ToggleWindow(string id)
    {
        ToggleWindow(GetWindow(id));
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
        if (!id.StartsWith("#"))
        {
            foreach (var window in windows)
            {
                if (window.Id == id)
                {
                    return window;
                }
            }
            return null;
        }
        else
        {
            switch (id.Substring(1))
            {
                case "top":
                    return GetTopWindow();
                case "bottom":
                    return GetBottomWindow();
                default:
                    return null;
            }
        }
    }

    public UIWindow GetWindow(VisualElement container)
    {
        foreach (var window in windows)
        {
            if (window.Container == container)
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
                if (activeWindow.Layer > result.Layer)
                {
                    result = activeWindow;
                }
            }
        }
        return result;
    }

    public UIWindow GetBottomWindow()
    {
        UIWindow result = null;
        var activeWindows = GetActiveWindows();
        if (activeWindows.Length > 0)
        {
            result = activeWindows[0];
            for (var i = 1; i < activeWindows.Length; i++)
            {
                var activeWindow = activeWindows[i];
                if (activeWindow.Layer < result.Layer)
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
        if (window.Parent != null)
        {
            result = window.Parent.Active;
        }

        // Part 2: Check if there are no other active windows in the same layer (e.g. main menu and hacks menu open at the same time)
        var activeWindows = GetActiveWindows();
        for (var i = 0; i < activeWindows.Length && result; i++)
        {
            var activeWindow = activeWindows[i];
            if (activeWindow.Active && activeWindow.Layer == window.Layer)
            {
                result = false;
            }
        }

        return result;
    }

    public bool IsActiveWindowValid(string id)
    {
        return IsActiveWindowValid(GetWindow(id));
    }
}

public class UIController : MonoBehaviour
{
    [Header("Object References")]
    public GameManager gameManager;
    public InputManager inputManager;

    // Message group
    public VisualElement containerMessage;
    public Label messageLabel;

    // Menu group
    public VisualElement containerMenu;
    public Button menuButtonContinue;
    public Button menuButtonLevelSelector;
    public Button menuButtonPrev;
    public Button menuButtonRestart;
    public Button menuButtonNext;
    public Button menuButtonSettings;
    public Button menuButtonHelp;
    public Button menuButtonMainMenu;
    public Button menuButtonExit;

    // Settings group
    public VisualElement containerMenuSettings;
    public Button menuSettingsButtonSaveChanges;
    public Button menuSettingsButtonDiscardChanges;
    public Foldout menuSettingsFoldoutControls;
    public TextField menuSettingsControlsPlayer1MoveUp;
    public TextField menuSettingsControlsPlayer1MoveDown;
    public TextField menuSettingsControlsPlayer1MoveLeft;
    public TextField menuSettingsControlsPlayer1MoveRight;
    public TextField menuSettingsControlsPlayer1Ability;
    public TextField menuSettingsControlsPlayer2MoveUp;
    public TextField menuSettingsControlsPlayer2MoveDown;
    public TextField menuSettingsControlsPlayer2MoveLeft;
    public TextField menuSettingsControlsPlayer2MoveRight;
    public TextField menuSettingsControlsPlayer2Ability;
    public Foldout menuSettingsFoldoutGraphics;
    public Toggle menuSettingsGraphicsParticlesEffects;
    public Toggle menuSettingsGraphicsParticlesAmbient;

    // Help group
    public VisualElement containerMenuHelp;

    private UIManager uiManager;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Message group
        containerMessage = root.Q<VisualElement>("container-message");
        messageLabel = containerMessage.Q<Label>("message-label");

        // Menu group
        containerMenu = root.Q<VisualElement>("container-menu");
        menuButtonContinue = containerMenu.Q<Button>("menu-button-continue");
        menuButtonLevelSelector = containerMenu.Q<Button>("menu-button-level-selector");
        menuButtonPrev = containerMenu.Q<Button>("menu-button-prev");
        menuButtonRestart = containerMenu.Q<Button>("menu-button-restart");
        menuButtonNext = containerMenu.Q<Button>("menu-button-next");
        menuButtonSettings = containerMenu.Q<Button>("menu-button-settings");
        menuButtonHelp = containerMenu.Q<Button>("menu-button-help");
        menuButtonMainMenu = containerMenu.Q<Button>("menu-button-main-menu");
        menuButtonExit = containerMenu.Q<Button>("menu-button-exit");

        menuButtonContinue.clicked += ActMenuContinue;
        menuButtonLevelSelector.clicked += ActMenuLevelSelector;
        menuButtonPrev.clicked += ActMenuPrev;
        menuButtonRestart.clicked += ActMenuRestart;
        menuButtonNext.clicked += ActMenuNext;
        menuButtonSettings.clicked += ActMenuSettings;
        menuButtonHelp.clicked += ActMenuHelp;
        menuButtonMainMenu.clicked += ActMenuMainMenu;
        menuButtonExit.clicked += ActMenuExit;

        // Settings group
        containerMenuSettings = root.Q<VisualElement>("container-menu-settings");
        menuSettingsButtonSaveChanges = containerMenuSettings.Q<Button>("menu-settings-button-save-changes");
        menuSettingsButtonDiscardChanges = containerMenuSettings.Q<Button>("menu-settings-button-discard-changes");
        menuSettingsFoldoutControls = containerMenuSettings.Q<Foldout>("menu-settings-foldout-controls");
        menuSettingsControlsPlayer1MoveUp = containerMenuSettings.Q<TextField>("menu-settings-controls-player-1-move-up");
        menuSettingsControlsPlayer1MoveDown = containerMenuSettings.Q<TextField>("menu-settings-controls-player-1-move-down");
        menuSettingsControlsPlayer1MoveLeft = containerMenuSettings.Q<TextField>("menu-settings-controls-player-1-move-left");
        menuSettingsControlsPlayer1MoveRight = containerMenuSettings.Q<TextField>("menu-settings-controls-player-1-move-right");
        menuSettingsControlsPlayer1Ability = containerMenuSettings.Q<TextField>("menu-settings-controls-player-1-ability");
        menuSettingsControlsPlayer2MoveUp = containerMenuSettings.Q<TextField>("menu-settings-controls-player-2-move-up");
        menuSettingsControlsPlayer2MoveDown = containerMenuSettings.Q<TextField>("menu-settings-controls-player-2-move-down");
        menuSettingsControlsPlayer2MoveLeft = containerMenuSettings.Q<TextField>("menu-settings-controls-player-2-move-left");
        menuSettingsControlsPlayer2MoveRight = containerMenuSettings.Q<TextField>("menu-settings-controls-player-2-move-right");
        menuSettingsControlsPlayer2Ability = containerMenuSettings.Q<TextField>("menu-settings-controls-player-2-ability");
        menuSettingsFoldoutGraphics = containerMenuSettings.Q<Foldout>("menu-settings-foldout-graphics");
        menuSettingsGraphicsParticlesEffects = containerMenuSettings.Q<Toggle>("menu-settings-graphics-particles-effects");
        menuSettingsGraphicsParticlesAmbient = containerMenuSettings.Q<Toggle>("menu-settings-graphics-particles-ambient");

        menuSettingsButtonSaveChanges.clicked += ActMenuSettingsSaveChanges;
        menuSettingsButtonDiscardChanges.clicked += ActMenuSettingsDiscardChanges;

        // Help group
        containerMenuHelp = root.Q<VisualElement>("container-menu-help");

        // Set up UI system
        containerMessage.visible = false;
        var uiWindowMenu = new UIWindow("menu", 0, containerMenu);
        var uiWindowMenuSettings = new UIWindow("menu.settings", 1, containerMenuSettings, uiWindowMenu);
        var uiWindowMenuHelp = new UIWindow("menu.help", 1, containerMenuHelp, uiWindowMenu);
        uiManager = new UIManager(new UIWindow[] { uiWindowMenu, uiWindowMenuSettings, uiWindowMenuHelp });

        LoadSettings();
    }

    private void ActMenuContinue()
    {
        uiManager.CloseWindow("menu");
    }

    private void ActMenuLevelSelector()
    {
        Debug.LogWarning("menu:@level-selector");
    }

    private void ActMenuPrev()
    {
        Debug.LogWarning("menu:@previous");
    }

    private void ActMenuRestart()
    {
        gameManager.ReloadGame();
    }

    private void ActMenuNext()
    {
        Debug.LogWarning("menu:@next");
    }

    private void ActMenuSettings()
    {
        uiManager.OpenWindow("menu.settings");
    }

    private void ActMenuHelp()
    {
        uiManager.OpenWindow("menu.help");
    }

    private void ActMenuMainMenu()
    {
        Debug.LogWarning("menu:@main-menu");
    }

    private void ActMenuExit()
    {
        Application.Quit();
    }

    private void ActMenuSettingsSaveChanges()
    {
        SaveSettings();
        inputManager.UpdateKeybinds();
    }

    private void ActMenuSettingsDiscardChanges()
    {
        LoadSettings();
    }

    public void ShowWinText(int playerNum, Color playerColor)
    {
        containerMessage.visible = true;
        messageLabel.text = $"Player {playerNum} wins!";
        messageLabel.style.color = playerColor;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetString("controls.player_1.move.up", menuSettingsControlsPlayer1MoveUp.value);
        PlayerPrefs.SetString("controls.player_1.move.down", menuSettingsControlsPlayer1MoveDown.value);
        PlayerPrefs.SetString("controls.player_1.move.left", menuSettingsControlsPlayer1MoveLeft.value);
        PlayerPrefs.SetString("controls.player_1.move.right", menuSettingsControlsPlayer1MoveRight.value);
        PlayerPrefs.SetString("controls.player_1.ability", menuSettingsControlsPlayer1Ability.value);
        PlayerPrefs.SetString("controls.player_2.move.up", menuSettingsControlsPlayer2MoveUp.value);
        PlayerPrefs.SetString("controls.player_2.move.down", menuSettingsControlsPlayer2MoveDown.value);
        PlayerPrefs.SetString("controls.player_2.move.left", menuSettingsControlsPlayer2MoveLeft.value);
        PlayerPrefs.SetString("controls.player_2.move.right", menuSettingsControlsPlayer2MoveRight.value);
        PlayerPrefs.SetString("controls.player_2.ability", menuSettingsControlsPlayer2Ability.value);
        PlayerPrefs.SetInt("graphics.particles.effects", menuSettingsGraphicsParticlesEffects.value ? 1 : 0);
        PlayerPrefs.SetInt("graphics.particles.ambient", menuSettingsGraphicsParticlesEffects.value ? 1 : 0);
    }

    public void LoadSettings()
    {
        menuSettingsControlsPlayer1MoveUp.value = PlayerPrefs.GetString("controls.player_1.move.up");
        menuSettingsControlsPlayer1MoveDown.value = PlayerPrefs.GetString("controls.player_1.move.down");
        menuSettingsControlsPlayer1MoveLeft.value = PlayerPrefs.GetString("controls.player_1.move.left");
        menuSettingsControlsPlayer1MoveRight.value = PlayerPrefs.GetString("controls.player_1.move.right");
        menuSettingsControlsPlayer1Ability.value = PlayerPrefs.GetString("controls.player_1.ability");
        menuSettingsControlsPlayer2MoveUp.value = PlayerPrefs.GetString("controls.player_2.move.up");
        menuSettingsControlsPlayer2MoveDown.value = PlayerPrefs.GetString("controls.player_2.move.down");
        menuSettingsControlsPlayer2MoveLeft.value = PlayerPrefs.GetString("controls.player_2.move.left");
        menuSettingsControlsPlayer2MoveRight.value = PlayerPrefs.GetString("controls.player_2.move.right");
        menuSettingsControlsPlayer2Ability.value = PlayerPrefs.GetString("controls.player_2.ability");
        menuSettingsGraphicsParticlesEffects.value = PlayerPrefs.GetInt("graphics.particles.effects") == 1;
        menuSettingsGraphicsParticlesAmbient.value = PlayerPrefs.GetInt("graphics.particles.ambient") == 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiManager.GetActiveWindows().Length > 0)
            {
                uiManager.CloseWindow("#top");
            }
            else
            {
                uiManager.OpenWindow("menu");
            }
        }
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            uiManager.ToggleWindow("hacks");
        }
    }
}
