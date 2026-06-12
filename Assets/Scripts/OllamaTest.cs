using UnityEngine;

[RequireComponent(typeof(OllamaClient))]
public class OllamaTest : MonoBehaviour
{
    void Start()
    {
        GetComponent<OllamaClient>().Generate(
            "こんにちは",
            response => Debug.Log($"[Ollama] {response}"),
            error   => Debug.LogError($"[Ollama] Error: {error}")
        );
    }
}
