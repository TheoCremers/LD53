using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : BaseOptionsMenu
{
    #region EventChannels

    public VoidEventChannel ShowDialog;

    #endregion

    public TextMeshProUGUI DialogTextMesh;
    public TextMeshProUGUI ContinueButtonTextMesh;
    public Button ContinueButton;
    public Image LeftImage;
    public Image RightImage;

    public void SetDialogWithSO(DialogSO dialogSO)
    {
        DialogTextMesh.text = dialogSO.DialogText;
        ContinueButtonTextMesh.text = dialogSO.DialogButtonText;

        if (dialogSO.CharacterPortrait == null)
        {
            LeftImage.enabled = false;
            RightImage.enabled = false;
        }
        else {
            if (dialogSO.PortraitOnRightSide)
            {
                LeftImage.enabled = false;
                RightImage.enabled = true;
                RightImage.sprite = dialogSO.CharacterPortrait;
            }
            else
            {
                RightImage.enabled = false;
                LeftImage.enabled = true;
                LeftImage.sprite = dialogSO.CharacterPortrait;
            }
        }
        
    }
}
