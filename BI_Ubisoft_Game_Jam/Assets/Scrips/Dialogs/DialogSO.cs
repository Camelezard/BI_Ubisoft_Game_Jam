using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogSO", menuName = "Scriptable Objects/DialogSO")]
public class DialogSO : ScriptableObject
{
    public DialogCharacter leftCharacter, rightCharacter;
    public List<DialogBox> dialogList;
}
