using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Mock server-side storage.
/// This version integrates directly with console-driven commands.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private Dictionary<string, string> registeredUsers = new Dictionary<string, string>();
    private Dictionary<string, CharacterData> characterDatabase = new Dictionary<string, CharacterData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    #region Account Handling

    public void SendLoginRequest(string username, string password, Action<bool, string> callback)
    {
        if (registeredUsers.ContainsKey(username) && registeredUsers[username] == password)
        {
            callback?.Invoke(true, username);
        }
        else
        {
            callback?.Invoke(false, "Invalid username or password.");
        }
    }

    public void SendRegisterRequest(string username, string password, string email, Action<bool, string> callback)
    {
        if (registeredUsers.ContainsKey(username))
        {
            callback?.Invoke(false, "Username already exists.");
            return;
        }

        registeredUsers[username] = password;
        callback?.Invoke(true, $"Account '{username}' registered successfully.");
    }

    #endregion

    #region Character Handling

    public void SendCharacterCreationRequest(CharacterData data)
    {
        if (characterDatabase.ContainsKey(data.characterName))
        {
            Debug.LogError("Character name already exists!");
            return;
        }

        characterDatabase[data.characterName] = data;
        Debug.Log($"[NetworkManager] Character '{data.characterName}' created.");
    }

    public List<CharacterData> GetAllCharacters()
    {
        return new List<CharacterData>(characterDatabase.Values);
    }

    #endregion
}
