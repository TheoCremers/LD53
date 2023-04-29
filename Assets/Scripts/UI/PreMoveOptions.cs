using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PreMoveOptions : MonoBehaviour
{
    #region EventChannels

    public VoidEventChannel StartMimicTurnEvent;
    public VoidEventChannel StartOverlordTurnEvent;
    public VoidEventChannel StartIdleEvent;

    #endregion

    private CanvasGroup _canvasGroup;

    public Button ProceedButton;
    public Button InterveneButton;

    private RectTransform _proceedButtonRectTransform;
    private RectTransform _interveneButtonRectTransform;

    public float DefaultAlpha = 0.8f;
    public float FadeTime = 0.5f;
    public float DefaultButtonOffsetY = -128f;
    public float FadeOffsetY = -256f;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _proceedButtonRectTransform = ProceedButton.GetComponent<RectTransform>();
        _interveneButtonRectTransform = InterveneButton.GetComponent<RectTransform>();

        ProceedButton.onClick.AddListener(StartMimicTurnEvent.RaiseEvent);
        InterveneButton.onClick.AddListener(StartOverlordTurnEvent.RaiseEvent);

        StartMimicTurnEvent.OnEventRaised += DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised += DisableAndFadeOut;
        StartIdleEvent.OnEventRaised += EnableAndFadeIn;
    }

    private void OnDestroy()
    {
        ProceedButton.onClick.RemoveListener(StartMimicTurnEvent.RaiseEvent);
        InterveneButton.onClick.RemoveListener(StartOverlordTurnEvent.RaiseEvent);

        StartMimicTurnEvent.OnEventRaised -= DisableAndFadeOut;
        StartOverlordTurnEvent.OnEventRaised -= DisableAndFadeOut;
        StartIdleEvent.OnEventRaised -= EnableAndFadeIn;
    }

    public async void DisableAndFadeOut()
    {
        ProceedButton.interactable = false;
        InterveneButton.interactable = false;
        _canvasGroup.DOFade(0f, FadeTime);
        _proceedButtonRectTransform.DOAnchorPos3DY(FadeOffsetY, FadeTime);
        await _interveneButtonRectTransform.DOAnchorPos3DY(FadeOffsetY, FadeTime).AsyncWaitForCompletion();
    }

    public async void EnableAndFadeIn()
    {
        _canvasGroup.DOFade(DefaultAlpha, FadeTime);
        _proceedButtonRectTransform.DOAnchorPos3DY(DefaultButtonOffsetY, FadeTime);
        await _interveneButtonRectTransform.DOAnchorPos3DY(DefaultButtonOffsetY, FadeTime).AsyncWaitForCompletion();
        ProceedButton.interactable = true;
        InterveneButton.interactable = true;
    }
}
