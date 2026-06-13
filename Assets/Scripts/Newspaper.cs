using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Newspaper : MonoBehaviour
{
    [SerializeField] OllamaClient ollamaClient;
    [SerializeField] TextMeshProUGUI articleText;
    [SerializeField] Button generateButton;

    const string HistoryPath = "C:/Users/Administrator/AppData/LocalLow/DefaultCompany/LivingAlley/history.txt";
    const int MaxHistoryLines = 50;

    void Start()
    {
        if (generateButton != null)
            generateButton.onClick.AddListener(GenerateNewspaper);
    }

    public void GenerateNewspaper()
    {
        if (!File.Exists(HistoryPath))
        {
            SetText("history.txtが見つかりません。");
            return;
        }

        string[] lines = File.ReadAllLines(HistoryPath, Encoding.UTF8);
        int start = Mathf.Max(0, lines.Length - MaxHistoryLines);
        string history = string.Join("\n", lines, start, lines.Length - start);

        SetText("生成中...");
        if (generateButton != null) generateButton.interactable = false;

        ollamaClient.Generate(BuildPrompt(history), OnArticleReceived, OnError);
    }

    string BuildPrompt(string history)
    {
        return
            "あなたはナリソメ村の新聞記者です。以下の村の出来事の記録をもとに、日本語で村の新聞記事を書いてください。\n" +
            "記事は300字以内で、見出しと本文で構成してください。事実のみを書き、記号や装飾は使わないでください。\n\n" +
            "【村の記録】\n" + history + "\n\n【新聞記事】\n";
    }

    void OnArticleReceived(string article)
    {
        SetText(article);
        if (generateButton != null) generateButton.interactable = true;
    }

    void OnError(string error)
    {
        SetText("生成失敗: " + error);
        if (generateButton != null) generateButton.interactable = true;
    }

    void SetText(string text)
    {
        if (articleText != null)
            articleText.text = text;
    }
}
