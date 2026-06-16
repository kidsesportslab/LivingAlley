using UnityEngine;
using System.Collections;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcBasePrefab;
    public GameObject[] visualPrefabs;
    public float spawnRadius = 4f;
    public GameObject[] foods;
public GameObject bed;

public int initialSpawnCount = 3;

void Start()
{
    for (int i = 0; i < initialSpawnCount; i++)
    {
        SpawnNPC();
    }
}

    private string[] npcNames = {
        "ケンジ", "ハルト", "ソウタ", "リョウ", "ナオキ",
        "アヤカ", "ミサキ", "ユイ", "ハナ", "サクラ"
    };

    public void SpawnNPC()
    {
        if (npcBasePrefab == null || visualPrefabs == null || visualPrefabs.Length == 0) return;
        float delay = Random.Range(0f, 2f);
        StartCoroutine(SpawnAfterDelay(delay));
    }

    IEnumerator SpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        float x = Random.Range(-spawnRadius, spawnRadius);
        float z = Random.Range(-spawnRadius, spawnRadius);
        Vector3 spawnPos = new Vector3(x, 0.5f, z);

        GameObject newNPC = Instantiate(npcBasePrefab, spawnPos, Quaternion.identity);

        GameObject visualPrefab = visualPrefabs[Random.Range(0, visualPrefabs.Length)];
        GameObject visual = Instantiate(visualPrefab, newNPC.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;

        NPCMover mover = newNPC.GetComponent<NPCMover>();
        if (mover != null)
        {
            mover.npcName = npcNames[Random.Range(0, npcNames.Length)];
            mover.foods = foods;
mover.bed = bed;
        }
    }
}