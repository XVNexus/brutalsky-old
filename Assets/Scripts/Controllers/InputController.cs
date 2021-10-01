using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("Object References")]
    public UIController uiController;

    void Update()
    {

    }

    private KeyCode KeyNameToKeyCode(string name)
    {
        // Sanitize input string (remove all non letter or number characters)
        var nameSanitized = Regex.Replace(name, @"[^a-z0-9]", "");
        // Parse sanitized input into enum
        return (KeyCode)Enum.Parse(typeof(KeyCode), nameSanitized);
    }
}
