using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RoomManipulation : MonoBehaviour
{
    public RoomShiftButton ShiftButtonPrefab = null;
    public UnityEvent RoomShiftPressedEvent = new UnityEvent();

    public void CreateShiftButton(Vector3 position, Orientation facingDirection)
    {
        Instantiate(ShiftButtonPrefab, position, new Quaternion(), transform);

    }
}
