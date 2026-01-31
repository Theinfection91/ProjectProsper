using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Quests
{
    public static class QuestTracker
    {
        private static readonly Dictionary<QuestSO, HashSet<int>> _questProgress = new();
        private static readonly HashSet<QuestSO> _completedQuests = new();

        public static void OnTalkedToNPC(string npcID)
        {
            // TODO For each active quest, check if there is a step that involves talking to this NPC's ID
            foreach (var quest in _questProgress.Keys)
            {
                foreach (var step in quest.questSteps)
                {
                    if (!_questProgress[quest].Contains(step.stepIndex))
                    {
                        foreach (var objective in step.objectives)
                        {
                            if (objective is TalkQuestObjectiveSO talkObjective && talkObjective.npcID == npcID)
                            {
                                CompleteStep(quest, step.stepIndex);
                                // TODO Notification of step completion sent to UI that needs to be built
                                if (IsQuestComplete(quest))
                                {
                                    CompleteQuest(quest);
                                    // TODO Notification of quest completion sent to UI that needs to be built
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CompleteStep(QuestSO quest, int stepIndex)
        {
            if (!_questProgress.ContainsKey(quest))
            {
                _questProgress[quest] = new HashSet<int>();
            }
            _questProgress[quest].Add(stepIndex);
        }

        public static void CompleteQuest(QuestSO quest)
        {
            _questProgress.Remove(quest);
            _completedQuests.Add(quest);
        }

        public static bool IsStepComplete(QuestSO quest, int stepIndex)
        {
            return _questProgress.ContainsKey(quest) && _questProgress[quest].Contains(stepIndex);
        }

        public static bool IsQuestComplete(QuestSO quest)
        {
            if (!_questProgress.ContainsKey(quest))
            {
                return false;
            }
            foreach (var step in quest.questSteps)
            {
                if (!_questProgress[quest].Contains(step.stepIndex))
                {
                    return false;
                }
            }
            return true;
        }

        public static void ResetQuest(QuestSO quest)
        {
            _questProgress.Remove(quest);
        }

        public static void ResetAllActiveQuests()
        {
            _questProgress.Clear();
        }
    }
}
