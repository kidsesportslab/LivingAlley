using UnityEngine;

public class OllamaTest : MonoBehaviour
{
    void Start()
    {
        OllamaClient.Instance.Generate("‚ ‚È‚½‚Í‰¼‘z¢ŠE‚ÌZl‚Å‚·BˆêŒ¾‚Åˆ¥ŽA‚µ‚Ä‚­‚¾‚³‚¢B", (response) =>
        {
            Debug.Log("AI•Ô“š: " + response);
        });
    }
}