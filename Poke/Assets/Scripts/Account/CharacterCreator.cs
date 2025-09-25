using UnityEngine;

/// <summary>
/// Creates new characters through console commands.
/// </summary>
public class CharacterCreator : MonoBehaviour
{
    public static CharacterCreator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[CharacterCreator] Duplicate instance detected. Destroying the newest instance.");
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
    /// Console command to create a character for the logged-in user.
    /// </summary>
    public void CreateCharacter(string name, PlayerRace race, PlayerGender gender, int modelId = 0)
    {
        if (!TryGetNetworkManager(out NetworkManager network))
        {
            return;
        }

        if (!TryGetLoggedInUser(out string owner))
        {
            return;
        }

        if (!CharacterData.TryValidateName(name, out string validationError))
        {
            Debug.LogError($"[CharacterCreator] {validationError}");
            return;
        }

        int sanitizedModelId = Mathf.Max(0, modelId);

        CharacterData newChar = CharacterData.Create(
            name.Trim(),
            race,
            gender,
            sanitizedModelId,
            CharacterData.DefaultLevel,
            Vector3.zero);

        network.SendCharacterCreationRequest(newChar);
        Debug.Log($"[CharacterCreator] Character '{newChar.characterName}' creation requested by '{owner}'.");
    }

    private bool TryGetNetworkManager(out NetworkManager networkManager)
    {
        networkManager = NetworkManager.Instance;
        if (networkManager == null)
        {
            Debug.LogError("[CharacterCreator] NetworkManager instance is not available.");
            return false;
        }

        return true;
    }

    private bool TryGetLoggedInUser(out string username)
    {
        username = null;

        AccountManager accountManager = AccountManager.Instance;
        if (accountManager == null)
        {
            Debug.LogError("[CharacterCreator] AccountManager instance is not available.");
            return false;
        }

        if (!accountManager.IsLoggedIn)
        {
            Debug.LogError("[CharacterCreator] Cannot create character because no user is currently logged in.");
            return false;
        }

        username = accountManager.LoggedInUser;
        return true;
    }
}
