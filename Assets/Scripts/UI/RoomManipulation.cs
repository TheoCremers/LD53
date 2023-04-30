using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManipulation : MonoBehaviour
{
    public RoomShiftButton ShiftButtonPrefab = null;
    private List<RoomShiftButton> _roomShiftButtons = new List<RoomShiftButton>();
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas.worldCamera == null)
        {
            _canvas.worldCamera = Camera.main;
        }
    }

    public void CreateShiftButton(Vector3 position, Orientation facingDirection, int lineIndex)
    {
        var newButton = Instantiate(ShiftButtonPrefab, position, new Quaternion(), transform);
        newButton.Configure(facingDirection, lineIndex);
        _roomShiftButtons.Add(newButton);
    }

    public void DeactivateShiftButtonsNow()
    {
        foreach (RoomShiftButton button in _roomShiftButtons)
        {
            button.DeactivateAndHide();
        }
    }

    public void DeactivateShiftButtons()
    {
        foreach(RoomShiftButton button in _roomShiftButtons)
        {
            button.DeactivateAndFadeOut();
        }
    }

    public void ActivateShiftButtons()
    {
        foreach (RoomShiftButton button in _roomShiftButtons)
        {
            button.ActivateAndFadeIn();
        }
    }
}
