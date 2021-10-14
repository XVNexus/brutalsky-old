using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [Header("Object References")]
    public GameManager gameManager;
    public InputManager inputManager;

    // FPS group
    public VisualElement containerFps;
    public Label fpsLabel;

    // Message group
    public VisualElement containerMessage;
    public Label messageLabel;

    // Menu group
    public VisualElement containerMenu;
    public VisualElement panelMenu;
    public Button menuButtonContinue;
    public Button menuButtonLevelSelector;
    public Button menuButtonPrev;
    public Button menuButtonRestart;
    public Button menuButtonNext;
    public Button menuButtonSettings;
    public Button menuButtonChangelog;
    public Button menuButtonHelp;
    public Button menuButtonMainMenu;
    public Button menuButtonExit;

    // Settings group
    public VisualElement containerMenuSettings;
    public VisualElement panelMenuSettings;
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
    public VisualElement panelMenuHelp;

    // Changelog group
    public VisualElement containerMenuChangelog;
    public VisualElement panelMenuChangelog;

    private UIManager uiManager;
    private bool loadedChangelog = false;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // FPS group
        containerFps = root.Q<VisualElement>("container-fps");
        fpsLabel = containerFps.Q<Label>("fps-label");

        // Message group
        containerMessage = root.Q<VisualElement>("container-message");
        messageLabel = containerMessage.Q<Label>("message-label");

        // Menu group
        containerMenu = root.Q<VisualElement>("container-menu");
        panelMenu = containerMenu.Q<VisualElement>("panel-menu");
        menuButtonContinue = panelMenu.Q<Button>("menu-button-continue");
        menuButtonLevelSelector = panelMenu.Q<Button>("menu-button-level-selector");
        menuButtonPrev = panelMenu.Q<Button>("menu-button-prev");
        menuButtonRestart = panelMenu.Q<Button>("menu-button-restart");
        menuButtonNext = panelMenu.Q<Button>("menu-button-next");
        menuButtonSettings = panelMenu.Q<Button>("menu-button-settings");
        menuButtonChangelog = panelMenu.Q<Button>("menu-button-changelog");
        menuButtonHelp = panelMenu.Q<Button>("menu-button-help");
        menuButtonMainMenu = panelMenu.Q<Button>("menu-button-main-menu");
        menuButtonExit = panelMenu.Q<Button>("menu-button-exit");

        menuButtonContinue.clicked += ActMenuContinue;
        menuButtonLevelSelector.clicked += ActMenuLevelSelector;
        menuButtonPrev.clicked += ActMenuPrev;
        menuButtonRestart.clicked += ActMenuRestart;
        menuButtonNext.clicked += ActMenuNext;
        menuButtonSettings.clicked += ActMenuSettings;
        menuButtonChangelog.clicked += ActMenuChangelog;
        menuButtonHelp.clicked += ActMenuHelp;
        menuButtonMainMenu.clicked += ActMenuMainMenu;
        menuButtonExit.clicked += ActMenuExit;

        // Settings group
        containerMenuSettings = root.Q<VisualElement>("container-menu-settings");
        panelMenuSettings = containerMenuSettings.Q<VisualElement>("panel-menu-settings");
        menuSettingsButtonSaveChanges = panelMenuSettings.Q<Button>("menu-settings-button-save-changes");
        menuSettingsButtonDiscardChanges = panelMenuSettings.Q<Button>("menu-settings-button-discard-changes");
        menuSettingsFoldoutControls = panelMenuSettings.Q<Foldout>("menu-settings-foldout-controls");
        menuSettingsControlsPlayer1MoveUp = panelMenuSettings.Q<TextField>("menu-settings-controls-player-1-move-up");
        menuSettingsControlsPlayer1MoveDown = panelMenuSettings.Q<TextField>("menu-settings-controls-player-1-move-down");
        menuSettingsControlsPlayer1MoveLeft = panelMenuSettings.Q<TextField>("menu-settings-controls-player-1-move-left");
        menuSettingsControlsPlayer1MoveRight = panelMenuSettings.Q<TextField>("menu-settings-controls-player-1-move-right");
        menuSettingsControlsPlayer1Ability = panelMenuSettings.Q<TextField>("menu-settings-controls-player-1-ability");
        menuSettingsControlsPlayer2MoveUp = panelMenuSettings.Q<TextField>("menu-settings-controls-player-2-move-up");
        menuSettingsControlsPlayer2MoveDown = panelMenuSettings.Q<TextField>("menu-settings-controls-player-2-move-down");
        menuSettingsControlsPlayer2MoveLeft = panelMenuSettings.Q<TextField>("menu-settings-controls-player-2-move-left");
        menuSettingsControlsPlayer2MoveRight = panelMenuSettings.Q<TextField>("menu-settings-controls-player-2-move-right");
        menuSettingsControlsPlayer2Ability = panelMenuSettings.Q<TextField>("menu-settings-controls-player-2-ability");
        menuSettingsFoldoutGraphics = panelMenuSettings.Q<Foldout>("menu-settings-foldout-graphics");
        menuSettingsGraphicsParticlesEffects = panelMenuSettings.Q<Toggle>("menu-settings-graphics-particles-effects");
        menuSettingsGraphicsParticlesAmbient = panelMenuSettings.Q<Toggle>("menu-settings-graphics-particles-ambient");

        menuSettingsButtonSaveChanges.clicked += ActMenuSettingsSaveChanges;
        menuSettingsButtonDiscardChanges.clicked += ActMenuSettingsDiscardChanges;

        // Help group
        containerMenuHelp = root.Q<VisualElement>("container-menu-help");
        panelMenuHelp = containerMenuHelp.Q<ScrollView>("panel-menu-help");

        // Changelog group
        containerMenuChangelog = root.Q<VisualElement>("container-menu-changelog");
        panelMenuChangelog = containerMenuChangelog.Q<ScrollView>("panel-menu-changelog");


        // Set up UI system
        containerMessage.visible = false;
        var uiWindowMenu = new UIWindow("menu", 0, containerMenu);
        var uiWindowMenuSettings = new UIWindow("menu.settings", 1, containerMenuSettings, uiWindowMenu);
        var uiWindowMenuHelp = new UIWindow("menu.help", 1, containerMenuHelp, uiWindowMenu);
        var uiWindowMenuChangelog = new UIWindow("menu.changelog", 1, containerMenuChangelog, uiWindowMenu);
        uiManager = new UIManager(new UIWindow[] { uiWindowMenu, uiWindowMenuSettings, uiWindowMenuHelp, uiWindowMenuChangelog });

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

    private void ActMenuChangelog()
    {
        uiManager.OpenWindow("menu.changelog");
        if (!loadedChangelog)
        {
            LoadChangelog();
            loadedChangelog = true;
        }
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

    public void SetFpsLabel(int fps)
    {
        fpsLabel.text = $"FPS: {fps}";
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

    public void LoadChangelog()
    {
        // Load CHANGELOG.mdfile from github url
        var changelogFileUrl = "https://raw.githubusercontent.com/XVNexus/brutalsky/main/CHANGELOG.md";
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
        {
            NoCache = true
        };
        var changelogFile = "";
        using (HttpResponseMessage httpResponse = httpClient.GetAsync(changelogFileUrl).Result)
        {
            var content = httpResponse.Content;
            changelogFile = content.ReadAsStringAsync().Result;
        }

        // Parse changelog file into a tree structure
        var changelog = Changelog.ParseMarkdown(changelogFile);

        // Generate changelog UI from tree
        foreach (var version in changelog.Versions)
        {
            var uiFoldout = new Foldout();
            uiFoldout.AddToClassList("base");
            uiFoldout.AddToClassList("foldout");
            uiFoldout.style.height = StyleKeyword.Auto;
            uiFoldout.style.unityFontStyleAndWeight = FontStyle.Bold;

            var versionTitle = $"V {version.Major}.{version.Minor}";
            versionTitle += version.IsPatch ? $".{version.Patch}" : "";
            versionTitle += version.HasName ? $" // {version.Name}" : "";
            uiFoldout.text = versionTitle;
            uiFoldout.style.unityFontStyleAndWeight = !version.IsPatch ? FontStyle.Bold : FontStyle.BoldAndItalic;

            foreach (var section in version.Sections)
            {
                var changePrefix = "";
                var colorMajor = new Color();
                var colorMinor = new Color();
                switch (section.Type)
                {
                    case ChangelogSectionType.Added:
                        changePrefix = "+";
                        colorMajor = new Color(.2f, 1f, .2f);
                        colorMinor = new Color(.8f, 1f, .8f);
                        break;
                    case ChangelogSectionType.Changed:
                        changePrefix = ">";
                        colorMajor = new Color(1f, 1f, .2f);
                        colorMinor = new Color(1f, 1f, .8f);
                        break;
                    case ChangelogSectionType.Removed:
                        changePrefix = "-";
                        colorMajor = new Color(1f, .2f, .2f);
                        colorMinor = new Color(1f, .8f, .8f);
                        break;
                    case ChangelogSectionType.Fixed:
                        changePrefix = "~";
                        colorMajor = new Color(.2f, .6f, 1f);
                        colorMinor = new Color(.8f, .9f, 1f);
                        break;
                    case ChangelogSectionType.Bugs:
                        changePrefix = "×";
                        colorMajor = new Color(1f, .6f, .2f);
                        colorMinor = new Color(1f, .9f, .8f);
                        break;
                }

                var header = new Label();
                header.AddToClassList("base");
                header.AddToClassList("label");
                header.AddToClassList("label-header");
                header.style.color = colorMajor;
                header.text = section.Type.ToString();

                uiFoldout.Add(header);

                foreach (var change in section.Changes)
                {
                    var label = new Label();
                    label.AddToClassList("base");
                    label.AddToClassList("label");
                    label.AddToClassList("label-info");
                    label.style.color = colorMinor;
                    label.style.marginLeft = 50 * change.Indention;
                    label.text = $"<b>{changePrefix}</b> {Regex.Replace(change.Text, @"\[.+?\]\(.+?\) ?| ?\[.+?\]\(.+?\)|`", "")}";

                    uiFoldout.Add(label);
                }
            }

            uiFoldout.value = false;
            panelMenuChangelog.Add(uiFoldout);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiManager.GetActiveWindows().Length > 0)
            {
                uiManager.CloseWindow("#top");
                if (!uiManager.GetWindow("menu").Active)
                {
                }
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

        // If any UI windows are visible, pause the game
        if (uiManager.IsAnyWindowOpen() && !gameManager.Paused)
        {
            gameManager.Pause();
        }
        else if (uiManager.IsAllWindowsClosed() && gameManager.Paused)
        {
            gameManager.Unpause();
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
            if (value != null)
            {
                if (!value.children.Contains(this))
                {
                    value.children.Add(this);
                }
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
        Parent = parent;
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

public enum ChangelogSectionType
{
    Added,
    Changed,
    Removed,
    Fixed,
    Bugs,
    None
}

public enum ChangelogLineType
{
    Version,
    Section,
    Change,
    None
}

public class Changelog
{
    public List<ChangelogVersion> Versions { get; set; }

    public Changelog(List<ChangelogVersion> versions = null)
    {
        Versions = versions ?? new List<ChangelogVersion>();
    }

    public Changelog()
    {
        Versions = new List<ChangelogVersion>();
    }

    public static Changelog ParseMarkdown(string markdownFile)
    {
        var result = new Changelog();
        var activeVersion = new ChangelogVersion();
        var activeSection = new ChangelogSection();
        var lines = markdownFile.Split('\n');
        var lineType = ChangelogLineType.None;
        foreach (var line in lines)
        {
            // Calculate line type
            if (!(line.Length == 0 || line.StartsWith("# ")))
            {
                if (line.StartsWith("## "))
                {
                    lineType = ChangelogLineType.Version;
                }
                else if (line.StartsWith("### "))
                {
                    lineType = ChangelogLineType.Section;
                }
                else if (Regex.IsMatch(line, @"^ *- "))
                {
                    lineType = ChangelogLineType.Change;
                }
            }
            else
            {
                lineType = ChangelogLineType.None;
            }
            // Parse line based on calculated line type
            switch (lineType)
            {
                case ChangelogLineType.Version:
                    var versionName = Regex.Match(line, @"(?<=\/\/ +)[^+]+?(?= +&nbsp;)").Groups[0].Value;
                    var versionNumbers = Regex.Match(line, @"\d+(\.\d+)+").Groups[0].Value.Split('.');
                    var versionMajor = int.Parse(versionNumbers[0]);
                    var versionMinor = int.Parse(versionNumbers[1]);
                    var versionTimestampNumbers = Regex.Split(Regex.Match(line, @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}").Groups[0].Value, @"[-: ]");
                    var versionTimestamp = new DateTime(
                        int.Parse(versionTimestampNumbers[0]),
                        int.Parse(versionTimestampNumbers[1]),
                        int.Parse(versionTimestampNumbers[2]),
                        int.Parse(versionTimestampNumbers[3]),
                        int.Parse(versionTimestampNumbers[4]),
                        0
                    );
                    var versionPatch = versionNumbers.Length == 3 ? int.Parse(versionNumbers[2]) : 0;

                    activeVersion = new ChangelogVersion(versionName, versionMajor, versionMinor, versionPatch, DateTime.Now);
                    result.Versions.Add(activeVersion);
                    break;
                case ChangelogLineType.Section:
                    var sectionName = line.Substring(4).Trim();

                    activeSection = new ChangelogSection((ChangelogSectionType)Enum.Parse(typeof(ChangelogSectionType), sectionName, true));
                    activeVersion.Sections.Add(activeSection);
                    break;
                case ChangelogLineType.Change:
                    var changeIndentionSpaces = Regex.Match(line, @"^ +").Groups[0].Value.Length;
                    var changeIndention = changeIndentionSpaces / 2;
                    var changeText = line.Substring(changeIndention * 2 + 2).Trim();

                    activeSection.Changes.Add(new ChangelogChange(changeText, changeIndention));
                    break;
                default:
                    break;
            }
        }
        return result;
    }

    public override string ToString()
    {
        var result = "";
        foreach (var version in Versions)
        {
            result += $"{version}\n\n";
        }
        return result;
    }
}

public class ChangelogVersion
{
    public string Name { get; set; }
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Patch { get; set; }
    public DateTime Timestamp { get; set; }
    public List<ChangelogSection> Sections { get; set; }
    public bool IsPatch { get => Patch > 0; }
    public bool HasName { get => Name.Length > 0; }

    public ChangelogVersion(string name, int major, int minor, int patch, DateTime timestamp, List<ChangelogSection> sections = null)
    {
        Name = name;
        Major = major;
        Minor = minor;
        Patch = patch;
        Timestamp = timestamp;
        Sections = sections ?? new List<ChangelogSection>();
    }

    public ChangelogVersion()
    {
        Name = "";
        Major = 0;
        Minor = 0;
        Patch = 0;
        Timestamp = DateTime.Now;
        Sections = new List<ChangelogSection>();
    }

    public override string ToString()
    {
        var result = $"## ";
        var nameDisplayed = Name.Length > 0;
        var patchDisplayed = Patch > 0;
        result += patchDisplayed ? "*" : "";                                // Add opening italics asterisk IF version is patch
        result += $"V {Major}.{Minor}";                                     // Add major and minor version numbers
        result += patchDisplayed ? $".{Patch}" : "";                        // Add patch version number IF version is patch
        result += nameDisplayed ? $" // {Name}" : "";                       // Add version name IF version name is not empty
        result += $" &nbsp; `{Timestamp.ToString("yyyy/MM/dd HH:mm")}`";    // Add timestamp
        result += patchDisplayed ? "*" : "";                                // Add closing italics asterisk IF version is patch
        // Add section strings
        foreach (var section in Sections)
        {
            result += $"\n{section}";
        }
        return result;
    }
}

public class ChangelogSection
{
    public ChangelogSectionType Type { get; set; }
    public List<ChangelogChange> Changes { get; set; }

    public ChangelogSection(ChangelogSectionType type, List<ChangelogChange> changes = null)
    {
        Type = type;
        Changes = changes ?? new List<ChangelogChange>();
    }

    public ChangelogSection()
    {
        Type = ChangelogSectionType.None;
        Changes = new List<ChangelogChange>();
    }

    public override string ToString()
    {
        var result = $"### {Type}";
        foreach (var change in Changes)
        {
            result += $"\n{change}";
        }
        return result;
    }
}

public class ChangelogChange
{
    public string Text { get; set; }
    public int Indention { get; set; }

    public ChangelogChange(string text, int indentation = 0)
    {
        Text = text;
        Indention = indentation;
    }

    public ChangelogChange()
    {
        Text = "";
        Indention = 0;
    }

    public override string ToString()
    {
        var indentionString = "";
        for (var i = 0; i < Indention; i++)
        {
            indentionString += "  ";
        }
        return indentionString + $"- {Text}";
    }
}
