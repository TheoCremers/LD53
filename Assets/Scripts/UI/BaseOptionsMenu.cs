using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class BaseOptionsMenu : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private RectTransform _rectTransform;
    private Vector2 _defaultPosition;

    public float DefaultAlpha = 0.8f;
    public float FadeTime = 0.5f;
    public Vector2 FadeOffset = new Vector2(0, -128f);

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _defaultPosition = _rectTransform.anchoredPosition;
    }

    public async Task FadeOut()
    {
        _canvasGroup.DOFade(0f, FadeTime);
        await _rectTransform.DOAnchorPos(_defaultPosition + FadeOffset, FadeTime).AsyncWaitForCompletion();
    }

    public async Task FadeIn()
    {
        _canvasGroup.DOFade(DefaultAlpha, FadeTime);
        await _rectTransform.DOAnchorPos(_defaultPosition, FadeTime).AsyncWaitForCompletion();
    }
}
