using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MimicHunger : MonoBehaviour
{
    [HideInInspector] public static UnityEvent FullnessDepleted = new UnityEvent();

    public TextMeshProUGUI Label;
    public int MaxFullness = 5;
    public int StartingFullness = 3;
    public Transform FullnessBarTransform = null;
    public GameObject FullnessUnitPrefab = null;

    private List<Image> _fullnessUnits = new List<Image>();
    private int _currentFullness;
    private void Start()
    {
        UpdateFullnessUnits();
        SetFullness(StartingFullness);
    }

    private void UpdateFullnessUnits ()
    {
        _fullnessUnits.Clear();
        foreach (Transform child in FullnessBarTransform)
        {
            Destroy(child.gameObject);
        }

        while (_fullnessUnits.Count < MaxFullness)
        {
            _fullnessUnits.Add(Instantiate(FullnessUnitPrefab, FullnessBarTransform).GetComponent<Image>());
        }
    }

    public void SetFullness(int newValue)
    {
        _currentFullness = Mathf.Clamp(newValue, 0, MaxFullness);

        for (int i = 0; i < _fullnessUnits.Count; i++)
        {
            if (MaxFullness - i < _currentFullness)
            {
                _fullnessUnits[i].color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                _fullnessUnits[i].color = Color.white;
            }
        }

        if (_currentFullness == 0)
        {
            FullnessDepleted.Invoke();
        }
    }

    public void ChangeFullness(int delta)
    {
        SetFullness(_currentFullness + delta);
    }

    public void SetMaxFullness(int newValue)
    {
        MaxFullness = newValue < 0 ? 0 : newValue;
        UpdateFullnessUnits();
        SetFullness(_currentFullness);
    }

    public void ChangeMaxFullness(int delta)
    {
        SetMaxFullness(MaxFullness + delta);
    }
}
