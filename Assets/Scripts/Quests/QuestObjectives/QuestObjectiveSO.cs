using UnityEngine;

[CreateAssetMenu(fileName = "QuestObjectiveSO", menuName = "Scriptable Objects/Quests/QuestObjectiveSO")]
public abstract class QuestObjectiveSO : ScriptableObject
{
    public string description;
    public abstract bool IsComplete();
}
