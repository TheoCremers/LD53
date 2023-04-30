using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class Dungeon : MonoBehaviour
{
    #region EventChannels
    public VoidEventChannel StartMimicTurnChannel;
    public VoidEventChannel StartOverlordTurnChannel;
    public VoidEventChannel StartIdleChannel;


    #endregion

    public DungeonFloor Floor;

    public MimicGuy MimicGuy;

    public TurnState TurnState;

    // Start is called before the first frame update

    void Start()
    {
        StartMimicTurnChannel.OnEventRaised += SetTurnStateMimicGuy;
        Floor.Generate(MimicGuy);
        Floor.Dungeon = this;

        // Populate the dungeon. 

        //SetTurnStateMimicGuy();
        EndMimicTurn(); // call this once to determine the start facing direction        
        SetTurnStateIdle();
    }

    public void SetTurnStateIdle()
    {
        TurnState = TurnState.Idle;
        StartIdleChannel.RaiseEvent();
        // Display buttons advance or intervene
    }

    public void SetTurnStateMimicGuy()
    {
        TurnState = TurnState.MimicGuy;
        // Hide buttons

        // Invoke event
        //StartMimicTurnChannel.RaiseEvent();

        // ---- maybe in update
        // Move MimicGuy to next room
        MovementStep();

        // Resolve room
    }


    private void EndMimicTurn()
    {
        UpdateMimicGuyFacingDirection();
        SetTurnStateIdle();
    }

    public void UpdateMimicGuyFacingDirection(bool tryCurrentFacingDirectionFirst = false)
    {
        var nextRoomDirection = DetermineNextRoomDirection(tryCurrentFacingDirectionFirst);
        MimicGuy.FacingDirection = nextRoomDirection;
        MimicGuy.UpdateSprite();
    }

    private async void MovementStep()
    {
        // remove previous room occupation
        var previousRoom = Floor.Rooms[MimicGuy.GridPosition.x, MimicGuy.GridPosition.y];
        if ((MimicGuy) previousRoom.Occupant == MimicGuy) { previousRoom.Occupant = null; }

        var nextRoomPos = DetermineNextRoom(MimicGuy.FacingDirection);
        var nextRoom = Floor.Rooms[nextRoomPos.x, nextRoomPos.y];
        Debug.Log($"Moving to {nextRoomPos}");
        MimicGuy.transform.SetParent(Floor.transform, true);
        await TweenMimicGuy(MimicGuy.transform, PositionHelper.GridToWorldPosition(nextRoomPos));

        // handle room interaction
        if (nextRoom.Occupant != null && (MimicGuy)nextRoom.Occupant != MimicGuy)
        {
            nextRoom.Occupant.OnPlayerEnterRoom(MimicGuy); // possibly get kicked out?
        }

        nextRoom.Occupant = MimicGuy;
        MimicGuy.transform.SetParent(nextRoom.transform, true);
        MimicGuy.GridPosition = nextRoomPos;
        EndMimicTurn();
    }

    private async Task TweenMimicGuy(Transform mimicGuy, Vector3 destination, float tweenTime = 1f)
    {
        await mimicGuy.DOMove(destination, tweenTime).AsyncWaitForCompletion();
    }

    private Orientation DetermineNextRoomDirection(bool tryCurrentFacingDirectionFirst = false)
    {
        // Check which adjacent room the character will move to
        var currentGridPosition = MimicGuy.GridPosition;
                
        var possibleOptions = new List<Orientation>();
        if (Floor.IsPassagePossible(currentGridPosition, Orientation.TopLeft)) { possibleOptions.Add(Orientation.TopLeft); }
        if (Floor.IsPassagePossible(currentGridPosition, Orientation.TopRight)) { possibleOptions.Add(Orientation.TopRight); }
        if (Floor.IsPassagePossible(currentGridPosition, Orientation.DownLeft)) { possibleOptions.Add(Orientation.DownLeft); }
        if (Floor.IsPassagePossible(currentGridPosition, Orientation.DownRight)) { possibleOptions.Add(Orientation.DownRight); }

        // If there are no options, do nothing (player has to intervene. if he can't, game over)
        // TODO implement game over
        if (!possibleOptions.Any())
        {
            throw new UnityException("Game over chief");
        }

        // Check if the current facing direction is available. If yes, follow through
        if (tryCurrentFacingDirectionFirst && possibleOptions.Contains(MimicGuy.FacingDirection)) 
        {
            return MimicGuy.FacingDirection;
            //return new Vector2Int(currentGridPosition.x, currentGridPosition.y) + PositionHelper.ToVector(MimicGuy.FacingDirection);
        }

        // Else pick randomly between forward, left or right hand if available
        var sideOptions = possibleOptions.Where(x => Vector2.Dot(PositionHelper.ToVector(x), PositionHelper.ToVector(MimicGuy.FacingDirection)) >= 0).ToList();
        if (sideOptions.Any()) 
        {
            var choice = sideOptions[Random.Range(0, sideOptions.Count)];           
            return choice; 
            //return new Vector2Int(currentGridPosition.x, currentGridPosition.y) + PositionHelper.ToVector(choice);
        } 
        else 
        {
            // turn around, last option
            return PositionHelper.ToOrientation(-PositionHelper.ToVector(MimicGuy.FacingDirection));
            //return new Vector2Int(currentGridPosition.x, currentGridPosition.y) - PositionHelper.ToVector(MimicGuy.FacingDirection);            
        };
    }

    private Vector2Int DetermineNextRoom(Orientation movementDirection)
    {
        var currentGridPosition = MimicGuy.GridPosition;
        return new Vector2Int(currentGridPosition.x, currentGridPosition.y) + PositionHelper.ToVector(movementDirection); 
    }



    public void SetTurnStateOverlord()
    {
        // Hide buttons

        // Invoke event
        StartOverlordTurnChannel.RaiseEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
