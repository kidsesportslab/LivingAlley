using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour
{
    [SerializeField] string model = "qwen2.5:3b";

    const string Endpoint = "http://localhost:11434/api/generate";

    [Serializable] class RequestBody { public string model; public string prompt; public bool stream; public string keep_alive; }
    [Serializable] class ResponseBody { public string response; }

    public void Generate(string prompt, Action<string> onComplete, Action<string> onError = null)
    {
        StartCoroutine(GenerateRoutine(prompt, onComplete, onError));
    }

    IEnumerator GenerateRoutine(string prompt, Action<string> onComplete, Action<string> onError)
    {
        var json = JsonUtility.ToJson(new RequestBody { model = model, prompt = prompt, stream = false, keep_alive = "30m" });
        var bytes = Encoding.UTF8.GetBytes(json);

        using var req = new UnityWebRequest(Endpoint, "POST");
        req.uploadHandler   = new UploadHandlerRaw(bytes);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        Debug.Log($"[Ollama][送信] {DateTime.Now:HH:mm:ss.fff}");
        yield return req.SendWebRequest();
        Debug.Log($"[Ollama][受信] {DateTime.Now:HH:mm:ss.fff}");

        if (req.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(req.error);
            yield break;
        }

        var res = JsonUtility.FromJson<ResponseBody>(req.downloadHandler.text);
        onComplete?.Invoke(res.response);
    }
}
