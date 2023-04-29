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

    #endregion

    private DungeonRoom[,] _rooms;

    public DungeonRoom DungeonRoomPrefab;
    public RoomManipulation RoomManipulation;

    public Vector2Int Size = new Vector2Int(5, 5);

    void Start()
    {
        _rooms = new DungeonRoom[Size.x, Size.y];

        ConstructLayout();
        GenerateShiftButtons();
        PopulateDungeon();

        RoomShiftEventChannel.OnEventRaised += ShiftRooms;
    }

    private void OnDestroy()
    {
        RoomShiftEventChannel.OnEventRaised -= ShiftRooms;
    }

    private void ConstructLayout()
    {
        for (int xIndex = 0; xIndex < Size.x; xIndex++) 
        {
            for (int yIndex = 0; yIndex < Size.y; yIndex++)
            {
                _rooms[xIndex, yIndex] = GenerateRoom(new Vector2Int(xIndex, yIndex));
            } 
        }

        UpdateDebugTextForRooms();
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
    }

    private DungeonRoom GenerateRoom(Vector2Int gridPosition)
    {
        var dungeonRoom = Instantiate(DungeonRoomPrefab);
        dungeonRoom.transform.position = PositionHelper.GridToWorldPosition(gridPosition);
        // All doors 70% chance for now
        dungeonRoom.DoorBottomLeft = (Random.value < 0.7f);
        dungeonRoom.DoorBottomRight = (Random.value < 0.7f);
        dungeonRoom.DoorTopLeft = (Random.value < 0.7f);
        dungeonRoom.DoorTopRight = (Random.value < 0.7f);
        dungeonRoom.UpdateDoorVisibility();
        return dungeonRoom;
    }

    private void PopulateDungeon()
    {

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

        UpdateDebugTextForRooms();

        RoomManipulation.ActivateShiftButtons();
    }

    public async Task ShiftRoomsAlongX(int lineIndex, bool backwards)
    {
        List<Transform> roomsToMove = new List<Transform>();

        for (int i = 0; i < Size.x; i++)
        {
            int j = backwards ? Size.x - (1 + i) : i;
            roomsToMove.Add(_rooms[j, lineIndex].transform);
        }

        Vector3 roomOffset = PositionHelper.GridToWorldPosition(new Vector2Int(backwards ? -1 : 1, 0)) ;

        await TweenRooms(roomsToMove, roomOffset);

        var teleportingRoom = _rooms[backwards ? 0 : Size.x - 1, lineIndex];
        for (int i = 0; i < Size.x - 1; i++)
        {
            int j = backwards ? i : Size.x - (1 + i);
            _rooms[j, lineIndex] = _rooms[j + (backwards ? 1 : -1), lineIndex];
        }
        _rooms[backwards ? Size.x - 1 : 0, lineIndex] = teleportingRoom;

    }

    public async Task ShiftRoomsAlongY(int lineIndex, bool backwards)
    {
        List<Transform> roomsToMove = new List<Transform>();

        for (int i = 0; i < Size.y; i++)
        {
            int j = backwards ? Size.y - (1 + i) : i;
            roomsToMove.Add(_rooms[lineIndex, j].transform);
        }

        Vector3 roomOffset = PositionHelper.GridToWorldPosition(new Vector2Int(0, backwards ? -1 : 1));

        await TweenRooms(roomsToMove, roomOffset);

        var teleportingRoom = _rooms[lineIndex, backwards ? 0 : Size.y - 1];
        for (int i = 0; i < Size.y - 1; i++)
        {
            int j = backwards ? i : Size.y - (1 + i);
            _rooms[lineIndex, j] = _rooms[lineIndex, j + (backwards ? 1 : -1)];
        }
        _rooms[lineIndex, backwards ? Size.y - 1 : 0] = teleportingRoom;
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
                await roomTransform.DOMove(roomTransform.position + finalOffset, tweenTime).AsyncWaitForCompletion(); //after move, teleport to other side
                roomTransform.position = positionToTeleportFinalRoomTo;
            }
        }
    }

    private void UpdateDebugTextForRooms()
    {
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                _rooms[i, j].DebugText.text = $"({i}, {j})";
            }
        }
    }
}
