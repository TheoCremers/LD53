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
}
