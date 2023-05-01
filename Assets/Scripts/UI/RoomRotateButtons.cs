using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomRotateButtons : MonoBehaviour
{
    #region EventChannels

    public RotateEventChannel RoomRotateEventChannel;

    #endregion

    public Button RotateButtonClockwise;
    public Button RotateButtonCounterClockwise;
    public Image ClockwiseArrow;
    public Image CounterClockwiseArrow;

    private Vector2Int _tileIndices;

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Configure(Vector2Int tileIndices)
    {
        _tileIndices = tileIndices;

        RotateButtonClockwise.onClick.AddListener(() => RoomRotateEventChannel.RaiseEvent(_tileIndices, true));
        RotateButtonCounterClockwise.onClick.AddListener(() => RoomRotateEventChannel.RaiseEvent(_tileIndices, false));
        RoomRotateEventChannel.OnEventRaised += (x, y) => DeactivateAndFadeOut();
    }

    public void DeactivateAndHide()
    {
        RotateButtonClockwise.interactable = false;
        RotateButtonCounterClockwise.interactable = false;
        ClockwiseArrow.color = new Color(1, 1, 1, 0);
        CounterClockwiseArrow.color = new Color(1, 1, 1, 0);
    }

    public async void DeactivateAndFadeOut()
    {
        RotateButtonClockwise.interactable = false;
        RotateButtonCounterClockwise.interactable = false;
        CounterClockwiseArrow.DOFade(0f, 0.5f);
        await ClockwiseArrow.DOFade(0f, 0.5f).AsyncWaitForCompletion();
    }

    public async void ActivateAndFadeIn()
    {
        RotateButtonClockwise.interactable = true;
        RotateButtonCounterClockwise.interactable = true;
        CounterClockwiseArrow.DOFade(1f, 0.5f);
        await ClockwiseArrow.DOFade(1f, 0.5f).AsyncWaitForCompletion();
    }
}
