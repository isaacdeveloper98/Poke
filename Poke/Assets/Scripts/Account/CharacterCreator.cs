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
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Console command to create a character for the logged-in user.
    /// </summary>
    public void CreateCharacter(string name, PlayerRace race, PlayerGender gender, int modelId = 0)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("Character name cannot be empty.");
            return;
        }

        var accountManager = AccountManager.Instance;
        if (accountManager == null)
        {
            Debug.LogError("AccountManager instance is not available. Cannot create character.");
            return;
        }

        string ownerUsername = accountManager.GetLoggedInUser();
        if (string.IsNullOrEmpty(ownerUsername))
        {
            Debug.LogError("No user is currently logged in. Cannot create character.");
            return;
        }

        CharacterData newChar = new CharacterData
        {
            ownerUsername = ownerUsername,
            characterName = name,
            race = race,
            gender = gender,
            modelId = modelId,
            level = 1,
            lastPosition = Vector3.zero
        };

        NetworkManager.Instance.SendCharacterCreationRequest(newChar);
    }
}
