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

    private Dictionary<Vector2Int, DungeonRoom> _rooms = new Dictionary<Vector2Int, DungeonRoom>();

    public DungeonRoom DungeonRoomPrefab;
    public RoomManipulation RoomManipulation;

    public Vector2Int Size = new Vector2Int(5, 5);

    void Start()
    {
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
                var dungeonRoom = GenerateRoom(new Vector2Int(xIndex, yIndex));
            } 
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

    public void ShiftRooms(Orientation shiftDirection, int lineIndex)
    {
        switch(shiftDirection)
        {
            case Orientation.TopLeft:
                ShiftRoomsAlongX(lineIndex, false);
                break;
            case Orientation.TopRight:
                ShiftRoomsAlongY(lineIndex, false);
                break;
            case Orientation.DownRight:
                ShiftRoomsAlongX(lineIndex, true);
                break;
            case Orientation.DownLeft:
                ShiftRoomsAlongY(lineIndex, true);
                break;
        }
    }

    public async void ShiftRoomsAlongX(int rowIndex, bool backwards)
    {
        List<Transform> roomsToMove = new List<Transform>();

        for (int i = 0; i < Size.x; i++)
        {
            int j = backwards ? i : Size.x - (1 + i);
            roomsToMove.Add(_rooms[new Vector2Int(j, rowIndex)].transform);
        }

        Vector3 roomOffset = PositionHelper.GridToWorldPosition(new Vector2Int(backwards ? -1 : 1, 0)) ;

        await TweenRooms(roomsToMove, roomOffset);
    }

    public async void ShiftRoomsAlongY(int rowIndex, bool backwards)
    {
        List<Transform> roomsToMove = new List<Transform>();

        for (int i = 0; i < Size.y; i++)
        {
            int j = backwards ? i : Size.y - (1 + i);
            roomsToMove.Add(_rooms[new Vector2Int(rowIndex, j)].transform);
        }

        Vector3 roomOffset = PositionHelper.GridToWorldPosition(new Vector2Int(0, backwards ? -1 : 1));

        await TweenRooms(roomsToMove, roomOffset);
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
}
