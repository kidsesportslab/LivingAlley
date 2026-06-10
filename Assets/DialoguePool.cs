using UnityEngine;
using System.Collections.Generic;

public class DialoguePool : MonoBehaviour
{
    public static DialoguePool Instance;
    public int poolTargetSize = 15;
    public int refillThreshold = 5;

    private Queue<string> pool = new Queue<string>();
    private bool isGenerating = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        FillPool();
    }

    void Update()
    {
        if (pool.Count < refillThreshold && !isGenerating)
        {
            FillPool();
        }
    }

    void FillPool()
    {
        if (isGenerating) return;
        isGenerating = true;
        RequestOne();
    }

    void RequestOne()
    {
        string prompt = "あなたは仮想世界の住人です。友達に会ったときの挨拶や雑談を必ず日本語で15文字以内で一言だけ話してください。日本語以外は禁止。セリフのみ出力。";
        OllamaClient.Instance.Generate(prompt, (response) =>
        {
            if (response != null)
            {
                string clean = response.Trim().Replace("「", "").Replace("」", "");
                if (clean.Length > 20) clean = clean.Substring(0, 20);
                if (clean.Length > 0) pool.Enqueue(clean);
            }

            if (pool.Count < poolTargetSize)
            {
                RequestOne();
            }
            else
            {
                isGenerating = false;
            }
        });
    }

    public string GetDialogue()
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        return null;
    }

    public int PoolCount => pool.Count;
}