using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

/// <summary>
/// Mock server-side storage.
/// This version integrates directly with console-driven commands.
/// </summary>
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private class UserRecord
    {
        public string Email { get; }
        public byte[] Salt { get; }
        public byte[] PasswordHash { get; }

        public UserRecord(string email, byte[] salt, byte[] passwordHash)
        {
            Email = email;
            Salt = salt;
            PasswordHash = passwordHash;
        }
    }

    private Dictionary<string, UserRecord> registeredUsers = new Dictionary<string, UserRecord>();
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int HashIterations = 10000;
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
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            callback?.Invoke(false, "Username and password are required.");
            return;
        }

        if (!registeredUsers.TryGetValue(username, out var record))
        {
            callback?.Invoke(false, "Invalid username or password.");
            return;
        }

        var hashedPassword = HashPassword(password, record.Salt);
        if (AreHashesEqual(hashedPassword, record.PasswordHash))
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
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
        {
            callback?.Invoke(false, "Username, password, and email are required.");
            return;
        }

        if (registeredUsers.ContainsKey(username))
        {
            callback?.Invoke(false, "Username already exists.");
            return;
        }

        var salt = GenerateSalt();
        var passwordHash = HashPassword(password, salt);
        registeredUsers[username] = new UserRecord(email, salt, passwordHash);
        callback?.Invoke(true, $"Account '{username}' registered successfully.");
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return salt;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, HashIterations, HashAlgorithmName.SHA256))
        {
            return deriveBytes.GetBytes(HashSize);
        }
    }

    private static bool AreHashesEqual(byte[] left, byte[] right)
    {
        if (left == null || right == null || left.Length != right.Length)
        {
            return false;
        }

        var result = 0;
        for (var i = 0; i < left.Length; i++)
        {
            result |= left[i] ^ right[i];
        }

        return result == 0;
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
