using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicGuy : MonoBehaviour
{
    public Sprite ForwardFacing;

    public Sprite BackwardFacing;

    public SpriteRenderer SpriteRenderer;

    public Vector2Int GridPosition;

    public Orientation FacingDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSprite()
    {
        switch (FacingDirection) 
        {
            case (Orientation.DownLeft):
                SpriteRenderer.sprite = ForwardFacing;
                SpriteRenderer.flipX = true;
                return;
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
}
