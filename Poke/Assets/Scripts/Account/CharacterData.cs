using UnityEngine;

public enum PlayerRace { Human, Elf, Dwarf }
public enum PlayerGender { Male, Female, NonBinary }

[System.Serializable]
public class CharacterData
{
    public const int MinNameLength = 3;
    public const int MaxNameLength = 16;
    public const int DefaultLevel = 1;

    public string characterName = string.Empty;
    public PlayerRace race = PlayerRace.Human;
    public PlayerGender gender = PlayerGender.Male;
    public int modelId;
    public int level = DefaultLevel;
    public Vector3 lastPosition = Vector3.zero;

    public string CharacterName => characterName;
    public PlayerRace Race => race;
    public PlayerGender Gender => gender;
    public int ModelId => modelId;
    public int Level => level;
    public Vector3 LastPosition => lastPosition;

    public static CharacterData Create(string name, PlayerRace race, PlayerGender gender, int modelId, int level, Vector3 lastPosition)
    {
        return new CharacterData
        {
            characterName = name,
            race = race,
            gender = gender,
            modelId = modelId,
            level = Mathf.Max(DefaultLevel, level),
            lastPosition = lastPosition
        };
    }

    public CharacterData Clone()
    {
        return new CharacterData
        {
            characterName = characterName,
            race = race,
            gender = gender,
            modelId = modelId,
            level = level,
            lastPosition = lastPosition
        };
    }

    public static bool TryValidateName(string name, out string validationError)
    {
        validationError = string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            validationError = "Character name cannot be empty or whitespace.";
            return false;
        }

        string trimmedName = name.Trim();
        if (trimmedName.Length < MinNameLength)
        {
            validationError = $"Character name must be at least {MinNameLength} characters long.";
            return false;
        }

        if (trimmedName.Length > MaxNameLength)
        {
            validationError = $"Character name cannot exceed {MaxNameLength} characters.";
            return false;
        }

        return true;
    }

    public override string ToString()
    {
        return $"{characterName} (Race: {race}, Gender: {gender}, Level: {level})";
    }
}
