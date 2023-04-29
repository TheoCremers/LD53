using UnityEngine;
using UnityEngine.UI;

public class RoomShiftButton : Button
{
    #region EventChannels

    public ShiftEventChannel RoomShiftEventChannel;

    #endregion

    public Image ArrowImage;
    private Orientation _facingDirection;
    public int _lineIndex;

    public void SetParameters(Orientation facingDirection, int lineIndex)
    {
        _facingDirection = facingDirection;
        _lineIndex = lineIndex;
    }

    public void SetPosition(Vector3 position)
    {

    }

    public void Configure()
    {
        // orient sprite
        // set onclick event
    }
}
