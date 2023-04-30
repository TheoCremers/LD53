using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UI;

public class DungeonFloor : MonoBehaviour
{
    #region EventChannels

    public ShiftEventChannel RoomShiftEventChannel;
    public VoidEventChannel ShowShiftButtonsEvent;
    public VoidEventChannel StartOverlordTurnEvent;

    #endregion

    public DungeonRoom[,] Rooms;

    public DungeonRoom DungeonRoomPrefab;
    public RoomManipulation RoomManipulation;

    public FoodItem PizzaPrefab;
    public FightItem SwordPrefab;
    public ForsakenPowerItem GemPrefab;

    public Monster MonsterPrefab;

    public Vector2Int Size = new Vector2Int(5, 5);

    private Vector3 _roomFadeOffset = new Vector3(0, 0.5f, 0.5f);

    [HideInInspector] public Dungeon Dungeon;

    public void Generate(MimicGuy mimicGuy, DungeonLevelSO level)
    {
        Rooms = new DungeonRoom[Size.x, Size.y];

        ConstructLayout(level);
        GenerateShiftButtons();
        PopulateDungeon(mimicGuy, level);

        RoomShiftEventChannel.OnEventRaised += ShiftRooms;
        ShowShiftButtonsEvent.OnEventRaised += RoomManipulation.ActivateShiftButtons;
    }

    private void OnDestroy()
    {
        RoomShiftEventChannel.OnEventRaised -= ShiftRooms;
        ShowShiftButtonsEvent.OnEventRaised -= RoomManipulation.ActivateShiftButtons;
    }    

    private DungeonRoom GenerateRoom(RoomType type)
    {
        var dungeonRoom = Instantiate(DungeonRoomPrefab, transform);
        int rng;
        switch (type)
        {
            case (RoomType.FourWay):
                dungeonRoom.SetDoors(true, true, true, true);
                break;
            case (RoomType.ThreeWay):
                rng = Random.Range(0, 4);
                switch (rng)
                {
                    case (0):
                        dungeonRoom.SetDoors(false, true, true, true);
                        break;
                    case (1):
                        dungeonRoom.SetDoors(true, false, true, true);
                        break;
                    case (2):
                        dungeonRoom.SetDoors(true, true, false, true);
                        break;
                    case (3):
                        dungeonRoom.SetDoors(true, true, true, false);
                        break;

                }
                break;
            case (RoomType.Hallway):
                rng = Random.Range(0, 2);
                switch (rng)
                {
                    case (0):
                        dungeonRoom.SetDoors(true, false, true, false);
                        break;
                    case (1):
                        dungeonRoom.SetDoors(false, true, false, true);
                        break;
                }
                break;
            case (RoomType.Bend):
                rng = Random.Range(0, 4);
                switch (rng)
                {
                    case (0):
                        dungeonRoom.SetDoors(true, true, false, false);
                        break;
                    case (1):
                        dungeonRoom.SetDoors(false, true, true, false);
                        break;
                    case (2):
                        dungeonRoom.SetDoors(false, false, true, true);
                        break;
                    case (3):
                        dungeonRoom.SetDoors(true, false, false, true);
                        break;
                }
                break;
            case (RoomType.DeadEnd):
                rng = Random.Range(0, 4);
                switch (rng)
                {
                    case (0):
                        dungeonRoom.SetDoors(true, false, false, false);
                        break;
                    case (1):
                        dungeonRoom.SetDoors(false, true, false, false);
                        break;
                    case (2):
                        dungeonRoom.SetDoors(false, false, true, false);
                        break;
                    case (3):
                        dungeonRoom.SetDoors(false, false, false, true);
                        break;
                }
                break;
            default:
                dungeonRoom.SetDoors(Random.value < 0.7f, Random.value < 0.7f, Random.value < 0.7f, Random.value < 0.7f);
                break;

        }

        dungeonRoom.UpdateDoorVisibility();
        return dungeonRoom;        
    }

    private void ConstructLayout(DungeonLevelSO level)
    {
        Size = level.LevelSize;

        // Create "tile" pool
        var tiles = new List<DungeonRoom>();

        // Generate rooms
        for (int i = 0; i < level.FourWays; i++)
        {
            var dungeonRoom = GenerateRoom(RoomType.FourWay);
            tiles.Add(dungeonRoom);
        }
        for (int i = 0; i < level.ThreeWays; i++)
        {
            var dungeonRoom = GenerateRoom(RoomType.ThreeWay);
            tiles.Add(dungeonRoom);
        }
        for (int i = 0; i < level.Bends; i++)
        {
            var dungeonRoom = GenerateRoom(RoomType.Bend);
            tiles.Add(dungeonRoom);
        }
        for (int i = 0; i < level.Hallways; i++)
        {
            var dungeonRoom = GenerateRoom(RoomType.Hallway);
            tiles.Add(dungeonRoom);
        }
        for (int i = 0; i < level.DeadEnds; i++)
        {
            var dungeonRoom = GenerateRoom(RoomType.DeadEnd);
            tiles.Add(dungeonRoom);
        }
        
        var tilesStack = new Stack<DungeonRoom>(tiles);

        // Assign them coords on the floor
        for (int xIndex = 0; xIndex < Size.x; xIndex++) 
        {
            for (int yIndex = 0; yIndex < Size.y; yIndex++)
            {
                Rooms[xIndex, yIndex] = tilesStack.Pop();
                Rooms[xIndex, yIndex].transform.position = PositionHelper.GridToWorldPosition(new Vector2(xIndex, yIndex));
            } 
        }

        //UpdateDebugTextForRooms();
    }

    public bool IsPassagePossible(Vector2Int currentPos, Orientation direction)
    {
        var currentRoom = Rooms[currentPos.x, currentPos.y];

        switch (direction)
        {
            case Orientation.TopLeft:
                return currentRoom.DoorTopLeft && (currentPos.y < Size.y-1) && Rooms[currentPos.x, currentPos.y+1].DoorBottomRight;
            case Orientation.TopRight:
                return currentRoom.DoorTopRight && (currentPos.x < Size.x-1) && Rooms[currentPos.x+1, currentPos.y].DoorBottomLeft;
            case Orientation.DownLeft:
                return currentRoom.DoorBottomLeft && (currentPos.x > 0) && Rooms[currentPos.x-1, currentPos.y].DoorTopRight;
            case Orientation.DownRight:
                return currentRoom.DoorBottomRight && (currentPos.y > 0) && Rooms[currentPos.x, currentPos.y-1].DoorTopLeft;
            default:
                return false;
        }
    }

    private void GenerateShiftButtons()
    {
        for (int i = 0; i < Size.x; i++)
        {
            Vector3 topLeftFacingButtonPosition = PositionHelper.GridToWorldPosition(new Vector2Int(i, -1));
            RoomManipulation.CreateShiftButton(topLeftFacingButtonPosition, Orientation.TopLeft, i);

            Vector3 bottomRightFacingButtonPosition = PositionHelper.GridToWorldPosition(new Vector2Int(i, Size.y));
            RoomManipulation.CreateShiftButton(bottomRightFacingButtonPosition, Orientation.DownRight, i);
        }

        for (int i = 0; i < Size.y; i++)
        {
            Vector3 topRightFacingButtonPosition = PositionHelper.GridToWorldPosition(new Vector2Int(-1, i));
            RoomManipulation.CreateShiftButton(topRightFacingButtonPosition, Orientation.TopRight, i);

            Vector3 bottomLeftFacingButtonPosition = PositionHelper.GridToWorldPosition(new Vector2Int(Size.x, i));
            RoomManipulation.CreateShiftButton(bottomLeftFacingButtonPosition, Orientation.DownLeft, i);
        }

        RoomManipulation.DeactivateShiftButtonsNow();
    }


    private DungeonRoom GenerateRandomRoom(Vector2Int gridPosition)
    {
        var dungeonRoom = Instantiate(DungeonRoomPrefab, transform);
        dungeonRoom.transform.position = PositionHelper.GridToWorldPosition(gridPosition);
        // All doors 70% chance for now
        dungeonRoom.DoorBottomLeft = (Random.value < 0.7f);
        dungeonRoom.DoorBottomRight = (Random.value < 0.7f);
        dungeonRoom.DoorTopLeft = (Random.value < 0.7f);
        dungeonRoom.DoorTopRight = (Random.value < 0.7f);
        dungeonRoom.UpdateDoorVisibility();
        return dungeonRoom;
    }    

    private void PopulateDungeon(MimicGuy mimicGuy, DungeonLevelSO level)
    {
        var startingRoomPosition = PickRandomEdgeRoom();
        var startingRoom = Rooms[startingRoomPosition.x, startingRoomPosition.y];
        // Set Mimic to starting room
        mimicGuy.transform.SetParent(startingRoom.transform, false);
        mimicGuy.GridPosition = startingRoomPosition;
        startingRoom.Occupant = mimicGuy;

        // Add objects
        for (int i = 0; i < level.Pizzas; i++)
        {
            AddOccupantToRandomVacantRoom(PizzaPrefab.gameObject); 
        }
        for (int i = 0; i < level.Gemstones; i++)
        {
            AddOccupantToRandomVacantRoom(GemPrefab.gameObject); 
        }
        for (int i = 0; i < level.Swords; i++)
        {
            AddOccupantToRandomVacantRoom(SwordPrefab.gameObject); 
        }

        // Add mobs
        foreach (var mob in level.Monsters)
        {
            var monsterObj = AddOccupantToRandomVacantRoom(MonsterPrefab.gameObject).GetComponent<Monster>();
            monsterObj.ApplyMonsterSO(mob);
        }
    }

    private GameObject AddOccupantToRandomVacantRoom(GameObject prefab)
    {
        var vacantRoomCoords = GetRandomVacantRoom();
        var instance = Instantiate(prefab, transform);
        //pizza.transform.position = PositionHelper.GridToWorldPosition(vacantRoomCoords);
        instance.transform.SetParent(Rooms[vacantRoomCoords.x, vacantRoomCoords.y].transform, false);
        Rooms[vacantRoomCoords.x, vacantRoomCoords.y].Occupant = instance.GetComponent<IRoomOccupant>();
        return instance;
    }

    private Vector2Int GetRandomVacantRoom(int iterations = 0)
    {
        if (iterations > 100)
        {
            throw new UnityException("too many objects in room");
        }

        // Pick a random room
        var xPos = Random.Range(0, Size.x);
        var yPos = Random.Range(0, Size.y);

        // Check if it is free
        if (Rooms[xPos, yPos].Occupant != null)
        {
            return GetRandomVacantRoom(iterations+1);
        } 
        // If we've tried too many times, just place it. Maybe there's too many objects in this floor.
        else if (iterations > 10) 
        {
            return new Vector2Int(xPos, yPos);
        }
        // Check if adjacent rooms are free
        else if ((xPos == 0 || Rooms[xPos-1, yPos] == null) &&
                (xPos == Size.x-1 || Rooms[xPos+1, yPos] == null) &&
                (yPos == 0 || Rooms[xPos, yPos-1] == null) &&
                (yPos == Size.y-1 || Rooms[xPos, yPos+1] == null))
        {
            return new Vector2Int(xPos, yPos);
        }
        // Else try again
        else
        {
            return GetRandomVacantRoom(iterations+1);
        }        
    }

    private Vector2Int PickRandomEdgeRoom()
    {
        // Pick a random edge room to drop the player in
        var xPos = Random.Range(0, Size.x);
        var yPos = Random.Range(0, Size.y);

        var side = Random.value;
        if (side < 0.25f) 
        {
            return new Vector2Int(0, yPos);
        } 
        else if (side < 0.5f)
        {
            return new Vector2Int(Size.x-1, yPos);
        } 
        else if (side < 0.75f)
        {
            return new Vector2Int(xPos, 0);
        } 
        else 
        {
            return new Vector2Int(xPos, Size.y-1);
        }        
    }

    public async void ShiftRooms(Orientation shiftDirection, int lineIndex)
    {
        switch(shiftDirection)
        {
            case Orientation.TopLeft:
                await ShiftRoomsAlongY(lineIndex, false);
                break;
            case Orientation.TopRight:
                await ShiftRoomsAlongX(lineIndex, false);
                break;
            case Orientation.DownRight:
                await ShiftRoomsAlongY(lineIndex, true);
                break;
            case Orientation.DownLeft:
                await ShiftRoomsAlongX(lineIndex, true);
                break;
        }

        Dungeon?.UpdateMimicGuyFacingDirection(true);

        //UpdateDebugTextForRooms();

        //RoomManipulation.ActivateShiftButtons();

        StartOverlordTurnEvent.RaiseEvent();
    }

    public async Task ShiftRoomsAlongX(int lineIndex, bool backwards)
    {
        List<Transform> roomsToMove = new List<Transform>();

        for (int i = 0; i < Size.x; i++)
        {
            int j = backwards ? Size.x - (1 + i) : i;
            roomsToMove.Add(Rooms[j, lineIndex].transform);
        }

        Vector3 roomOffset = PositionHelper.GridToWorldPosition(new Vector2Int(backwards ? -1 : 1, 0)) ;

        await TweenRooms(roomsToMove, roomOffset);

        var teleportingRoom = Rooms[backwards ? 0 : Size.x - 1, lineIndex];
        for (int i = 0; i < Size.x - 1; i++)
        {
            int j = backwards ? i : Size.x - (1 + i);
            Rooms[j, lineIndex] = Rooms[j + (backwards ? 1 : -1), lineIndex];
        }
        int teleportingRoomNewIndex = backwards ? Size.x - 1 : 0;
        Rooms[teleportingRoomNewIndex, lineIndex] = teleportingRoom;

        for (int i = 0; i < Size.x; i++)
        {
            Rooms[i, lineIndex].Occupant?.OnRoomIdChange(i, lineIndex);
        }

        Dungeon.DetermineNextRoomDirectionIfPlayerIsStuck();
    }

    public async Task ShiftRoomsAlongY(int lineIndex, bool backwards)
    {
        List<Transform> roomsToMove = new List<Transform>();

        for (int i = 0; i < Size.y; i++)
        {
            int j = backwards ? Size.y - (1 + i) : i;
            roomsToMove.Add(Rooms[lineIndex, j].transform);
        }

        Vector3 roomOffset = PositionHelper.GridToWorldPosition(new Vector2Int(0, backwards ? -1 : 1));

        await TweenRooms(roomsToMove, roomOffset);

        var teleportingRoom = Rooms[lineIndex, backwards ? 0 : Size.y - 1];
        for (int i = 0; i < Size.y - 1; i++)
        {
            int j = backwards ? i : Size.y - (1 + i);
            Rooms[lineIndex, j] = Rooms[lineIndex, j + (backwards ? 1 : -1)];
        }
        int teleportingRoomNewIndex = backwards ? Size.y - 1 : 0;
        Rooms[lineIndex, teleportingRoomNewIndex] = teleportingRoom;

        for (int i = 0; i < Size.y; i++)
        {
            Rooms[lineIndex, i].Occupant?.OnRoomIdChange(lineIndex, i);
        }

        Dungeon.DetermineNextRoomDirectionIfPlayerIsStuck();
    }

    private async Task TweenRooms(List<Transform> roomsToMove, Vector3 finalOffset, float tweenTime = 1f)
    {
        Vector3 positionToTeleportFinalRoomTo = new Vector3();
        for (int i = 0; i < roomsToMove.Count; i++)
        {
            var roomTransform = roomsToMove[i];
            if (i == 0) 
            {
                positionToTeleportFinalRoomTo = new Vector3(roomTransform.position.x, roomTransform.position.y, roomTransform.position.z);
            }

            if (i < roomsToMove.Count - 1)
            {
                roomTransform.DOMove(roomTransform.position + finalOffset, tweenTime); //just move it
            }
            else
            {
                await roomTransform.DOMove(roomTransform.position + finalOffset, tweenTime).AsyncWaitForCompletion();
                await TeleportRoom(roomTransform, positionToTeleportFinalRoomTo);
            }
        }
    }

    private async Task TeleportRoom(Transform roomTransform, Vector3 positionToTeleportRoomTo)
    {
        foreach (var renderer in roomTransform.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.DOFade(0f, 0.2f);
        }
        await roomTransform.DOMove(roomTransform.position + _roomFadeOffset, 0.3f).AsyncWaitForCompletion();
        roomTransform.position = positionToTeleportRoomTo + _roomFadeOffset;
        foreach (var renderer in roomTransform.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.DOFade(1f, 0.2f);
        }
        await roomTransform.DOMove(roomTransform.position - _roomFadeOffset, 0.3f).AsyncWaitForCompletion();
    }

    private void UpdateDebugTextForRooms()
    {
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                Rooms[i, j].DebugText.text = $"({i}, {j})";
            }
        }
    }
}
