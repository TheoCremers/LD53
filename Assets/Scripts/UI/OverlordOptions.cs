using UnityEngine.UI;

public class OverlordOptions : BaseOptionsMenu
{
    #region EventChannels

    public VoidEventChannel ShowShiftButtonsEvent;
    public VoidEventChannel ShowRotateButtonsEvent;
    public VoidEventChannel StartOverlordTurnEvent;
    public VoidEventChannel StartIdleEvent;

    #endregion

    public ButtonWithCost ShowShiftButton;
    public ButtonWithCost ShowRotateButton;
    public Button FinishInterventionButton;

    protected override void Awake()
    {
        base.Awake();

        ShowShiftButton.onClick.AddListener(ShowShiftButtonsEvent.RaiseEvent);
        ShowRotateButton.onClick.AddListener(ShowRotateButtonsEvent.RaiseEvent);
        FinishInterventionButton.onClick.AddListener(StartIdleEvent.RaiseEvent);

        ShowShiftButtonsEvent.OnEventRaised += DisableAndFadeOut;
        ShowRotateButtonsEvent.OnEventRaised += DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised += EnableAndFadeIn;
        StartIdleEvent.OnEventRaised += DisableAndFadeOut;
    }

    private void OnDestroy()
    {
        ShowShiftButton.onClick.RemoveListener(ShowShiftButtonsEvent.RaiseEvent);
        ShowRotateButton.onClick.RemoveListener(ShowRotateButtonsEvent.RaiseEvent);
        FinishInterventionButton.onClick.RemoveListener(StartOverlordTurnEvent.RaiseEvent);

        ShowShiftButtonsEvent.OnEventRaised -= DisableAndFadeOut;
        ShowRotateButtonsEvent.OnEventRaised -= DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised -= EnableAndFadeIn;
        StartIdleEvent.OnEventRaised -= DisableAndFadeOut;
    }

    public async void DisableAndFadeOut()
    {
        ShowShiftButton.interactable = false;
        FinishInterventionButton.interactable = false;
        await FadeOut();
    }

    public async void EnableAndFadeIn()
    {
        SetButtonCosts();
        await FadeIn();
        ShowShiftButton.interactable = true;
        FinishInterventionButton.interactable = true;
    }

    public void SetButtonCosts()
    {
        ShowShiftButton.SetAmount(ResourceManager.Instance.ShiftCost);
        ShowRotateButton.SetAmount(ResourceManager.Instance.RotateCost);
    }
}
