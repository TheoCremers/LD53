using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonFloor : MonoBehaviour
{
    private Dictionary<Vector2Int, DungeonRoom> _rooms = new Dictionary<Vector2Int, DungeonRoom>();

    public DungeonRoom DungeonRoomPrefab;

    public Vector2Int Size = new Vector2Int(5, 5);

    // Start is called before the first frame update
    void Start()
    {
        ConstructLayout(); 
        PopulateDungeon();    
    }

    private void ConstructLayout()
    {
        for (int xIndex = 0; xIndex < Size.x; xIndex++) 
        {
            for (int yIndex = 0; yIndex < Size.y; yIndex++)
            {
                var dungeonRoom = GenerateRoom(new Vector2Int(xIndex, yIndex));
            } 
        }
    }

    private DungeonRoom GenerateRoom(Vector2Int gridPosition)
    {
        var dungeonRoom = Instantiate(DungeonRoomPrefab);
        dungeonRoom.transform.position = CalcWorldPosition(gridPosition);
        return dungeonRoom;
    }

    private Vector3 CalcWorldPosition(Vector2Int gridPosition)
    {
        var spriteWidth = DungeonRoomPrefab.SpriteRenderer.bounds.size.x;
        var spriteHeight = DungeonRoomPrefab.SpriteRenderer.bounds.size.y;

        float isoX = (gridPosition.x - gridPosition.y);// - (spriteWidth * 0.5f);
        float isoY = ((gridPosition.x + gridPosition.y) * 0.5f);// - (spriteHeight * 0.5f);

        return new Vector3(isoX, isoY, isoY);
    }

    private void PopulateDungeon()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
