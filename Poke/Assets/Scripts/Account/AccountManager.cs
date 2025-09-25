using System;
using UnityEngine;

/// <summary>
/// Handles account registration and login.
/// Works with the CommandConsole instead of UI.
/// Implements Singleton.
/// </summary>
public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance { get; private set; }

    /// <summary>
    /// Raised whenever a login attempt succeeds. Provides the username of the logged in user.
    /// </summary>
    public event Action<string> LoginSucceeded;

    /// <summary>
    /// Raised whenever a login attempt fails. Provides the server error message.
    /// </summary>
    public event Action<string> LoginFailed;

    /// <summary>
    /// Raised when a user logs out of the session.
    /// </summary>
    public event Action LoggedOut;

    /// <summary>
    /// Raised when a registration request completes successfully. Provides the descriptive server message.
    /// </summary>
    public event Action<string> RegistrationSucceeded;

    /// <summary>
    /// Raised when a registration request fails. Provides the error message supplied by the server.
    /// </summary>
    public event Action<string> RegistrationFailed;

    private string loggedInUser = null;

    /// <summary>
    /// Returns true if a user is currently logged in.
    /// </summary>
    public bool IsLoggedIn => !string.IsNullOrEmpty(loggedInUser);

    /// <summary>
    /// Returns the username of the currently logged-in account or <c>null</c> if no session is active.
    /// </summary>
    public string LoggedInUser => loggedInUser;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[AccountManager] Duplicate instance detected. Destroying the newest instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Console command handler for registration.
    /// </summary>
    public void Register(string username, string password, string email)
    {
        if (!ValidateRegistrationInput(username, password, email))
        {
            return;
        }

        if (!TryGetNetworkManager(out NetworkManager network))
        {
            return;
        }

        network.SendRegisterRequest(username.Trim(), password, email.Trim(), HandleRegisterResponse);
    }

    /// <summary>
    /// Console command handler for login.
    /// </summary>
    public void Login(string username, string password)
    {
        if (!ValidateLoginInput(username, password))
        {
            return;
        }

        if (!TryGetNetworkManager(out NetworkManager network))
        {
            return;
        }

        network.SendLoginRequest(username.Trim(), password, HandleLoginResponse);
    }

    /// <summary>
    /// Logs the current user out and clears the cached session information.
    /// </summary>
    public void Logout()
    {
        if (!IsLoggedIn)
        {
            Debug.LogWarning("[AccountManager] Logout requested but no user is currently logged in.");
            return;
        }

        string previousUser = loggedInUser;
        loggedInUser = null;

        Debug.Log($"[AccountManager] User '{previousUser}' logged out.");
        LoggedOut?.Invoke();
    }

    private void HandleLoginResponse(bool success, string message)
    {
        if (success)
        {
            loggedInUser = message;
            Debug.Log($"[AccountManager] User '{loggedInUser}' logged in successfully.");
            LoginSucceeded?.Invoke(loggedInUser);
        }
        else
        {
            Debug.LogWarning($"[AccountManager] Login failed: {message}");
            LoginFailed?.Invoke(message);
        }
    }

    private void HandleRegisterResponse(bool success, string message)
    {
        if (success)
        {
            Debug.Log($"[AccountManager] {message}");
            RegistrationSucceeded?.Invoke(message);
        }
        else
        {
            Debug.LogWarning($"[AccountManager] Registration failed: {message}");
            RegistrationFailed?.Invoke(message);
        }
    }

    public string GetLoggedInUser()
    {
        return loggedInUser;
    }

    private bool TryGetNetworkManager(out NetworkManager networkManager)
    {
        networkManager = NetworkManager.Instance;
        if (networkManager == null)
        {
            Debug.LogError("[AccountManager] NetworkManager instance is not available.");
            return false;
        }

        return true;
    }

    private bool ValidateRegistrationInput(string username, string password, string email)
    {
        if (!ValidateUsername(username))
        {
            Debug.LogError("[AccountManager] Registration failed: Username cannot be empty or whitespace.");
            return false;
        }

        if (!ValidatePassword(password))
        {
            Debug.LogError("[AccountManager] Registration failed: Password must be at least 6 characters long.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            Debug.LogError("[AccountManager] Registration failed: Email address appears to be invalid.");
            return false;
        }

        return true;
    }

    private bool ValidateLoginInput(string username, string password)
    {
        if (!ValidateUsername(username))
        {
            Debug.LogError("[AccountManager] Login failed: Username cannot be empty or whitespace.");
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("[AccountManager] Login failed: Password cannot be empty.");
            return false;
        }

        return true;
    }

    private bool ValidateUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username);
    }

    private bool ValidatePassword(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Length >= 6;
    }
}
