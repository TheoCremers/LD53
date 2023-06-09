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


    public ConversationSO GameOverConvo;

    public ConversationSO CreditsConvo;

    public MimicGuy MimicGuy;

    public TurnState TurnState;

    public List<DungeonLevelSO> Levels;

    public int CurrentLevel = 0;

    public int StartFloorStrength = 1;

    private bool _playerIsStuck = false;

    private void Awake()
    {
        StartMimicTurnChannel.OnEventRaised += SetTurnStateMimicGuy;
        StartIdleChannel.OnEventRaised += () => UpdateMimicGuyFacingDirection(true);
    }

    private void Start()
    {
        Floor.Generate(MimicGuy, Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)]);
        Floor.Dungeon = this;

        //SetTurnStateMimicGuy();
        StartGame();
    }

    public async void StartGame()
    {
        await TimeHelper.WaitForSeconds(1);
        await DialogHelper.ShowConversation(Levels[0].Intro);
        StartNewTurn(); // call this once to determine the start facing direction
    }

    public void SetTurnStateIdle()
    {
        TurnState = TurnState.Idle;
        StartIdleChannel.RaiseEvent();
        // Display buttons advance or intervene
    }

    public async void SetTurnStateMimicGuy()
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
            await TimeHelper.WaitForSeconds(0.3f);
            StartNewTurn();
        }
    }


    private async void StartNewTurn()
    {
        if (ResourceManager.Instance.MimicFullness <= 0)
        {
            int result = await DialogHelper.ShowConversation(GameOverConvo);
            if (result == 1)
            {
                RestartCurrentFloor();
            }
            else
            {
                RestartFromFloor1();
            }
            return;
        }

        //UpdateMimicGuyFacingDirection();
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
        MimicGuy.transform.SetParent(Floor.transform, true);

        AudioManager.PlaySFX(SFXType.Walk);
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

        DOTween.KillAll();
        CurrentLevel++;
        
        Floor.Generate(MimicGuy, Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)]);
        ResourceManager.Instance.RestockResources();
        await TimeHelper.WaitForSeconds(0.1f);
        if (Levels[CurrentLevel].Intro != null)
        {
            await DialogHelper.ShowConversation(Levels[CurrentLevel].Intro);
        }

        if (CurrentLevel < Levels.Count - 1)
        {
            StartNewTurn();
        }
        else
        {
            await TimeHelper.WaitForSeconds(0.5f);
            int result = await DialogHelper.ShowConversation(CreditsConvo);
            if (result == 2)
            {
                ResourceManager.Instance.ShiftCost *= 2;
                ResourceManager.Instance.RotateCost *= 2;
            }
            RestartFromFloor1();
        }
    }

    private async void RestartFromFloor1()
    {
        DOTween.KillAll();
        CurrentLevel = 0;
        StartFloorStrength = 1;
        Floor.Generate(MimicGuy, Levels[Mathf.Clamp(CurrentLevel, 0, Levels.Count - 1)]);
        ResourceManager.Instance.RestockResources(StartFloorStrength);
        await TimeHelper.WaitForSeconds(0.1f);
        StartNewTurn();
    }

    private async void RestartCurrentFloor()
    {
        DOTween.KillAll();
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

        // check if player is stuck
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
