using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Scriptable Objects/Quests/QuestSO")]
public class QuestSO : ScriptableObject
{
    public string questID;
    public string questName;
    [TextArea] public string description;
    public List<QuestStepSO> questSteps = new();
}
