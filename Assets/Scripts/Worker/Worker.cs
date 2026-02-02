using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Worker
{
    [Serializable]
    public class Worker
    {
        public string name;
        public WorkerTypeSO workerType;
        public SkillLevel skillLevel;
        public int experience;
        public int dailyWage;
        public Worker(string name, WorkerTypeSO workerType)
        {
            this.name = name;
            this.workerType = workerType;
            skillLevel = SkillLevel.None;
            experience = 0;
            dailyWage = workerType.baseDailyWage;
        }
    }
}
