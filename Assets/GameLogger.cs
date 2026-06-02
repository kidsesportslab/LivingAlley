using UnityEngine;
using System.IO;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance;
    public float secondsPerDay = 60f;

    private int currentDay = 1;
    private float dayTimer = 0f;
    private string logFilePath;

    void Awake()
    {
        Instance = this;
        logFilePath = Path.Combine(Application.persistentDataPath, "history.txt");
        File.WriteAllText(logFilePath, "=== Living Alley 歴史記録 ===\n");
        Debug.Log("ログファイル: " + logFilePath);
    }

    void Update()
    {
        dayTimer += Time.deltaTime;
        if (dayTimer >= secondsPerDay)
        {
            dayTimer = 0f;
            currentDay++;
        }
    }

    public void Log(string npcName, string action)
    {
        string entry = $"[Day {currentDay}] {npcName}: {action}";
        File.AppendAllText(logFilePath, entry + "\n");
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }
}