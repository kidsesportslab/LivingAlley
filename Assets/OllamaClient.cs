using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class OllamaClient : MonoBehaviour
{
    public static OllamaClient Instance;
    private const string API_URL = "http://localhost:11434/api/generate";
    private const string MODEL = "llama3.2:3b-instruct-q4_K_M";

    void Awake()
    {
        Instance = this;
    }

    public void Generate(string prompt, System.Action<string> onComplete)
    {
        StartCoroutine(GenerateCoroutine(prompt, onComplete));
    }

    IEnumerator GenerateCoroutine(string prompt, System.Action<string> onComplete)
    {
        string json = JsonUtility.ToJson(new OllamaRequest
        {
            model = MODEL,
            prompt = prompt,
            stream = false
        });

        UnityWebRequest req = new UnityWebRequest(API_URL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            OllamaResponse res = JsonUtility.FromJson<OllamaResponse>(req.downloadHandler.text);
            onComplete?.Invoke(res.response);
        }
        else
        {
            Debug.LogError("Ollama API Error: " + req.error);
            onComplete?.Invoke(null);
        }
    }

    [System.Serializable]
    class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
    }

    [System.Serializable]
    class OllamaResponse
    {
        public string response;
    }
}