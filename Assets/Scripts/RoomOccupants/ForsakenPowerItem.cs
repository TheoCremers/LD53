using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class ForsakenPowerItem : MonoBehaviour, IRoomOccupant
{
    public SpriteRenderer SpriteRenderer;
    public int ForsakenPowerGained = 3;
    public float FadeTime = 0.4f;

    public async Task<bool> OnPlayerEnterRoom(MimicGuy guy)
    {
        ResourceManager.Instance.ChangeForsakenPower(ForsakenPowerGained, transform.position);

        await SpriteRenderer.DOFade(0f, FadeTime).AsyncWaitForCompletion();

        Destroy(gameObject);

        return true;
    }

    public void OnRoomIdChange(int x, int y)
    {
        // nothing
    }
}
