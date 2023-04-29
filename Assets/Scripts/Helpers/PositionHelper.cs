using UnityEngine;

public static class PositionHelper
{
    //public const float SpriteWidth = 128;
    //public const float SpriteHeight = 64;

    public static Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return GridToWorldPosition((Vector2) gridPosition);
    }

    public static Vector3 GridToWorldPosition(Vector2 gridPosition)
    {
        float isoX = (gridPosition.x - gridPosition.y);// - (spriteWidth * 0.5f);
        float isoY = ((gridPosition.x + gridPosition.y) * 0.5f);// - (spriteHeight * 0.5f);

        return new Vector3(isoX, isoY, isoY);
    }

    public static Orientation ToOrientation(this Vector2Int vector)
    {
        if (vector == Vector2Int.up)
            return Orientation.TopLeft;
        else if (vector == Vector2Int.down)
            return Orientation.DownRight;
        else if (vector == Vector2Int.left)
            return Orientation.DownLeft; 
        else if (vector == Vector2Int.right)
            return Orientation.TopRight;
        else 
            throw new System.ArgumentException();
    }

    public static Vector2Int ToVector(this Orientation direction)   
    {   
        switch (direction)
        {
            case Orientation.TopLeft:
                return Vector2Int.up;
            case Orientation.DownRight:
                return Vector2Int.down;
            case Orientation.DownLeft:
                return Vector2Int.left;
            case Orientation.TopRight:
                return Vector2Int.right;
            default:
                throw new System.ArgumentException();
        }        
    }
}
