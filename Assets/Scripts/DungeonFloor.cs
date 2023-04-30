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

    public Vector2Int Size = new Vector2Int(5, 5);

    private Vector3 _roomFadeOffset = new Vector3(0, 0.5f, 0.5f);

    public void Generate(MimicGuy mimicGuy)
    {
        Rooms = new DungeonRoom[Size.x, Size.y];

        ConstructLayout();
        GenerateShiftButtons();
        PopulateDungeon(mimicGuy);

        RoomShiftEventChannel.OnEventRaised += ShiftRooms;
        ShowShiftButtonsEvent.OnEventRaised += RoomManipulation.ActivateShiftButtons;
    }

    private void OnDestroy()
    {
        RoomShiftEventChannel.OnEventRaised -= ShiftRooms;
        ShowShiftButtonsEvent.OnEventRaised -= RoomManipulation.ActivateShiftButtons;
    }

    private void ConstructLayout()
    {
        for (int xIndex = 0; xIndex < Size.x; xIndex++) 
        {
            for (int yIndex = 0; yIndex < Size.y; yIndex++)
            {
                Rooms[xIndex, yIndex] = GenerateRoom(new Vector2Int(xIndex, yIndex));
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

        RoomManipulation.DeactivateShiftButtons();
    }

    private DungeonRoom GenerateRoom(Vector2Int gridPosition)
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

    private void PopulateDungeon(MimicGuy mimicGuy)
    {
        var startingRoom = PickRandomEdgeRoom();
        // Set Mimic to starting room
        mimicGuy.transform.SetParent(Rooms[startingRoom.x, startingRoom.y].transform, false);
        mimicGuy.GridPosition = startingRoom;
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
        Rooms[backwards ? Size.x - 1 : 0, lineIndex] = teleportingRoom;

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
        Rooms[lineIndex, backwards ? Size.y - 1 : 0] = teleportingRoom;
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
