using UnityEngine;

public enum CharacterSide
{
    leftCharacter,
    rightCharacter
}

[System.Serializable]
public class DialogBox
{
    public CharacterSide characterSide;
    
    [TextArea] public string dialog;
}
