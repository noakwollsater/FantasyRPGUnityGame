using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text time_text;

    public static TimeManager Instance;

    public float timeMultiplier = 1f; // 1 real-sekund = 1 spelminut
    public float currentTime; // I minuter
    public int dayCount;
    public int month;
    public int year;

    public int minutesPerDay = 1440; // 24 * 60

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        currentTime += Time.deltaTime * timeMultiplier;

        if (currentTime >= minutesPerDay)
        {
            currentTime = 0;
            dayCount++;
            HandleNewDay();
        }
        setTime();
    }

    public void LoadTimeFromData(GameSaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Ingen spardata att ladda tid från.");
            SetRandomTime();
            return;
        }

        year = data.inGameYear;
        month = Mathf.Clamp(data.inGameMonth, 1, 12);  // 🔒 Säker gräns
        dayCount = Mathf.Clamp(data.inGameDay, 1, 30); // 🔒 Säker gräns
        currentTime = Mathf.Clamp(data.inGameTimeMinutes, 0, minutesPerDay - 1);

        setTime();
    }

    private void setTime()
    {
        string time = GetCurrentTimeString();
        time_text.text = time;
    }

    private void HandleNewDay()
    {
        if (dayCount > 30) // Enkel månadslogik
        {
            dayCount = 1;
            month++;

            if (month > 12)
            {
                month = 1;
                year++;
            }
        }

        // Ex: Här kan du kalla events, spara spel, osv
        Debug.Log($"Ny dag! {GetCurrentDateString()}");
    }

    public string GetCurrentTimeString()
    {
        int hours = Mathf.FloorToInt(currentTime / 60f);
        int minutes = Mathf.FloorToInt(currentTime % 60f);
        return $"{hours:00}:{minutes:00}";
    }

    public string GetCurrentDateString()
    {
        string[] weekdays = { "Söndag", "Måndag", "Tisdag", "Onsdag", "Torsdag", "Fredag", "Lördag" };
        int totalDays = (month - 1) * 30 + dayCount;
        string weekday = weekdays[totalDays % 7];
        return $"{weekday}, Dag {dayCount} - Månad {month} - År {year}";
    }

    public void SetRandomTime()
    {
        // Slumpar en tid mellan 0 och 1440 minuter (24 timmar)
        currentTime = UnityEngine.Random.Range(0, minutesPerDay);
        dayCount = UnityEngine.Random.Range(1, 31); // Slumpar en dag mellan 1 och 30
        month = UnityEngine.Random.Range(1, 13); // Slumpar en månad mellan 1 och 12

        setTime();
    }
}
