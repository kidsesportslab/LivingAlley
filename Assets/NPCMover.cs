using UnityEngine;
using TMPro;

public class NPCMover : MonoBehaviour
{
    public float speed = 2f;
    public float waypointRadius = 5f;
    public float waitTime = 1f;
    public GameObject[] foods;
    public GameObject bed;
    public float hungerMax = 10f;
    public float fatigueMax = 15f;
    public float lonelinessMax = 20f;
    public float workDuration = 5f;
    public float workReward = 30f;
    public float money = 100f;
    public string npcName = "名無し";
    public float lifespan = 300f;
    private float age = 0f;
    public TextMeshPro statusText;

    private Vector3 currentTarget;
    private float waitTimer = 0f;
    private float workTimer = 0f;
    private float hunger = 0f;
    private float fatigue = 0f;
    private float loneliness = 0f;
    private bool isWaiting = false;
    private bool isWorking = false;
    private bool isHungry = false;
    private bool isTired = false;
    private bool isLonely = false;

    void Start()
    {
        hunger = Random.Range(0f, hungerMax * 0.5f);
        fatigue = Random.Range(0f, fatigueMax * 0.5f);
        GameLogger.Instance.Log(npcName, "誕生した");
        SetRandomTarget();
    }

    void Update()
    {
        age += Time.deltaTime;
        if (age >= lifespan)
        {
            GameLogger.Instance.Log(npcName, "生涯を終えた");
            FindObjectsByType<NPCSpawner>(FindObjectsSortMode.None)[0].SpawnNPC();
            Destroy(gameObject);
            return;
        }

        hunger += Time.deltaTime;
        fatigue += Time.deltaTime;
        loneliness += Time.deltaTime;

        if (hunger >= hungerMax) isHungry = true;
        if (fatigue >= fatigueMax) isTired = true;
        if (loneliness >= lonelinessMax) isLonely = true;
        if (money <= 0f) { money = 0f; isWorking = true; }

        if (isWorking)
        {
            UpdateStatus("working");
            workTimer += Time.deltaTime;
            if (workTimer >= workDuration)
            {
                money += workReward;
                workTimer = 0f;
                isWorking = false;
                GameLogger.Instance.Log(npcName, "仕事をした");
                SetRandomTarget();
            }
            return;
        }

        if (isLonely)
        {
            UpdateStatus("social");
            GameObject nearestNPC = GetNearestNPC();
            if (nearestNPC != null)
            {
                float dist = Vector3.Distance(transform.position, nearestNPC.transform.position);
                if (dist > 1.5f)
                {
                    MoveToward(nearestNPC.transform.position);
                }
                else
                {
                    loneliness = 0f;
                    isLonely = false;
                    SetRandomTarget();
                }
            }
            else
            {
                loneliness = 0f;
                isLonely = false;
            }
            return;
        }

        if (isTired && bed != null)
        {
            UpdateStatus("sleep");
            float dist = Vector3.Distance(transform.position, bed.transform.position);
            if (dist > 0.5f)
            {
                MoveToward(bed.transform.position);
            }
            else
            {
                fatigue = 0f;
                isTired = false;
                GameLogger.Instance.Log(npcName, "眠った");
                SetRandomTarget();
            }
            return;
        }

        if (isHungry && foods != null && foods.Length > 0)
        {
            UpdateStatus("hungry");
            GameObject nearestFood = GetNearestFood();
            if (nearestFood == null) return;
            float dist = Vector3.Distance(transform.position, nearestFood.transform.position);
            if (dist > 0.5f)
            {
                MoveToward(nearestFood.transform.position);
            }
            else
            {
                hunger = 0f;
                isHungry = false;
                RelocateFood(nearestFood);
                money -= 10f;
                GameLogger.Instance.Log(npcName, "食事をした");
                SetRandomTarget();
            }
            return;
        }

        UpdateStatus("walking");

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SetRandomTarget();
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget);
        if (distance > 0.3f)
        {
            MoveToward(currentTarget);
        }
        else
        {
            isWaiting = true;
            waitTimer = waitTime;
        }
    }

    void UpdateStatus(string text)
    {
        if (statusText != null)
        {
            statusText.text = npcName + "\n" + text;
        }

        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        switch (text)
        {
            case "hungry":
                rend.material.color = Color.red;
                speed = 4f;
                break;
            case "sleep":
                rend.material.color = Color.blue;
                speed = 0.5f;
                break;
            case "working":
                rend.material.color = Color.yellow;
                speed = 0f;
                break;
            case "social":
                rend.material.color = Color.green;
                speed = 3f;
                break;
            default:
                rend.material.color = Color.white;
                speed = 2f;
                break;
        }
    }

    void MoveToward(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void SetRandomTarget()
    {
        float x = Random.Range(-waypointRadius, waypointRadius);
        float z = Random.Range(-waypointRadius, waypointRadius);
        currentTarget = new Vector3(x, transform.position.y, z);
    }

    void RelocateFood(GameObject target)
    {
        if (target == null) return;
        float x = Random.Range(-waypointRadius, waypointRadius);
        float z = Random.Range(-waypointRadius, waypointRadius);
        target.transform.position = new Vector3(x, 0.5f, z);
    }

    GameObject GetNearestFood()
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;
        foreach (GameObject f in foods)
        {
            if (f == null) continue;
            float d = Vector3.Distance(transform.position, f.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = f;
            }
        }
        return nearest;
    }

    GameObject GetNearestNPC()
    {
        NPCMover[] allNPCs = FindObjectsByType<NPCMover>(FindObjectsSortMode.None);
        GameObject nearest = null;
        float minDist = float.MaxValue;
        foreach (NPCMover npc in allNPCs)
        {
            if (npc.gameObject == this.gameObject) continue;
            float d = Vector3.Distance(transform.position, npc.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = npc.gameObject;
            }
        }
        return nearest;
    }
}