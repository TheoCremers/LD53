using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Shift Event Channel")]
public class ShiftEventChannel : ScriptableObject
{
    public UnityAction<Orientation, int> OnEventRaised;

    public void RaiseEvent(Orientation direction, int lineIndex)
    {
        if (OnEventRaised != null)
        {
            OnEventRaised.Invoke(direction, lineIndex);
        }
    }
}
