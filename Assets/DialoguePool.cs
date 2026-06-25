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
        _client = FindFirstObjectByType<OllamaClient>();
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
        string hungerDesc = hunger > 210f ? "\u304b\u306a\u308a\u7a7a\u8179" : hunger > 120f ? "\u5c11\u3057\u7a7a\u8179" : "\u6e80\u8179";
        string moneyDesc  = money  < 10f  ? "\u304a\u91d1\u304c\u307b\u307c\u306a\u3044" : money < 30f ? "\u304a\u91d1\u304c\u5c11\u306a\u3044" : "\u4f59\u88d5\u304c\u3042\u308b";
        string prompt = $"\u3042\u306a\u305f\u306f\u30ca\u30ea\u30bd\u30e1\u6751\u306b\u4f4f\u3080{npcName}\u3067\u3059\u3002{hungerDesc}\u3067\u3001\u6240\u6301\u91d1\u306f{(int)money}\u5186\uff08{moneyDesc}\uff09\u3002\u6751\u4eba\u3068\u81ea\u7136\u306a\u4f1a\u8a71\u3092\u3057\u3066\u3044\u307e\u3059\u3002\u4eca\u306e\u72b6\u614b\u3084\u6c17\u5206\u3092\u53cd\u6620\u3057\u305f\u4f1a\u8a71\u6587\u3092\u4e00\u3064\u3060\u3051\u65e5\u672c\u8a9e\u3067\u8a00\u3063\u3066\u304f\u3060\u3055\u3044\u3002\u5fc5\u305a\u65e5\u672c\u8a9e\u306e\u307f\u3067\u3001\u82f1\u6570\u5b57\u6f22\u5b57\u3082\u4f7f\u3063\u3066\u826f\u3044\u3067\u3059\u3002\u30a2\u30eb\u30d5\u30a1\u30d9\u30c3\u30c8\u3084\u4e2d\u56fd\u8a9e\u306f\u7d76\u5bfe\u4f7f\u308f\u306a\u3044\u3067\u304f\u3060\u3055\u3044\u3002";
        _client.Generate(prompt, (response) =>
        {
            if (response != null)
            {
                string clean = response.Trim().Replace("\u3001", "").Replace("\u3002", "");
                clean = System.Text.RegularExpressions.Regex.Replace(clean, @"[^\u3041-\u309F\u30A0-\u30FF\u4E00-\u9FFF\s]", "");
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
