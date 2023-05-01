using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Dialog")]
public class DialogSO : ScriptableObject
{
    public Sprite CharacterPortrait = null;
    public bool PortraitOnRightSide = false;
    [TextArea(10, 20)]
    public string DialogText = "PlaceHolderText";
    public string DialogButtonText = "...";
    public bool SecondButtonActive = false;
    public string SecondButtonText = "...";
}
