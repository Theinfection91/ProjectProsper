using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Shop
{
    [Serializable]
    public class DaySchedule
    {
        public bool isOpen;
        public int startHour;
        public int endHour;

        public DaySchedule()
        {
            isOpen = false;
            startHour = 7;
            endHour = 17;
        }

        public int GetStartTimeInMinutes()
        {
            return startHour * 60;
        }

        public int GetEndTimeInMinutes()
        {
            return endHour * 60;
        }

        public bool IsWithinOperatingHours(int currentHour)
        {
            if (!isOpen)
            {
                return false;
            }
            return currentHour >= startHour && currentHour < endHour;
        }
    }
}
