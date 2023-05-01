using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Rotate Event Channel")]
public class RotateEventChannel : ScriptableObject
{
    public UnityAction<Vector2Int, bool> OnEventRaised;

    public void RaiseEvent(Vector2Int roomIndices, bool clockwise)
    {
        if (OnEventRaised != null)
        {
            OnEventRaised.Invoke(roomIndices, clockwise);
        }
    }
}
