using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    [Serializable]
    public class ShopSchedule
    {
        [SerializeField]
        private DaySchedule[] weeklySchedule = new DaySchedule[7];

        public ShopSchedule()
        {
            for (int i = 0; i < weeklySchedule.Length; i++)
            {
                weeklySchedule[i] = new DaySchedule();
            }
        }

        public DaySchedule GetDaySchedule(int dayOfWeek)
        {
            return weeklySchedule[dayOfWeek];
        }

        public DaySchedule[] GetWeeklySchedule()
        {
            return weeklySchedule;
        }

        public bool IsShopOpenToday(int dayOfWeek)
        {
            DaySchedule today = GetDaySchedule(dayOfWeek);
            return today != null && today.isOpen;
        }

        public bool IsShopOpenNow(int dayOfWeek, int currentHour)
        {
            DaySchedule today = GetDaySchedule(dayOfWeek);
            return today != null && today.IsWithinOperatingHours(currentHour);
        }
    }
}
