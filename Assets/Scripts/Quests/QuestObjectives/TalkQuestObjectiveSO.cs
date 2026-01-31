using UnityEngine;

[CreateAssetMenu(fileName = "TalkQuestObjectiveSO", menuName = "Scriptable Objects/Quests/QuestObjectives/TalkQuestObjectiveSO")]
public class TalkQuestObjectiveSO : QuestObjectiveSO
{
    public string npcID;
    public override bool IsComplete()
    {
        // Implementation for checking if the player has talked to the specified NPC
        // This is a placeholder and should be replaced with actual game logic
        return false;
    }
}
