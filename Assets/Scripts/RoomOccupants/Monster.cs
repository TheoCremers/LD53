using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class Monster : MonoBehaviour, IRoomOccupant
{
    public SpriteRenderer SpriteRenderer;
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
    }

    public async Task<bool> OnPlayerEnterRoom(MimicGuy guy)
    {
        var gridForwardDirection = PositionHelper.ToVector(guy.FacingDirection);
        var forwardDirection = PositionHelper.GridToWorldPosition(gridForwardDirection);

        // back up both sprites
        guy.transform.DOMove(guy.transform.position - forwardDirection * StandOffGridDistance, StandOffStepTime);
        await transform.DOMove(transform.position + forwardDirection * StandOffGridDistance, StandOffStepTime).AsyncWaitForCompletion();

        await Task.Delay((int) (StandOffTime * 1000));

        // sprites clash
        guy.transform.DOMove(guy.transform.position + forwardDirection * StandOffGridDistance, ClashMoveTime).SetEase(Ease.InBack);
        await transform.DOMove(transform.position - forwardDirection * StandOffGridDistance, ClashMoveTime).SetEase(Ease.InBack).AsyncWaitForCompletion();

        if (ResourceManager.Instance.MimicStrength >= PowerLevel)
        {
            // monster sprite is pushed back and fades
            transform.DOMove(transform.position + forwardDirection * 0.4f, ClashResolveTime).SetEase(Ease.OutQuint);
            await SpriteRenderer.DOFade(0f, MonsterFadeTime).AsyncWaitForCompletion();
            ResourceManager.Instance.ChangeMimicFullness(HungerGain, guy.transform.position);
            await Task.Delay(300);
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
            ResourceManager.Instance.TooWeak(guy.transform.position - forwardDirection);
            return false;
        }
    }

    public void OnRoomIdChange(int x, int y)
    {
        // do nothing
    }
}
