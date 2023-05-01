using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MimicGuy : MonoBehaviour, IRoomOccupant
{
    public Sprite ForwardFacing;

    public Sprite BackwardFacing;

    public SpriteRenderer SpriteRenderer;

    public Vector2Int GridPosition;

    public Orientation FacingDirection;

    public void UpdateSprite()
    {
        switch (FacingDirection) 
        {
            case (Orientation.DownLeft):
                SpriteRenderer.sprite = ForwardFacing;
                SpriteRenderer.flipX = true;
                break;
            case (Orientation.DownRight):
                SpriteRenderer.sprite = ForwardFacing;
                SpriteRenderer.flipX = false;
                break;
            case (Orientation.TopLeft):
                SpriteRenderer.sprite = BackwardFacing;
                SpriteRenderer.flipX = true;
                break;
            case (Orientation.TopRight):
                SpriteRenderer.sprite = BackwardFacing;
                SpriteRenderer.flipX = false;
                break;
        }
    }

    public async Task<bool> OnPlayerEnterRoom(MimicGuy guy)
    {
        Debug.LogWarning("Player entered their own room???");
        await TimeHelper.WaitForSeconds(0.1f);
        return true;
    }

    public void OnRoomIdChange(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }

    public void OnRoomRotate(bool clockwise)
    {
        switch(FacingDirection)
        {
            case (Orientation.DownLeft):
                FacingDirection = clockwise ? Orientation.TopLeft : Orientation.DownRight; 
                break;
            case (Orientation.DownRight):
                FacingDirection = clockwise ? Orientation.DownLeft : Orientation.TopRight;
                break;
            case (Orientation.TopLeft):
                FacingDirection = clockwise ? Orientation.TopRight : Orientation.DownLeft;
                break;
            case (Orientation.TopRight):
                FacingDirection = clockwise ? Orientation.DownRight : Orientation.TopLeft;
                break;
        }
        UpdateSprite();
    }
}
