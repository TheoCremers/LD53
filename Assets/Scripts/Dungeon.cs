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


    #endregion

    public DungeonFloor Floor;

    public MimicGuy MimicGuy;

    public TurnState TurnState;

    // Start is called before the first frame update
    void Start()
    {
        Floor.Generate(MimicGuy);

        // Populate the dungeon. 

        SetTurnStateMimicGuy();
    }

    public void SetTurnStateIdle()
    {
        TurnState = TurnState.Idle;
        // Display buttons advance or intervene
    }

    public void SetTurnStateMimicGuy()
    {
        TurnState = TurnState.MimicGuy;
        // Hide buttons

        // Invoke event
        StartMimicTurnChannel.RaiseEvent();

        // ---- maybe in update
        // Move MimicGuy to next room
        MovementStep();

        // Resolve room
    }

    private void MovementStep()
    {
        var nextRoomPos = DetermineNextRoom();
        var nextRoom = Floor.Rooms[nextRoomPos.x, nextRoomPos.y];
        Debug.Log($"Moving to {nextRoomPos}");

        TweenMimicGuy(MimicGuy.transform, PositionHelper.GridToWorldPosition(nextRoomPos));
    }

    private void TweenMimicGuy(Transform mimicGuy, Vector3 destination, float tweenTime = 1f)
    {
        mimicGuy.DOMove(destination, tweenTime);
    }

    private Vector2Int DetermineNextRoom()
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
        // TODO this logic.

        // Else pick a new random direction
        var choice = possibleOptions[Random.Range(0, possibleOptions.Count-1)];
        return new Vector2Int(currentGridPosition.x, currentGridPosition.y) + PositionHelper.ToVector(choice);
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
