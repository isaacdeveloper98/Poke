using UnityEngine;
using System;

/// <summary>
/// Handles account registration and login. 
/// Works with the CommandConsole instead of UI.
/// Implements Singleton.
/// </summary>
public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    private string loggedInUser = null;

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

    /// <summary>
    /// Console command handler for registration.
    /// </summary>
    public void Register(string username, string password, string email)
    {
        NetworkManager.Instance.SendRegisterRequest(username, password, email, HandleRegisterResponse);
    }

    /// <summary>
    /// Console command handler for login.
    /// </summary>
    public void Login(string username, string password)
    {
        NetworkManager.Instance.SendLoginRequest(username, password, HandleLoginResponse);
    }

    private void HandleLoginResponse(bool success, string message)
    {
        Debug.Log(message);

        if (success)
        {
            loggedInUser = message; // message could include username
            Debug.Log($"[AccountManager] User '{loggedInUser}' logged in successfully.");
        }
    }

    private void HandleRegisterResponse(bool success, string message)
    {
        Debug.Log(message);
    }

    public string GetLoggedInUser()
    {
        return loggedInUser;
    }
}
