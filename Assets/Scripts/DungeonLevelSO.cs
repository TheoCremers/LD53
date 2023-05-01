using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/DungeonLevel")]
public class DungeonLevelSO : ScriptableObject
{
    public Vector2Int LevelSize;

    public ConversationSO Intro;

    public BGMType BGM;

    // 4 exits
    public int FourWays;

    // 3 exits
    public int ThreeWays;

    // 2 exits on opposite sides
    public int Hallways;

    // 2 exits adjacent
    public int Bends;

    // 1 exit
    public int DeadEnds;

    public int Pizzas;

    public int Gemstones;

    public int Swords;

    public List<MonsterSO> Monsters;
}
