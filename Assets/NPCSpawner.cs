using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public float spawnRadius = 4f;

    private string[] npcNames = {
        "ケンジ", "ハルト", "ソウタ", "リョウ", "ナオキ",
        "アヤカ", "ミサキ", "ユイ", "ハナ", "サクラ"
    };

    public void SpawnNPC()
    {
        if (npcPrefab == null) return;

        float x = Random.Range(-spawnRadius, spawnRadius);
        float z = Random.Range(-spawnRadius, spawnRadius);
        Vector3 spawnPos = new Vector3(x, 1f, z);

        GameObject newNPC = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
        NPCMover mover = newNPC.GetComponent<NPCMover>();
        if (mover != null)
        {
            mover.npcName = npcNames[Random.Range(0, npcNames.Length)];
        }
    }
}