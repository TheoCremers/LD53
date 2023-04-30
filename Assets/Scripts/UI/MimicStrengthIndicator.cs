using UnityEngine;
using TMPro;

public class MimicStrengthIndicator : MonoBehaviour
{
    public int StrengthValue = 1;
    public TextMeshProUGUI StrengthLabel;

    public Sprite SmallerIcon;

    private void OnValidate()
    {
        SetStrength(StrengthValue);
    }

    public void SetStrength(int newValue)
    {
        StrengthValue = newValue;
        if (StrengthLabel != null)
        {
            StrengthLabel.text = StrengthValue.ToString();
        }
    }

    public void ChangeStreng(int change)
    {
        SetStrength(StrengthValue + change);
    }
}
