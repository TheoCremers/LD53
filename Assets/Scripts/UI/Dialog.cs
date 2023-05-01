using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class Dialog : BaseOptionsMenu
{
    public static Dialog Instance;

    public TextMeshProUGUI DialogTextMesh;
    public TextMeshProUGUI ContinueButtonTextMesh;
    public TextMeshProUGUI SecondButtonTextMesh;
    public Button ContinueButton;
    public Button SecondButton;
    public Image LeftImage;
    public Image RightImage;

    private bool _dialogInProgress = false;
    private int _buttonClickedIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        ContinueButton.onClick.AddListener(() => StopDialog(1));
        SecondButton.onClick.AddListener(() => StopDialog(2));
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    public void OnDestroy()
    {
        ContinueButton.onClick.RemoveListener(() => StopDialog(1));
        SecondButton.onClick.AddListener(() => StopDialog(2));
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

        if (dialogSO.SecondButtonActive)
        {
            SecondButton.gameObject.SetActive(true);
            SecondButtonTextMesh.text = dialogSO.SecondButtonText;
        }
        else
        {
            SecondButton.gameObject.SetActive(false);
        }
    }

    public void StartDialog()
    {
        _dialogInProgress = true;
    }

    public void StopDialog(int buttonClickedIndex) {
        _buttonClickedIndex = buttonClickedIndex;
        _dialogInProgress = false;
    }

    public async Task<int> WaitForDialogToFinish()
    {
        while (_dialogInProgress)
        {
            await Task.Yield();
        }
        return _buttonClickedIndex;
    }
}
