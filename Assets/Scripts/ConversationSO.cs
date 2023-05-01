using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Conversation")]
public class ConversationSO : ScriptableObject
{
    public List<DialogSO> Dialogues;
}
