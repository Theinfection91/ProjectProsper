using Assets.Scripts.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text dayNameText;
    [SerializeField] private Toggle openToggle;
    [SerializeField] private TMP_Dropdown startHourDropdown;
    [SerializeField] private TMP_Dropdown endHourDropdown;

    private int dayOfWeek; // 0 = Monday, 6 = Sunday
    private Shop currentShop;

    private void Awake()
    {
        // Ensure dropdowns are populated before any external Setup() call after Instantiate
        PopulateHourDropdowns();

        // Register listeners here so they're present after Awake
        if (openToggle != null) openToggle.onValueChanged.AddListener(OnOpenToggleChanged);
        if (startHourDropdown != null) startHourDropdown.onValueChanged.AddListener(OnStartHourChanged);
        if (endHourDropdown != null) endHourDropdown.onValueChanged.AddListener(OnEndHourChanged);
    }

    private void OnDestroy()
    {
        if (openToggle != null) openToggle.onValueChanged.RemoveListener(OnOpenToggleChanged);
        if (startHourDropdown != null) startHourDropdown.onValueChanged.RemoveListener(OnStartHourChanged);
        if (endHourDropdown != null) endHourDropdown.onValueChanged.RemoveListener(OnEndHourChanged);
    }

    public void Setup(int dayOfWeek, Shop shop)
    {
        this.dayOfWeek = dayOfWeek;
        this.currentShop = shop;

        // Set day name
        if (dayNameText != null)
        {
            dayNameText.text = GetDayName(dayOfWeek);
        }

        // Load current schedule from shop; use defaults if missing
        DaySchedule daySchedule = shop?.shopSchedule?.GetDaySchedule(dayOfWeek);
        if (daySchedule == null)
        {
            // Fallback - create a local default schedule (7 -> 17)
            daySchedule = new DaySchedule { isOpen = false, startHour = 7, endHour = 17 };
        }

        // Temporarily disable listeners while we set UI values to avoid extra callbacks if desired.
        // (We remove & re-add to ensure no duplicate handlers.)
        if (startHourDropdown != null) startHourDropdown.onValueChanged.RemoveListener(OnStartHourChanged);
        if (endHourDropdown != null) endHourDropdown.onValueChanged.RemoveListener(OnEndHourChanged);

        // Ensure hours are inside valid range and set dropdown indices
        if (startHourDropdown != null)
        {
            int startIndex = Mathf.Clamp(daySchedule.startHour, 0, 23);
            startHourDropdown.value = startIndex;
            startHourDropdown.RefreshShownValue();
        }

        if (endHourDropdown != null)
        {
            int endIndex = Mathf.Clamp(daySchedule.endHour, 0, 23);
            endHourDropdown.value = endIndex;
            endHourDropdown.RefreshShownValue();
        }

        // Re-register the listeners
        if (startHourDropdown != null) startHourDropdown.onValueChanged.AddListener(OnStartHourChanged);
        if (endHourDropdown != null) endHourDropdown.onValueChanged.AddListener(OnEndHourChanged);

        // Set toggle last so it can enable/disable the dropdowns correctly
        if (openToggle != null)
        {
            openToggle.isOn = daySchedule.isOpen;
            startHourDropdown.interactable = daySchedule.isOpen;
            endHourDropdown.interactable = daySchedule.isOpen;
        }
    }

    private void PopulateHourDropdowns()
    {
        if (startHourDropdown == null || endHourDropdown == null) return;

        startHourDropdown.ClearOptions();
        endHourDropdown.ClearOptions();

        var startOptions = new System.Collections.Generic.List<TMP_Dropdown.OptionData>(24);
        var endOptions = new System.Collections.Generic.List<TMP_Dropdown.OptionData>(24);

        for (int hour = 0; hour < 24; hour++)
        {
            string displayText = FormatHour(hour);
            startOptions.Add(new TMP_Dropdown.OptionData(displayText));
            endOptions.Add(new TMP_Dropdown.OptionData(displayText));
        }

        startHourDropdown.AddOptions(startOptions);
        endHourDropdown.AddOptions(endOptions);

        startHourDropdown.RefreshShownValue();
        endHourDropdown.RefreshShownValue();
    }

    private string FormatHour(int hour)
    {
        int hour12 = hour % 12;
        if (hour12 == 0) hour12 = 12;
        string ampm = hour < 12 ? "AM" : "PM";
        return $"{hour12:D2}:00 {ampm}";
    }

    private string GetDayName(int dayOfWeek)
    {
        return dayOfWeek switch
        {
            0 => "Monday",
            1 => "Tuesday",
            2 => "Wednesday",
            3 => "Thursday",
            4 => "Friday",
            5 => "Saturday",
            6 => "Sunday",
            _ => "Unknown"
        };
    }

    private void OnOpenToggleChanged(bool isOpen)
    {
        if (currentShop == null) return;

        DaySchedule daySchedule = currentShop.shopSchedule.GetDaySchedule(dayOfWeek);
        if (daySchedule != null)
        {
            daySchedule.isOpen = isOpen;
            if (startHourDropdown != null) startHourDropdown.interactable = isOpen;
            if (endHourDropdown != null) endHourDropdown.interactable = isOpen;
            Debug.Log($"{GetDayName(dayOfWeek)} set to {(isOpen ? "Open" : "Closed")}");
        }
    }

    private void OnStartHourChanged(int hourIndex)
    {
        if (currentShop == null) return;

        DaySchedule daySchedule = currentShop.shopSchedule.GetDaySchedule(dayOfWeek);
        if (daySchedule != null)
        {
            daySchedule.startHour = Mathf.Clamp(hourIndex, 0, 23);
            Debug.Log($"{GetDayName(dayOfWeek)} start hour set to {FormatHour(daySchedule.startHour)}");
        }
    }

    private void OnEndHourChanged(int hourIndex)
    {
        if (currentShop == null) return;

        DaySchedule daySchedule = currentShop.shopSchedule.GetDaySchedule(dayOfWeek);
        if (daySchedule != null)
        {
            daySchedule.endHour = Mathf.Clamp(hourIndex, 0, 23);
            Debug.Log($"{GetDayName(dayOfWeek)} end hour set to {FormatHour(daySchedule.endHour)}");
        }
    }
}
