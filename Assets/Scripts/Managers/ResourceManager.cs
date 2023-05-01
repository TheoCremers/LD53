using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public ResourceBar ForsakenPowerResource;
    public ResourceBar MimicFullnessResource;
    public MimicStrengthIndicator MimicStrengthIndicator;

    public ResourceChangeIndicator ResourceChangePrefab;

    public int ForsakenPower {
        get { return ForsakenPowerResource.CurrentValue; }
        set { ForsakenPowerResource.SetValue(value); }
    }

    public int MimicFullness {
        get { return MimicFullnessResource.CurrentValue; }
        set { MimicFullnessResource.SetValue(value); }
    }

    public int MimicStrength {
        get { return MimicStrengthIndicator.StrengthValue; }
        set { MimicStrengthIndicator.SetStrength(value); }
    }

    public int ShiftCost { get; set; } = 1;
    public int RotateCost { get; set; } = 2;

    public void ChangeMimicFullness(int change, Vector3 changeLocation)
    {
        MimicFullnessResource.ChangeValue(change);
        var newChangeIndicator = Instantiate(ResourceChangePrefab, changeLocation, Quaternion.identity);
        newChangeIndicator.SetChangeAmount(change);
        newChangeIndicator.SetImageSprite(MimicFullnessResource.SampleIcon);
    }

    public void RestockResources(int mimicStrength = 0)
    {
        MimicFullness = MimicFullnessResource.StartingValue;
        ForsakenPower = ForsakenPowerResource.StartingValue;
        if (mimicStrength != 0)
        {
            MimicStrength = mimicStrength;
        }        
    }

    public void ChangeMimicFullness(int change)
    {
        Vector3 changeLocation = Vector3.zero;
        ChangeMimicFullness(change, changeLocation);
    }

    public void ChangeForsakenPower(int change, Vector3 changeLocation)
    {
        ForsakenPowerResource.ChangeValue(change);
        var newChangeIndicator = Instantiate(ResourceChangePrefab, changeLocation, Quaternion.identity);
        newChangeIndicator.SetChangeAmount(change);
        newChangeIndicator.SetImageSprite(ForsakenPowerResource.SampleIcon);
    }

    public void ChangeForsakenPower(int change)
    {
        Vector3 changeLocation = Vector3.zero;
        ChangeForsakenPower(change, changeLocation);
    }

    public void ChangeMimicPower(int change, Vector3 changeLocation)
    {
        MimicStrengthIndicator.ChangeStreng(change);
        var newChangeIndicator = Instantiate(ResourceChangePrefab, changeLocation, Quaternion.identity);
        newChangeIndicator.SetChangeAmount(change);
        newChangeIndicator.SetImageSprite(MimicStrengthIndicator.SmallerIcon);
    }

    public void TooWeak(Vector3 location)
    {
        var newChangeIndicator = Instantiate(ResourceChangePrefab, location, Quaternion.identity);
        newChangeIndicator.SetLabelText("Too weak!", Color.red, 2f);
        newChangeIndicator.SetImageSprite(MimicStrengthIndicator.SmallerIcon);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
