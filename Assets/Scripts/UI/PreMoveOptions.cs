using UnityEngine.UI;

public class PreMoveOptions : BaseOptionsMenu
{
    #region EventChannels

    public VoidEventChannel StartMimicTurnEvent;
    public VoidEventChannel StartOverlordTurnEvent;
    public VoidEventChannel StartIdleEvent;

    #endregion

    public ButtonWithDoubleCost ProceedButton;
    public Button InterveneButton;

    protected override void Awake()
    {
        base.Awake();

        ProceedButton.onClick.AddListener(StartMimicTurnEvent.RaiseEvent);
        InterveneButton.onClick.AddListener(StartOverlordTurnEvent.RaiseEvent);

        StartMimicTurnEvent.OnEventRaised += DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised += DisableAndFadeOut;
        StartIdleEvent.OnEventRaised += EnableAndFadeIn;
    }

    private void OnDestroy()
    {
        ProceedButton.onClick.RemoveListener(StartMimicTurnEvent.RaiseEvent);
        InterveneButton.onClick.RemoveListener(StartOverlordTurnEvent.RaiseEvent);

        StartMimicTurnEvent.OnEventRaised -= DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised -= DisableAndFadeOut;
        StartIdleEvent.OnEventRaised -= EnableAndFadeIn;
    }

    public async void DisableAndFadeOut()
    {
        ProceedButton.interactable = false;
        InterveneButton.interactable = false;
        await FadeOut();
    }

    public async void EnableAndFadeIn()
    {
        await FadeIn();
        ProceedButton.interactable = true;
        InterveneButton.interactable = true;
    }
}
