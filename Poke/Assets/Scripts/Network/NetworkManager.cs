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
    private Dictionary<string, List<CharacterData>> characterDatabase = new Dictionary<string, List<CharacterData>>();

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
        if (data == null)
        {
            Debug.LogError("Character data cannot be null.");
            return;
        }

        if (string.IsNullOrEmpty(data.ownerUsername))
        {
            Debug.LogError("Character must have a valid owner before creation.");
            return;
        }

        if (!characterDatabase.ContainsKey(data.ownerUsername))
        {
            characterDatabase[data.ownerUsername] = new List<CharacterData>();
        }

        List<CharacterData> ownerCharacters = characterDatabase[data.ownerUsername];

        if (ownerCharacters.Exists(c => c.characterName == data.characterName))
        {
            Debug.LogError("Character name already exists!");
            return;
        }

        ownerCharacters.Add(data);
        Debug.Log($"[NetworkManager] Character '{data.characterName}' created.");
    }

    public List<CharacterData> GetAllCharacters()
    {
        var accountManager = AccountManager.Instance;
        if (accountManager == null)
        {
            Debug.LogError("AccountManager instance is not available. Cannot fetch characters.");
            return new List<CharacterData>();
        }

        string ownerUsername = accountManager.GetLoggedInUser();
        if (string.IsNullOrEmpty(ownerUsername))
        {
            Debug.LogWarning("No user is currently logged in. Returning empty character list.");
            return new List<CharacterData>();
        }

        if (!characterDatabase.TryGetValue(ownerUsername, out List<CharacterData> ownerCharacters))
        {
            return new List<CharacterData>();
        }

        return new List<CharacterData>(ownerCharacters);
    }

    #endregion
}
