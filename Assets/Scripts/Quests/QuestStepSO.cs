using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestStep", menuName = "Scriptable Objects/Quests/QuestStepSO")]
public class QuestStepSO : ScriptableObject
{
    public int stepIndex;
    [TextArea(2, 3)] public string description;
    public List<QuestObjectiveSO> objectives = new();

    // TODO Look into adding empty list of new scriptable object to dictate the objectives for this step (QuestObjectiveSO maybe?)
}
