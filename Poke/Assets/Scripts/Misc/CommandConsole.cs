using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Developer/admin console for commands.
/// Can register/login accounts, create characters, and more.
/// </summary>
public class CommandConsole : MonoBehaviour
{
    public static CommandConsole Instance { get; private set; }

    private Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();
    private bool isConsoleOpen = false;
    private string input = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        RegisterCommands();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) // toggle with ~
        {
            isConsoleOpen = !isConsoleOpen;
        }
    }

    private void OnGUI()
    {
        if (!isConsoleOpen) return;

        GUI.Box(new Rect(10, 10, 600, 30), "");
        input = GUI.TextField(new Rect(15, 15, 590, 20), input);

        if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown)
        {
            ProcessCommand(input);
            input = "";
        }
    }

    private void RegisterCommands()
    {
        commands["register"] = args =>
        {
            if (args.Length < 3) { Debug.Log("Usage: register <username> <password> <email>"); return; }
            AccountManager.Instance.Register(args[0], args[1], args[2]);
        };

        commands["login"] = args =>
        {
            if (args.Length < 2) { Debug.Log("Usage: login <username> <password>"); return; }
            AccountManager.Instance.Login(args[0], args[1]);
        };

        commands["createchar"] = args =>
        {
            if (args.Length < 3) { Debug.Log("Usage: createchar <name> <race> <gender>"); return; }

            if (!Enum.TryParse(args[1], true, out PlayerRace race)) { Debug.Log("Invalid race."); return; }
            if (!Enum.TryParse(args[2], true, out PlayerGender gender)) { Debug.Log("Invalid gender."); return; }

            CharacterCreator.Instance.CreateCharacter(args[0], race, gender);
        };

        commands["listchars"] = args =>
        {
            var chars = NetworkManager.Instance.GetAllCharacters();
            foreach (var c in chars)
            {
                Debug.Log($"Char: {c.characterName} | Race: {c.race} | Gender: {c.gender} | Lv: {c.level}");
            }
        };
    }

    private void ProcessCommand(string inputLine)
    {
        if (string.IsNullOrWhiteSpace(inputLine)) return;

        string[] split = inputLine.Split(' ');
        string cmd = split[0].ToLower();
        string[] args = new string[split.Length - 1];
        Array.Copy(split, 1, args, 0, args.Length);

        if (commands.ContainsKey(cmd))
        {
            commands[cmd].Invoke(args);
        }
        else
        {
            Debug.Log("Unknown command: " + cmd);
        }
    }
}
