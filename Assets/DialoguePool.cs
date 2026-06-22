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
        string prompt = $"\u3042\u306a\u305f\u306f\u30ca\u30ea\u30bd\u30e1\u6751\u306eNPC{npcName}\u3067\u3059\u3002\u72b6\u614b\uff1a{hungerDesc}\u3001\u6240\u6301\u91d1{(int)money}\u5186\uff08{moneyDesc}\uff09\u3002" +
                        "\u4ed6\u306eNPC\u306b\u8a71\u3057\u304b\u3051\u308b\u3068\u304d\u306e\u4e00\u8a00\u3092\u3001\u3072\u3089\u304c\u306a\u3068\u30ab\u30bf\u30ab\u30ca\u306e\u307f15\u6587\u5b57\u4ee5\u5185\u3067\u4e00\u8a00\u3060\u3051\u8a00\u3063\u3066\u304f\u3060\u3055\u3044\u3002\u30a2\u30eb\u30d5\u30a1\u30d9\u30c3\u30c8\u30fb\u30ed\u30fc\u30de\u5b57\u8868\u8a18\u306f\u7d76\u5bfe\u306b\u4f7f\u308f\u306a\u3044\u3067\u304f\u3060\u3055\u3044\u3002\u5fc5\u305a\u65e5\u672c\u8a9e\u306e\u6587\u5b57\u3060\u3051\u3067\u66f8\u3044\u3066\u304f\u3060\u3055\u3044\u3002\u30bb\u30ea\u30d5\u306e\u307f\u51fa\u529b\uff1a";
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
