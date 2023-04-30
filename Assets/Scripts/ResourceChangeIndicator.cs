using UnityEngine;
using TMPro;
using DG.Tweening;

public class ResourceChangeIndicator : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public TextMeshPro DeltaLabel;
    public int ChangeAmount = 1;
    public float FadeOutTime = 1f;
    public Vector3 FadeOutOffset = Vector3.up * 1f;

    private void Start()
    {
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

    public void SetLabelText(string text, Color color, float fontSize)
    {
        DeltaLabel.text = text;
        DeltaLabel.color = color;
        DeltaLabel.fontSize = fontSize;
    }

    public void SetImageSprite(Sprite sprite)
    {
        SpriteRenderer.sprite = sprite;
    }

    private async void FadeOutAndDestroy()
    {
        transform.DOMove(transform.position + FadeOutOffset, FadeOutTime).SetEase(Ease.InOutCubic);
        DeltaLabel.DOFade(0f, FadeOutTime).SetEase(Ease.InQuint);
        await SpriteRenderer.DOFade(0f, FadeOutTime).SetEase(Ease.InQuint).AsyncWaitForCompletion();
        Destroy(gameObject);
    }
}
