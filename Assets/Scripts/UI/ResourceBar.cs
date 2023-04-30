using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class ResourceBar : MonoBehaviour
{
    [HideInInspector] public UnityEvent ResourceHitsZero = new UnityEvent();

    public TextMeshProUGUI LabelReference;
    public int MaxValue = 5;
    public int StartingValue = 3;
    public Transform BarTransform = null;
    public GameObject UnitPrefab = null;
    public float TweenDuration = 0.5f;
    public float InactiveAlpha = 0.5f;

    public Sprite SampleIcon {
        get { return UnitPrefab.GetComponent<Image>().sprite; }
    }

    private List<Image> _resourceUnits = new List<Image>();
    private int _currentValue;
    public int CurrentValue { get { return _currentValue; } }

    private void Awake()
    {
        UpdateResourceUnits();
        SetValue(StartingValue);
    }

    private void UpdateResourceUnits ()
    {
        _resourceUnits.Clear();
        foreach (Transform child in BarTransform)
        {
            Destroy(child.gameObject);
        }

        while (_resourceUnits.Count < MaxValue)
        {
            _resourceUnits.Add(Instantiate(UnitPrefab, BarTransform).GetComponent<Image>());
        }
    }

    public void SetValue(int newValue)
    {
        _currentValue = Mathf.Clamp(newValue, 0, MaxValue);

        for (int i = 0; i < _resourceUnits.Count; i++)
        {
            if (i + 1 > _currentValue)
            {
                _resourceUnits[i].DOFade(InactiveAlpha, TweenDuration);
            }
            else
            {
                _resourceUnits[i].DOFade(1f, TweenDuration);
            }
        }

        if (_currentValue == 0)
        {
            ResourceHitsZero.Invoke();
        }
    }

    public void ChangeValue(int delta)
    {
        SetValue(_currentValue + delta);
    }

    public void SetMaxValue(int newValue)
    {
        MaxValue = newValue < 0 ? 0 : newValue;
        UpdateResourceUnits();
        SetValue(_currentValue);
    }

    public void ChangeMaxValue(int delta)
    {
        SetMaxValue(MaxValue + delta);
    }

    public void SetLabelText(string text)
    {
        LabelReference.text = text;
    }

    public async void FlashRed(float fadeTime)
    {
        foreach(var unit in _resourceUnits)
        {
            unit.DOColor(new Color(1, 0, 0, unit.color.a), fadeTime);
        }
        System.Threading.Thread.Sleep((int) fadeTime * 1000);
        //await Task.Delay((int) (fadeTime * 1000));

        //await Task.Delay(100);
        System.Threading.Thread.Sleep(100);

        foreach (var unit in _resourceUnits)
        {
            unit.DOColor(new Color(1, 1, 1, unit.color.a), fadeTime);
        }
        System.Threading.Thread.Sleep((int) fadeTime * 1000);
        //await Task.Delay((int) (fadeTime * 1000));
    }
}
