using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlordOptions : BaseOptionsMenu
{
    #region EventChannels

    public VoidEventChannel StartMimicTurnEvent;
    public VoidEventChannel StartOverlordTurnEvent;
    public VoidEventChannel StartIdleEvent;

    #endregion

    public Button StartShiftTilesButton;
    public Button FinishInterventionButton;

    protected override void Start()
    {
        base.Start();

        StartShiftTilesButton.onClick.AddListener(StartMimicTurnEvent.RaiseEvent);
        FinishInterventionButton.onClick.AddListener(StartOverlordTurnEvent.RaiseEvent);

        StartMimicTurnEvent.OnEventRaised += DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised += DisableAndFadeOut;
        StartIdleEvent.OnEventRaised += EnableAndFadeIn;
    }

    private void OnDestroy()
    {
        StartShiftTilesButton.onClick.RemoveListener(StartMimicTurnEvent.RaiseEvent);
        FinishInterventionButton.onClick.RemoveListener(StartOverlordTurnEvent.RaiseEvent);

        StartMimicTurnEvent.OnEventRaised -= DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised -= DisableAndFadeOut;
        StartIdleEvent.OnEventRaised -= EnableAndFadeIn;
    }

    public async void DisableAndFadeOut()
    {
        StartShiftTilesButton.interactable = false;
        FinishInterventionButton.interactable = false;
        await FadeOut();
    }

    public async void EnableAndFadeIn()
    {
        await FadeIn();
        StartShiftTilesButton.interactable = true;
        FinishInterventionButton.interactable = true;
    }
}
