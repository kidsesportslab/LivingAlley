using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;

public class DialoguePool : MonoBehaviour
{
    public static DialoguePool Instance;
    public int poolTargetSize = 15;
    public int refillThreshold = 5;

    private Queue<string> pool = new Queue<string>();
    private bool isGenerating = false;
    private OllamaClient _client;

    void Awake()
    {
        Instance = this;
        _client = FindObjectOfType<OllamaClient>();
    }

    void Start()
    {
        FillPool("村人", 30f, 0f);
    }

    public string GetDialogue(string npcName, float money, float hunger)
    {
        if (pool.Count < refillThreshold && !isGenerating)
            FillPool(npcName, money, hunger);

        return pool.Count > 0 ? pool.Dequeue() : null;
    }

    void FillPool(string npcName, float money, float hunger)
    {
        if (isGenerating) return;
        isGenerating = true;
        RequestOne(npcName, money, hunger);
    }

    void RequestOne(string npcName, float money, float hunger)
    {
        string hungerDesc = hunger > 7f ? "かなり空腹" : hunger > 4f ? "少し空腹" : "満腹";
        string moneyDesc  = money  < 10f ? "お金がほぼない" : money < 30f ? "お金が少ない" : "余裕がある";
        string prompt = $"あなたは仮想世界の村人「{npcName}」です。状態：{hungerDesc}、所持金{(int)money}円（{moneyDesc}）。" +
                        "他の村人に話しかけるときの一言を、ひらがなとカタカナのみ15文字以内で一言だけ言ってください。日本語以外は禁止。セリフのみ出力。";

        _client.Generate(prompt, (response) =>
        {
            if (response != null)
            {
                string clean = response.Trim().Replace("「", "").Replace("」", "");
                clean = Regex.Replace(clean, @"[^぀-ゟ゠-ヿ･-ﾟa-zA-Z0-9！？!? ]", "");
                if (clean.Length > 20) clean = clean.Substring(0, 20);
                if (clean.Length > 0) pool.Enqueue(clean);
            }

            if (pool.Count < poolTargetSize)
                RequestOne(npcName, money, hunger);
            else
                isGenerating = false;
        });
    }

    public int PoolCount => pool.Count;
}
