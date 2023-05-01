using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomShiftButton : MonoBehaviour
{
    #region EventChannels

    public ShiftEventChannel RoomShiftEventChannel;
    public VoidEventChannel StartOverlordTurnEvent;

    #endregion

    private Image _arrowImage;
    private Button _shiftButton;
    private Orientation _facingDirection;
    private int _lineIndex;

    private void Awake()
    {
        _arrowImage = GetComponent<Image>();
        _shiftButton = GetComponent<Button>();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Configure(Orientation facingDirection, int lineIndex)
    {
        _facingDirection = facingDirection;
        _lineIndex = lineIndex;

        _shiftButton.onClick.AddListener(() => RoomShiftEventChannel.RaiseEvent(_facingDirection, _lineIndex));
        RoomShiftEventChannel.OnEventRaised += (x, y) => DeactivateAndFadeOut();
        StartOverlordTurnEvent.OnEventRaised += DeactivateAndFadeOut;

        switch (facingDirection)
        {
            case Orientation.TopLeft:
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case Orientation.TopRight:
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            case Orientation.DownRight:
                transform.localScale = new Vector3(-1, -1, 1);
                break;
            case Orientation.DownLeft:
                transform.localScale = new Vector3(1, -1, 1);
                break;
        }
    }

    public void DeactivateAndHide()
    {
        _shiftButton.interactable = false;
        _arrowImage.color = new Color(1, 1, 1, 0);
    }

    public async void DeactivateAndFadeOut()
    {
        _shiftButton.interactable = false;
        await _arrowImage.DOFade(0f, 0.5f).AsyncWaitForCompletion();
    }

    public async void ActivateAndFadeIn()
    {
        _shiftButton.interactable = true;
        await _arrowImage.DOFade(1f, 0.5f).AsyncWaitForCompletion();
    }
}
