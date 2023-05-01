using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class FightItem : MonoBehaviour, IRoomOccupant
{
    public SpriteRenderer SpriteRenderer;
    public int FightPowerGain = 1;
    public float FadeTime = 0.4f;

    public async Task<bool> OnPlayerEnterRoom(MimicGuy guy)
    {
        ResourceManager.Instance.ChangeMimicPower(FightPowerGain, transform.position);

        await SpriteRenderer.DOFade(0f, FadeTime).AsyncWaitForCompletion();

        Destroy(gameObject);

        return true;
    }

    public void OnRoomIdChange(int x, int y)
    {
        // nothing
    }

    public void OnRoomRotate(bool clockwise)
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
    }
}
