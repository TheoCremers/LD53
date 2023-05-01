using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class ButtonWithDoubleCost : Button
{
    public int ResourceCost = 1;
    public Image IconImage;
    public TextMeshProUGUI AmountLabel;
    public ResourceBar AssociatedResource;

    public int ResourceCost2 = 1;
    public Image IconImage2;
    public TextMeshProUGUI AmountLabel2;
    public ResourceBar AssociatedResource2;

    private Color _defaultTextColor = Color.black;
    private float _fadeTime = 0.2f;

    protected override void Start()
    {
        base.Start();

        _defaultTextColor = AmountLabel.color;
        UpdateAmountLabels();
    }

    public void UpdateAmountLabels()
    {
        var labelText = string.Empty;
        if (ResourceCost >= 0) { labelText = $"-{ResourceCost}"; }
        else if ( ResourceCost < 0 ) { labelText = $"+{-ResourceCost}"; }
        AmountLabel.text = labelText;

        var labelText2 = string.Empty;
        if (ResourceCost2 >= 0) { labelText2 = $"-{ResourceCost2}"; }
        else if (ResourceCost2 < 0) { labelText2 = $"+{-ResourceCost2}"; }
        AmountLabel2.text = labelText2;
    }

    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (AssociatedResource.CurrentValue < ResourceCost)
        {
            FlashRed();
            AssociatedResource.FlashRed(_fadeTime);
        }
        else if (AssociatedResource2.CurrentValue < ResourceCost2)
        {
            FlashRed2();
            AssociatedResource2.FlashRed(_fadeTime);
        }
        else
        {
            AssociatedResource.ChangeValue(-ResourceCost);
            AssociatedResource2.ChangeValue(-ResourceCost2);
            base.OnPointerClick(eventData);
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

    private async void FlashRed2()
    {
        // play SFX?

        //AmountLabel.DOColor(Color.red, _fadeTime);
        await IconImage2.DOColor(Color.red, _fadeTime).AsyncWaitForCompletion();
        await TimeHelper.WaitForSeconds(0.1f);
        //AmountLabel.DOColor(_defaultTextColor, _fadeTime);
        await IconImage2.DOColor(Color.white, _fadeTime).AsyncWaitForCompletion();
    }
}
