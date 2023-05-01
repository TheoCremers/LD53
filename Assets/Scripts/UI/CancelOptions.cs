using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelOptions : BaseOptionsMenu
{
    #region EventChannels

    public VoidEventChannel ShowShiftButtonsEvent;
    public VoidEventChannel ShowRotateButtonsEvent;
    public VoidEventChannel StartOverlordTurnEvent;
    public RotateEventChannel RoomRotateEventChannel;
    public ShiftEventChannel RoomShiftEventChannel;

    #endregion

    public ButtonWithCost CancelButton;

    protected override void Awake()
    {
        base.Awake();

        CancelButton.onClick.AddListener(StartOverlordTurnEvent.RaiseEvent);

        ShowShiftButtonsEvent.OnEventRaised += EnableAndFadeIn;
        ShowRotateButtonsEvent.OnEventRaised += EnableAndFadeIn;
        StartOverlordTurnEvent.OnEventRaised += DisableAndFadeOut;
        RoomRotateEventChannel.OnEventRaised += (x, y) => DisableAndFadeOut();
        RoomShiftEventChannel.OnEventRaised += (x, y) => DisableAndFadeOut();
    }

    private void OnDestroy()
    {
        CancelButton.onClick.RemoveListener(StartOverlordTurnEvent.RaiseEvent);

        ShowShiftButtonsEvent.OnEventRaised -= EnableAndFadeIn;
        ShowRotateButtonsEvent.OnEventRaised -= EnableAndFadeIn;
        StartOverlordTurnEvent.OnEventRaised -= DisableAndFadeOut;
        RoomRotateEventChannel.OnEventRaised -= (x, y) => DisableAndFadeOut();
        RoomShiftEventChannel.OnEventRaised -= (x, y) => DisableAndFadeOut();
    }

    public async void DisableAndFadeOut()
    {
        CancelButton.interactable = false;
        await FadeOut();
    }

    public async void EnableAndFadeIn()
    {
        await FadeIn();
        CancelButton.interactable = true;
    }
}
