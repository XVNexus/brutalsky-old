using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class Keybind
{
    public string Id { get; private set; }
    public KeyCode Key { get; set; }

    public Keybind(string id, KeyCode key)
    {
        Id = id;
        Key = key;
    }

    public bool GetKeyDown()
    {
        return Input.GetKeyDown(Key);
    }

    public bool GetKeyUp()
    {
        return Input.GetKeyUp(Key);
    }

    public bool GetKey()
    {
        return Input.GetKey(Key);
    }
}

public class Axis
{
    public string Id { get; private set; }
    public Keybind Negative { get; set; }
    public Keybind Positive { get; set; }

    public Axis(string id, Keybind positive, Keybind negative)
    {
        Id = id;
        Positive = positive;
        Negative = negative;
    }

    public float GetValue()
    {
        return (Negative.GetKey() ? -1 : 0) + (Positive.GetKey() ? 1 : 0);
    }
}

public class InputManager : MonoBehaviour
{
    [Header("Object References")]
    public UIController uiController;
    public Keybind[] keybinds;
    public Axis[] axes;

    void Start()
    {
        UpdateKeybinds();
        InvokeRepeating("UpdateKeybinds", 1f, 1f);
    }

    public bool GetKeyDown(string id)
    {
        return GetKeybind(id).GetKeyDown();
    }

    public bool GetKeyUp(string id)
    {
        return GetKeybind(id).GetKeyUp();
    }

    public bool GetKeyValue(string id)
    {
        return GetKeybind(id).GetKey();
    }

    public float GetAxisValue(string id)
    {
        return GetAxis(id).GetValue();
    }

    public Axis GetAxis(int index)
    {
        return axes[index];
    }

    public Axis GetAxis(string id)
    {
        foreach (var axis in axes)
        {
            if (axis.Id == id)
            {
                return axis;
            }
        }
        return null;
    }

    public Axis GetAxis(Keybind positive, Keybind negative)
    {
        foreach (var axis in axes)
        {
            if (axis.Positive == positive && axis.Negative == negative)
            {
                return axis;
            }
        }
        return null;
    }

    public Keybind GetKeybind(int index)
    {
        return keybinds[index];
    }

    public Keybind GetKeybind(string id)
    {
        foreach (var keybind in keybinds)
        {
            if (keybind.Id == id)
            {
                return keybind;
            }
        }
        return null;
    }

    public Keybind GetKeybind(KeyCode keyCode)
    {
        foreach (var keybind in keybinds)
        {
            if (keybind.Key == keyCode)
            {
                return keybind;
            }
        }
        return null;
    }

    public void UpdateKeybinds()
    {
        keybinds = new Keybind[] {
            new Keybind("player_1.move.up", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer1MoveUp.text)),
            new Keybind("player_1.move.down", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer1MoveDown.text)),
            new Keybind("player_1.move.left", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer1MoveLeft.text)),
            new Keybind("player_1.move.right", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer1MoveRight.text)),
            new Keybind("player_1.ability", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer1Ability.text)),
            new Keybind("player_2.move.up", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer2MoveUp.text)),
            new Keybind("player_2.move.down", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer2MoveDown.text)),
            new Keybind("player_2.move.left", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer2MoveLeft.text)),
            new Keybind("player_2.move.right", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer2MoveRight.text)),
            new Keybind("player_2.ability", KeyNameToKeyCode(uiController.menuSettingsControlsPlayer2Ability.text))
        };
        axes = new Axis[]
        {
            new Axis("player_1.move.horizontal", keybinds[3], keybinds[2]),
            new Axis("player_1.move.vertical", keybinds[0], keybinds[1]),
            new Axis("player_2.move.horizontal", keybinds[8], keybinds[7]),
            new Axis("player_2.move.vertical", keybinds[5], keybinds[6])
        };
    }

    private KeyCode KeyNameToKeyCode(string name)
    {
        return (KeyCode)Enum.Parse(typeof(KeyCode), Regex.Replace(name, @"[^a-z0-9]", ""), true);
    }
}
