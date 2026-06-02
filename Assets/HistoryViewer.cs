using UnityEngine;
using TMPro;
using System.IO;

public class HistoryViewer : MonoBehaviour
{
    public TextMeshProUGUI historyText;
    public float refreshInterval = 5f;

    private float timer = 0f;
    private string logFilePath;

    void Start()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "history.txt");
        RefreshHistory();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= refreshInterval)
        {
            timer = 0f;
            RefreshHistory();
        }
    }

    void RefreshHistory()
    {
        if (!File.Exists(logFilePath)) return;
        string content = File.ReadAllText(logFilePath);
        if (historyText != null)
            historyText.text = content;
    }
}