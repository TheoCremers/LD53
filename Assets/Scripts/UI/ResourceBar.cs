using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

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

    private List<Image> _resourceUnits = new List<Image>();
    private int _currentValue;

    private void Start()
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
            if (MaxValue - i < _currentValue)
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
}
