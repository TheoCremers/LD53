using UnityEngine;
using TMPro;
using DG.Tweening;

public class ResourceChangeIndicator : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public TextMeshPro DeltaLabel;
    public int ChangeAmount = 1;
    public float FadeOutTime = 1f;
    public Vector3 FadeOutOffset = Vector3.up * 50f;

    private void Start()
    {
        SetChangeAmount(ChangeAmount);
        FadeOutAndDestroy();
    }

    public void SetChangeAmount(int changeAmount)
    {
        ChangeAmount = changeAmount;
        if (ChangeAmount > 0)
        {
            DeltaLabel.text = $"+{ChangeAmount}";
        }
        else
        {
            DeltaLabel.text = $"{ChangeAmount}";
        }
    }

    public void SetImageSprite(Sprite sprite)
    {
        SpriteRenderer.sprite = sprite;
    }

    private async void FadeOutAndDestroy()
    {
        transform.DOMove(transform.position + FadeOutOffset, FadeOutTime);
        SpriteRenderer.DOFade(0f, FadeOutTime);
        await DeltaLabel.DOFade(0f, FadeOutTime).AsyncWaitForCompletion();

        Destroy(this);
    }
}
