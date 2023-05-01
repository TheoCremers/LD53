using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class Dialog : BaseOptionsMenu
{
    #region EventChannels

    #endregion

    public TextMeshProUGUI DialogTextMesh;
    public TextMeshProUGUI ContinueButtonTextMesh;
    public Button ContinueButton;
    public Image LeftImage;
    public Image RightImage;

    private bool _dialogInProgress = false;

    protected override void Awake()
    {
        base.Awake();

        ContinueButton.onClick.AddListener(StopDialog);
    }

    public void OnDestroy()
    {
        ContinueButton.onClick.RemoveListener(StopDialog);
    }

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

    public void StartDialog()
    {
        _dialogInProgress = true;
    }

    public void StopDialog() {
        _dialogInProgress = false;
    }

    public async Task WaitForDialogToFinish()
    {
        while (_dialogInProgress)
        {
            await Task.Yield();
        }
    }
}
