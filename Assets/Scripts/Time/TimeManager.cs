using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public float dayLength = 1440f; // Length of a day in seconds
    [Range(0f, 1f)] public float currentTimeOfDay = 0.25f; // 0 = Midnight, 1 = End of Day - Start at 6 AM
    public float totalTimePassed;

    public event System.Action OnEndOfDay;
    public int dayCount = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsGamePaused)
            PassTime();
    }

    public void PassTime()
    {
        totalTimePassed += Time.deltaTime;
        currentTimeOfDay += Time.deltaTime / dayLength;
        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f;
            EndOfDay();
            Debug.Log($"New day! Day: {dayCount}");
        }
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

        string formattedTime = $"Day {TimeManager.Instance.dayCount} - {hour12:D2}:{minute:D2} {ampm}";
        return formattedTime;
    }

    public void EndOfDay()
    {
        OnEndOfDay?.Invoke();
        dayCount++;
    }
}
