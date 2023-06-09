using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class DungeonRoom : MonoBehaviour
{
    [HideInInspector] public string roomIndex;

    public TextMeshPro DebugText;

    public SpriteRenderer SpriteRenderer;

    public bool DoorTopLeft;

    public bool DoorTopRight;

    public bool DoorBottomRight;

    public bool DoorBottomLeft;

    private SpriteRenderer _doorTopLeft;
    private SpriteRenderer _doorTopRight;
    private SpriteRenderer _doorBottomRight;
    private SpriteRenderer _doorBottomLeft;
    private SpriteRenderer _exitIndicator;

    public Sprite DoorOpenSpriteTopRight;
    public Sprite DoorOpenSpriteTopLeft;
    public Sprite DoorOpenSpriteBottomRight;
    public Sprite DoorOpenSpriteBottomLeft;

    public Sprite DoorClosedSpriteTopRight;
    public Sprite DoorClosedSpriteTopLeft;
    public Sprite DoorClosedSpriteBottomRight;
    public Sprite DoorClosedSpriteBottomLeft;

    public IRoomOccupant Occupant;

    public bool HasExit = false;

    void Awake()
    {
        DebugText.text = string.Empty;

        _doorTopLeft = this.transform.Find("DoorTopLeft").GetComponent<SpriteRenderer>();
        _doorTopRight = this.transform.Find("DoorTopRight").GetComponent<SpriteRenderer>();
        _doorBottomRight = this.transform.Find("DoorBottomRight").GetComponent<SpriteRenderer>();
        _doorBottomLeft = this.transform.Find("DoorBottomLeft").GetComponent<SpriteRenderer>();
        _exitIndicator = this.transform.Find("ExitIndicator").GetComponent<SpriteRenderer>();
        UpdateDoorVisibility();
    }

    public void SetDoors(bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
    {
        DoorTopLeft = topLeft;
        DoorTopRight = topRight;
        DoorBottomRight = bottomRight;
        DoorBottomLeft = bottomLeft;
    }

    public void RotateDoors(bool clockwise)
    {
        bool doorTopLeft = DoorTopLeft;
        if (clockwise)
        {
            DoorTopLeft = DoorBottomLeft;
            DoorBottomLeft = DoorBottomRight;
            DoorBottomRight = DoorTopRight;
            DoorTopRight = doorTopLeft;
        }
        else
        {
            DoorTopLeft = DoorTopRight;
            DoorTopRight = DoorBottomRight;
            DoorBottomRight = DoorBottomLeft;
            DoorBottomLeft = doorTopLeft;
        }
        UpdateDoorVisibility();
    }

    public void UpdateDoorVisibility()
    {
        _doorTopLeft.sprite = DoorTopLeft ? DoorOpenSpriteTopLeft : DoorClosedSpriteTopLeft;
        _doorTopRight.sprite = DoorTopRight ? DoorOpenSpriteTopRight : DoorClosedSpriteTopRight;
        _doorBottomRight.sprite = DoorBottomRight ? DoorOpenSpriteBottomRight : DoorClosedSpriteBottomRight;
        _doorBottomLeft.sprite = DoorBottomLeft ? DoorOpenSpriteBottomLeft : DoorClosedSpriteBottomLeft;
        //_doorTopRight.gameObject.SetActive(DoorTopRight);
        //_doorBottomRight.gameObject.SetActive(DoorBottomRight);
        //_doorBottomLeft.gameObject.SetActive(DoorBottomLeft);
    }

    public void AddExit()
    {
        HasExit = true;
        _exitIndicator.enabled = true;
    }

    // only used for editor debugging
    void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }
        UpdateDoorVisibility();
    }
}
