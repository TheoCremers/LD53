using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlordOptions : BaseOptionsMenu
{
    #region EventChannels

    public VoidEventChannel ShowShiftButtonsEvent;
    public VoidEventChannel StartOverlordTurnEvent;
    public VoidEventChannel StartIdleEvent;

    #endregion

    public ButtonWithCost ShowShiftButton;
    public Button FinishInterventionButton;

    protected override void Awake()
    {
        base.Awake();

        ShowShiftButton.onClick.AddListener(ShowShiftButtonsEvent.RaiseEvent);
        FinishInterventionButton.onClick.AddListener(StartIdleEvent.RaiseEvent);

        ShowShiftButtonsEvent.OnEventRaised += DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised += EnableAndFadeIn;
        StartIdleEvent.OnEventRaised += DisableAndFadeOut;
    }

    private void OnDestroy()
    {
        ShowShiftButton.onClick.RemoveListener(ShowShiftButtonsEvent.RaiseEvent);
        FinishInterventionButton.onClick.RemoveListener(StartOverlordTurnEvent.RaiseEvent);

        ShowShiftButtonsEvent.OnEventRaised -= DisableAndFadeOut;
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
        await FadeIn();
        ShowShiftButton.interactable = true;
        FinishInterventionButton.interactable = true;
    }
}
