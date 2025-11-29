using UnityEngine;

public enum CharacterNames
{
    Seth,
    Osiris,
    None
}

[System.Serializable]
public class DialogCharacter
{
    public CharacterNames characterNames = CharacterNames.Seth;
}
