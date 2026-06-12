using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public float workReward = 20f;
    public float money = 30f;
    public string npcName = "名無し";
    public float lifespan = 300f;
    public float eatDuration = 3f;
    public float sleepDuration = 5f;
    public float socialDuration = 3f;
    private float age = 0f;
    public TextMeshPro statusText;
    public TextMeshProUGUI bubbleText;
    private float bubbleTimer = 0f;
    private string[] socialPhrases = { "やあ！", "元気？", "最近どう？", "いい天気だね", "また会ったね" };

    private Vector3 currentTarget;
    private float waitTimer = 0f;
    private float workTimer = 0f;
    private float eatTimer = 0f;
    private float sleepTimer = 0f;
    private float socialTimer = 0f;
    private bool isEating = false;
    private bool isSleeping = false;
    private bool isSocializing = false;
    private float hunger = 0f;
    private float fatigue = 0f;
    private float loneliness = 0f;
    private bool isWaiting = false;
    private bool isWorking = false;
    private bool isHungry = false;
    private bool isTired = false;
    private bool isLonely = false;
    private Animator animator;
    private int currentState = -1;
    private bool isBroke = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        hunger = Random.Range(0f, hungerMax * 0.5f);
        fatigue = Random.Range(0f, fatigueMax * 0.5f);
        GameLogger.Instance.Log(npcName, "誕生した");
        SetRandomTarget();
    }

    void LateUpdate()
    {
        if (Camera.main == null) return;
        Quaternion camRot = Camera.main.transform.rotation;
        if (statusText != null)
            statusText.transform.rotation = camRot;
        if (bubbleText != null)
            bubbleText.canvas.transform.rotation = camRot;
    }

    void SetAnimState(int s)
    {
        if (animator != null && currentState != s)
        {
            animator.SetInteger("state", s);
            currentState = s;
        }
    }

    void Update()
    {
        if (bubbleTimer > 0f)
        {
            bubbleTimer -= Time.deltaTime;
            if (bubbleTimer <= 0f && bubbleText != null)
                bubbleText.text = "...";
        }

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
        if (money <= 0f && !isBroke)
{
    money = 0f;
    isBroke = true;
    isWorking = true;
    GameLogger.Instance.Log(npcName, "お金が尽きた");
}
else if (money > 0f)
{
    isBroke = false;
}

        // 食事中タイマー
        if (isEating)
        {
            UpdateStatus("hungry");
            SetAnimState(5);
            eatTimer += Time.deltaTime;
            if (eatTimer >= eatDuration)
            {
                isEating = false;
                eatTimer = 0f;
                GameLogger.Instance.Log(npcName, "食事をした");
                SetRandomTarget();
            }
            return;
        }

        // 睡眠中タイマー
        if (isSleeping)
        {
            UpdateStatus("sleep");
            SetAnimState(4);
            sleepTimer += Time.deltaTime;
            if (sleepTimer >= sleepDuration)
            {
                isSleeping = false;
                sleepTimer = 0f;
                fatigue = 0f;
                isTired = false;
                GameLogger.Instance.Log(npcName, "眠った");
                SetRandomTarget();
            }
            return;
        }

        // 社交中タイマー
        if (isSocializing)
        {
            UpdateStatus("social");
            SetAnimState(3);
            socialTimer += Time.deltaTime;
            if (socialTimer >= socialDuration)
            {
                isSocializing = false;
                socialTimer = 0f;
                loneliness = 0f;
                isLonely = false;
                GameLogger.Instance.Log(npcName, "社交した");
                SetRandomTarget();
            }
            return;
        }

        if (isWorking)
        {
            UpdateStatus("working");
            SetAnimState(2);
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
                    SetAnimState(1);
                    MoveTowardNoAnim(nearestNPC.transform.position);
                }
                else
                {
                    isSocializing = true;
                    socialTimer = 0f;
                    StartCoroutine(ShowDialogueWhenReady());
                }
            }
            else
            {
                loneliness = 0f;
                isLonely = false;
                SetRandomTarget();
            }
            return;
        }

        if (isTired && bed != null)
        {
            UpdateStatus("sleep");
            float dist = Vector3.Distance(transform.position, bed.transform.position);
            if (dist > 0.5f)
            {
                SetAnimState(1);
                MoveTowardNoAnim(bed.transform.position);
            }
            else
            {
                isSleeping = true;
                sleepTimer = 0f;
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
                SetAnimState(1);
                MoveTowardNoAnim(nearestFood.transform.position);
            }
            else
            {
                if (money >= 25f)
                {
                    hunger = 0f;
                    isHungry = false;
                    RelocateFood(nearestFood);
                    money -= 25f;
                    isEating = true;
                    eatTimer = 0f;
                }
                else
                {
                    GameLogger.Instance.Log(npcName, "餓死した");
                    FindObjectsByType<NPCSpawner>(FindObjectsSortMode.None)[0].SpawnNPC();
                    Destroy(gameObject);
                    return;
                }
            }
            return;
        }

        UpdateStatus("walking");

        if (isWaiting)
        {
            SetAnimState(0);
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
            SetAnimState(1);
            MoveTowardNoAnim(currentTarget);
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
            statusText.text = npcName + "\n" + text + "\n$" + (int)money;

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

    void MoveTowardNoAnim(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void SetRandomTarget()
    {
        SetAnimState(0);
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
            if (d < minDist) { minDist = d; nearest = f; }
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
            if (d < minDist) { minDist = d; nearest = npc.gameObject; }
        }
        return nearest;
    }

    IEnumerator ShowDialogueWhenReady()
    {
        if (DialoguePool.Instance != null && DialoguePool.Instance.PoolCount == 0)
        {
            float elapsed = 0f;
            while (DialoguePool.Instance.PoolCount == 0 && elapsed < 8f)
            {
                yield return new WaitForSeconds(0.5f);
                elapsed += 0.5f;
            }
        }

        string dialogue = DialoguePool.Instance != null
            ? DialoguePool.Instance.GetDialogue(npcName, money, hunger)
            : null;
        ShowBubble(dialogue ?? socialPhrases[Random.Range(0, socialPhrases.Length)]);
    }

    void ShowBubble(string text)
    {
        if (bubbleText != null)
        {
            bubbleText.text = text;
            bubbleTimer = 3f;
        }
    }
}