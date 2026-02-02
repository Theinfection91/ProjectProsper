using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    public float dayLengthInSeconds = 1440f;
    [Range(0f, 1f)] public float currentTimeOfDay = 0.25f;
    public int dayCount = 1;
    public int currentDayOfWeek = 0; // 0 = Monday, 6 = Sunday

    // Time Speed Control
    public enum TimeSpeed { Paused, Normal, Fast, Faster, Ultra }
    public TimeSpeed currentSpeed = TimeSpeed.Normal;

    // Listeners
    public static event Action<int, int> OnMinuteChanged;
    public static event Action<int> OnHourChanged;
    public event Action OnEndOfDay;
    public event Action<int> OnDayOfWeekChanged; // New event for day changes

    // Internal tracking for discrete time changes
    private int lastMinute = -1;
    private int lastHour = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Update()
    {
        // Use your GameManager pause, but also our internal speed
        if (!GameManager.IsGamePaused && currentSpeed != TimeSpeed.Paused)
        {
            PassTime();
        }
    }

    public void PassTime()
    {
        // Apply the speed multiplier
        float speedMultiplier = GetSpeedMultiplier();
        currentTimeOfDay += (Time.deltaTime * speedMultiplier) / dayLengthInSeconds;

        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f;
            EndOfDay();
        }

        CheckForDiscreteTimeChanges();
    }

    private void CheckForDiscreteTimeChanges()
    {
        // Convert the 0-1 float into discrete integers
        float totalHours = currentTimeOfDay * 24f;
        int currentHour = Mathf.FloorToInt(totalHours);
        int currentMinute = Mathf.FloorToInt((totalHours - currentHour) * 60f);

        // Only trigger events if the integer actually changed
        if (currentMinute != lastMinute)
        {
            lastMinute = currentMinute;
            OnMinuteChanged?.Invoke(currentHour, currentMinute);
        }

        if (currentHour != lastHour)
        {
            lastHour = currentHour;
            OnHourChanged?.Invoke(currentHour);
        }
    }

    public float GetSpeedMultiplier()
    {
        return currentSpeed switch
        {
            TimeSpeed.Normal => 1f,
            TimeSpeed.Fast => 5f,   // 5x faster
            TimeSpeed.Faster => 10f, // 10x faster
            TimeSpeed.Ultra => 20f, // 20x faster
            _ => 0f
        };
    }

    public void EndOfDay()
    {
        dayCount++;
        currentDayOfWeek = (currentDayOfWeek + 1) % 7; // Cycle through 0-6
        OnEndOfDay?.Invoke();
        OnDayOfWeekChanged?.Invoke(currentDayOfWeek);
        Debug.Log($"New day! Day: {dayCount}, {GetDayOfWeekName()}");
    }

    public string GetFormattedTime()
    {
        // Convert to hours and minutes
        float totalHours = currentTimeOfDay * 24f;
        int hour24 = Mathf.FloorToInt(totalHours);
        int minute = Mathf.FloorToInt((totalHours - hour24) * 60f);

        // Convert to 12-hour format
        int hour12 = hour24 % 12;
        if (hour12 == 0) hour12 = 12; // Midnight or noon becomes 12

        // Determine AM/PM
        string ampm = hour24 < 12 ? "AM" : "PM";

        string formattedTime = $"{GetDayOfWeekName()}, Day {TimeManager.Instance.dayCount} - {hour12:D2}:{minute:D2} {ampm}";
        return formattedTime;
    }

    public int GetCurrentHour()
    {
        float totalHours = currentTimeOfDay * 24f;
        return Mathf.FloorToInt(totalHours);
    }

    public int GetCurrentMinute()
    {
        float totalHours = currentTimeOfDay * 24f;
        int currentHour = Mathf.FloorToInt(totalHours);
        return Mathf.FloorToInt((totalHours - currentHour) * 60f);
    }

    public string GetDayOfWeekName()
    {
        return currentDayOfWeek switch
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

    public string GetDayOfWeekShortName()
    {
        return currentDayOfWeek switch
        {
            0 => "Mon",
            1 => "Tue",
            2 => "Wed",
            3 => "Thu",
            4 => "Fri",
            5 => "Sat",
            6 => "Sun",
            _ => "?"
        };
    }
}
