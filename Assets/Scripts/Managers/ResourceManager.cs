using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public ResourceBar ForsakenPowerResource;
    public ResourceBar MimicFullnessResource;
    public MimicStrengthIndicator MimicStrengthIndicator;

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

    private void Start()
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
