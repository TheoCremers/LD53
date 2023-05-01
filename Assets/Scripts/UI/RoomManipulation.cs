using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManipulation : MonoBehaviour
{
    public RoomShiftButton ShiftButtonPrefab = null;
    public RoomRotateButtons RotateButtonsPrefab = null;
    private List<RoomShiftButton> _roomShiftButtons = new List<RoomShiftButton>();
    private List<RoomRotateButtons> _roomRotateButtons = new List<RoomRotateButtons>();
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas.worldCamera == null)
        {
            _canvas.worldCamera = Camera.main;
        }
    }

    public void DestroyAllShiftButtons()
    {
        foreach (var button in _roomShiftButtons)
        {
            if (button.gameObject != null)
            {
                Destroy(button.gameObject);
            }
        }
        _roomShiftButtons.Clear();
    }

    public void DestroyAllRotateButtons()
    {
        foreach (var button in _roomRotateButtons)
        {
            if (button.gameObject != null)
            {
                Destroy(button.gameObject);
            }
        }
        _roomRotateButtons.Clear();
    }

    public void CreateShiftButton(Vector3 position, Orientation facingDirection, int lineIndex)
    {
        var newButton = Instantiate(ShiftButtonPrefab, position, Quaternion.identity, transform);
        newButton.Configure(facingDirection, lineIndex);
        _roomShiftButtons.Add(newButton);
    }

    public void CreateRotateButton(Vector3 position, Vector2Int tileIndexes)
    {
        var newButton = Instantiate(RotateButtonsPrefab, position, Quaternion.identity, transform);
        newButton.Configure(tileIndexes);
        _roomRotateButtons.Add(newButton);
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

    public void DeactivateRotateButtonsNow()
    {
        foreach (var button in _roomRotateButtons)
        {
            button.DeactivateAndHide();
        }
    }

    public void DeactivateRotateButtons()
    {
        foreach (var button in _roomRotateButtons)
        {
            button.DeactivateAndFadeOut();
        }
    }

    public void ActivateRotateButtons()
    {
        foreach (var button in _roomRotateButtons)
        {
            button.ActivateAndFadeIn();
        }
    }
}
