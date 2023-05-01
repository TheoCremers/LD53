using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class ButtonWithCost : Button
{
    public int ResourceCost = 1;
    public Image IconImage;
    public TextMeshProUGUI AmountLabel;
    public ResourceBar AssociatedResource;

    private Color _defaultTextColor = Color.black;
    private float _fadeTime = 0.2f;

    protected override void Start()
    {
        base.Start();

        _defaultTextColor = AmountLabel.color;
        UpdateAmountLabel();
    }

    public void SetAmount(int amount)
    {
        ResourceCost = amount;
        UpdateAmountLabel();
    }

    public void UpdateAmountLabel()
    {
        var labelText = string.Empty;
        if (ResourceCost >= 0) { labelText = $"-{ResourceCost}"; }
        else if ( ResourceCost < 0 ) { labelText = $"+{-ResourceCost}"; }
        AmountLabel.text = labelText;
    }

    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (AssociatedResource.CurrentValue >= ResourceCost)
        {
            AssociatedResource.ChangeValue(-ResourceCost);
            base.OnPointerClick(eventData);
        }
        else
        {
            FlashRed();
            AssociatedResource.FlashRed(_fadeTime);
        }
    }

    private async void FlashRed()
    {
        // play SFX?

        //AmountLabel.DOColor(Color.red, _fadeTime);
        await IconImage.DOColor(Color.red, _fadeTime).AsyncWaitForCompletion(); 
        await TimeHelper.WaitForSeconds(0.1f);
        //AmountLabel.DOColor(_defaultTextColor, _fadeTime);
        await IconImage.DOColor(Color.white, _fadeTime).AsyncWaitForCompletion();
    }
}
