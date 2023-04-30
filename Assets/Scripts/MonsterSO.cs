using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/Monster")]
public class MonsterSO : ScriptableObject
{    
    public int PowerLevel;

    // Not sure if we want to change these ever
    public int HungerGain = 3;
    public int AtkPowerGain = 1;

    public Sprite Sprite;    
}
