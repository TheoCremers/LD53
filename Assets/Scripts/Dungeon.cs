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

    public List<DungeonLevelSO> Levels;

    public int CurrentLevel = 0;

    public int StartFloorStrength = 1;

    private bool _playerIsStuck = false;

    private void Awake()
    {
        StartMimicTurnChannel.OnEventRaised += SetTurnStateMimicGuy;
    }

    private async void Start()
    {
        Floor.Generate(MimicGuy, Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)]);
        Floor.Dungeon = this;

        //SetTurnStateMimicGuy();
        StartNewTurn(); // call this once to determine the start facing direction

        await TimeHelper.WaitForSeconds(2);
        await DialogHelper.ShowConversation(Levels[0].Intro);
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
        if (!_playerIsStuck)
        {
            MovementStep();
        }
        else
        {
            StartNewTurn();
        }
    }


    private void StartNewTurn()
    {
        if (ResourceManager.Instance.MimicFullness <= 0)
        {
            // TODO: play some dialog here?
            RestartFromFloor1();
            return;
        }

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
        if (previousRoom.Occupant == (IRoomOccupant) MimicGuy) 
        { 
            previousRoom.Occupant = null; 
        }

        var nextRoomPos = DetermineNextRoom(MimicGuy.FacingDirection);
        var nextRoom = Floor.Rooms[nextRoomPos.x, nextRoomPos.y];
        Debug.Log($"Moving to {nextRoomPos}");
        MimicGuy.transform.SetParent(Floor.transform, true);
        await TweenMimicGuy(MimicGuy.transform, PositionHelper.GridToWorldPosition(nextRoomPos));

        // handle room interaction
        if (nextRoom.Occupant != null && nextRoom.Occupant != (IRoomOccupant) MimicGuy)
        {
            if (!await nextRoom.Occupant.OnPlayerEnterRoom(MimicGuy)) // if true, can enter room, otherwise turn around
            {
                previousRoom.Occupant = MimicGuy;
                MimicGuy.transform.SetParent(previousRoom.transform, true);
                StartNewTurn();
                return;
            }
        }

        nextRoom.Occupant = MimicGuy;
        MimicGuy.transform.SetParent(nextRoom.transform, true);
        MimicGuy.GridPosition = nextRoomPos;

        if (nextRoom.HasExit)
        {
            FinishLevel(nextRoom);
            return;
        }

        StartNewTurn();
    }

    private async void FinishLevel(DungeonRoom exitRoom)
    {
        //move room down for style points
        await Floor.MoveRoomDown(exitRoom, MimicGuy);
        StartFloorStrength = ResourceManager.Instance.MimicStrength;

        CurrentLevel++;
        Floor.Generate(MimicGuy, Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)]);
        ResourceManager.Instance.RestockResources();
        await TimeHelper.WaitForSeconds(0.1f);
        StartNewTurn();

        await DialogHelper.ShowConversation(Levels[CurrentLevel].Intro);
    }

    private async void RestartFromFloor1()
    {
        CurrentLevel = 0;
        StartFloorStrength = 1;
        Floor.Generate(MimicGuy, Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)]);
        ResourceManager.Instance.RestockResources(StartFloorStrength);
        await TimeHelper.WaitForSeconds(0.1f);
        StartNewTurn();
    }

    private async void RestartCurrentFloor()
    {
        Floor.Generate(MimicGuy, Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)]);
        ResourceManager.Instance.RestockResources(StartFloorStrength);
        await TimeHelper.WaitForSeconds(0.1f);
        StartNewTurn();
    }

    private async Task TweenMimicGuy(Transform mimicGuy, Vector3 destination, float tweenTime = 1f)
    {
        await mimicGuy.DOMove(destination, tweenTime).AsyncWaitForCompletion();
    }

    public bool IsCurrentFacingDirectionOpen()
    {
        var currentGridPosition = MimicGuy.GridPosition;

        var possibleOptions = new List<Orientation>();
        if (Floor.IsPassagePossible(currentGridPosition, Orientation.TopLeft)) { possibleOptions.Add(Orientation.TopLeft); }
        if (Floor.IsPassagePossible(currentGridPosition, Orientation.TopRight)) { possibleOptions.Add(Orientation.TopRight); }
        if (Floor.IsPassagePossible(currentGridPosition, Orientation.DownLeft)) { possibleOptions.Add(Orientation.DownLeft); }
        if (Floor.IsPassagePossible(currentGridPosition, Orientation.DownRight)) { possibleOptions.Add(Orientation.DownRight); }

        return possibleOptions.Contains(MimicGuy.FacingDirection);
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
        if (!possibleOptions.Any())
        {
            _playerIsStuck = true;
            return MimicGuy.FacingDirection;
        }
        _playerIsStuck = false;

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

    public void DetermineNextRoomDirectionIfPlayerIsStuck()
    {
        if (_playerIsStuck)
        {
            DetermineNextRoomDirection(true);
        }
    }

    public void SetTurnStateOverlord()
    {
        // Hide buttons

        // Invoke event
        StartOverlordTurnChannel.RaiseEvent();
    }

    private void OverlordStateTutorial()
    {

    }
}
