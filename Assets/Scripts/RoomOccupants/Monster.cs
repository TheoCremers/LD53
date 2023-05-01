using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Monster : MonoBehaviour, IRoomOccupant
{
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer PowerSprite;
    public TextMeshPro PowerLabel;
    public int PowerLevel = 1;
    public int HungerGain = 1;
    public int AtkPowerGain = 1;

    public const float StandOffGridDistance = 0.2f;
    public const float StandOffStepTime = 0.5f;
    public const float StandOffTime = 0.5f;
    public const float ClashMoveTime = 0.3f;
    public const float ClashResolveTime = 0.5f;
    public const float MonsterFadeTime = 0.5f;

    public void ApplyMonsterSO(MonsterSO monsterSO)
    {
        SpriteRenderer.sprite = monsterSO.Sprite;
        PowerLevel = monsterSO.PowerLevel;
        HungerGain = monsterSO.HungerGain;
        AtkPowerGain = monsterSO.AtkPowerGain;
        SpriteRenderer.flipX = (Random.Range(0, 2) == 0);
        PowerLabel.text = $"{PowerLevel}";
    }

    public void ShowPowerIndicator()
    {
        PowerSprite.DOFade(1f, 0.3f);
        PowerLabel.DOFade(1f, 0.3f);
    }

    public void HidePowerIndicator()
    {
        PowerSprite.DOFade(0f, 0.3f);
        PowerLabel.DOFade(0f, 0.3f);
    }

    public async Task<bool> OnPlayerEnterRoom(MimicGuy guy)
    {
        HidePowerIndicator();

        var gridForwardDirection = PositionHelper.ToVector(guy.FacingDirection);
        var forwardDirection = PositionHelper.GridToWorldPosition(gridForwardDirection);

        // back up both sprites
        guy.transform.DOMove(guy.transform.position - forwardDirection * StandOffGridDistance, StandOffStepTime);
        await transform.DOMove(transform.position + forwardDirection * StandOffGridDistance, StandOffStepTime).AsyncWaitForCompletion();

        // 1000 ms sleep
        await TimeHelper.WaitForSeconds(1f);

        // sprites clash
        guy.transform.DOMove(guy.transform.position + forwardDirection * StandOffGridDistance, ClashMoveTime).SetEase(Ease.InBack);
        await transform.DOMove(transform.position - forwardDirection * StandOffGridDistance, ClashMoveTime).SetEase(Ease.InBack).AsyncWaitForCompletion();

        if (ResourceManager.Instance.MimicStrength >= PowerLevel)
        {
            // monster sprite is pushed back and fades
            transform.DOMove(transform.position + forwardDirection * 0.4f, ClashResolveTime).SetEase(Ease.OutQuint);
            await SpriteRenderer.DOFade(0f, MonsterFadeTime).AsyncWaitForCompletion();
            ResourceManager.Instance.ChangeMimicFullness(HungerGain, guy.transform.position);
            await TimeHelper.WaitForSeconds(0.3f);
            ResourceManager.Instance.ChangeMimicPower(AtkPowerGain, guy.transform.position);
            Destroy(gameObject);
            return true;
        }
        else
        {
            // guy sprite is pushed back to previous tile
            guy.FacingDirection = PositionHelper.ToOrientation(-gridForwardDirection);
            guy.UpdateSprite();
            await guy.transform.DOMove(guy.transform.position - forwardDirection, ClashResolveTime).SetEase(Ease.OutQuint).AsyncWaitForCompletion();
            ResourceManager.Instance.TooWeak(guy.transform.position);
            ShowPowerIndicator();
            return false;
        }
    }

    public void OnRoomIdChange(int x, int y)
    {
        // do nothing
    }

    public void OnRoomRotate(bool clockwise)
    {
        SpriteRenderer.flipX = !clockwise;
    }
}
