using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class FoodItem : MonoBehaviour, IRoomOccupant
{
    public SpriteRenderer SpriteRenderer;
    public int FullnessGain = 5;
    public float FadeTime = 0.4f;

    public async Task<bool> OnPlayerEnterRoom(MimicGuy guy)
    {
        ResourceManager.Instance.ChangeMimicFullness(FullnessGain, transform.position);

        AudioManager.PlaySFX(SFXType.Eat);
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
        SpriteRenderer.flipX = !clockwise;
    }
}
