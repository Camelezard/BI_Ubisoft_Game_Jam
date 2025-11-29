using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogSO", menuName = "Scriptable Objects/DialogSO")]
public class DialogSO : ScriptableObject
{
    public CharacterNames leftCharacter, rightCharacter;
    public List<DialogBox> dialogList;
    [TextArea(3, 10)]
    public string description;
}
