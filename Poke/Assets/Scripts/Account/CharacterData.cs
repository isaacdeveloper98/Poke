using UnityEngine;

public enum PlayerRace { Human, Elf, Dwarf }
public enum PlayerGender { Male, Female, NonBinary }

[System.Serializable]
public class CharacterData
{
    public string ownerUsername;
    public string characterName;
    public PlayerRace race;
    public PlayerGender gender;
    public int modelId;
    public int level;
    public Vector3 lastPosition;
}
