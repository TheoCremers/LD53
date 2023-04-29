using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Check which adjacent room the character will move to
        var currentGridPosition = MimicGuy.GridPosition;
                
        Debug.Log("topleft: " + Floor.IsPassagePossible(currentGridPosition, Orientation.TopLeft));
        Debug.Log("topright: " + Floor.IsPassagePossible(currentGridPosition, Orientation.TopRight));
        Debug.Log("botleft: " + Floor.IsPassagePossible(currentGridPosition, Orientation.DownLeft));
        Debug.Log("botright: " + Floor.IsPassagePossible(currentGridPosition, Orientation.DownRight));

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
