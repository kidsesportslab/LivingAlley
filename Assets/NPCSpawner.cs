using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NPCSpawner : MonoBehaviour
{
    public GameObject npcBasePrefab;
    public GameObject[] visualPrefabs;
    public float spawnRadius = 4f;
    public GameObject[] foods;
    public GameObject bed;
    public int initialSpawnCount = 3;
    private string[] npcNames = {
        "\u30b1\u30f3\u30b8", "\u30cf\u30eb\u30c8", "\u30bd\u30a6\u30bf", "\u30ea\u30e7\u30a6", "\u30ca\u30aa\u30ad",
        "\u30a2\u30e4\u30ab", "\u30df\u30b5\u30ad", "\u30e6\u30a4", "\u30cf\u30ca", "\u30b5\u30af\u30e9"
    };
    private Queue<string> nameQueue = new Queue<string>();
    private int nameGeneration = 0;
    void Start()
    {
        RefillNameQueue();
        for (int i = 0; i < initialSpawnCount; i++)
            SpawnNPC();
    }
    private void RefillNameQueue()
    {
        List<string> shuffled = new List<string>(npcNames);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        nameGeneration++;
        foreach (var name in shuffled)
            nameQueue.Enqueue(nameGeneration == 1 ? name : $"{name}{nameGeneration}");
    }
    private string GetUniqueName()
    {
        if (nameQueue.Count == 0) RefillNameQueue();
        return nameQueue.Dequeue();
    }
    public void SpawnNPC()
    {
        if (npcBasePrefab == null || visualPrefabs == null || visualPrefabs.Length == 0) return;
        StartCoroutine(SpawnAfterDelay(Random.Range(0f, 2f)));
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
            mover.npcName = GetUniqueName();
            mover.foods = foods;
            mover.bed = bed;
        }
    }
}