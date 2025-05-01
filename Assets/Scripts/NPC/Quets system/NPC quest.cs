// NPCQuest.cs
// Attach this to your quest-giver NPC. No need to assign 'player' in the Inspector now,
// but do tag your Player GameObject as "Player".

using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [Header("References")]
    public Transform questTarget; // drag in the objective (structure/NPC)

    [Header("Distances")]
    public float interactRadius = 3f;
    public float targetRadius    = 3f;

    [Header("Texts")]
    [TextArea] public string questDescription   = "Find that building.";
    [TextArea] public string completionMessage  = "Nice work! Quest done.";

    // internal states
    bool questStarted   = false;
    bool targetReached  = false;
    bool questCompleted = false;
    bool messageAck     = false;

    Transform player;

    void Awake()
    {
        Debug.Log($"[NPCQuest] Awake on \"{gameObject.name}\"");

        // Auto-find the player by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            Debug.Log($"[NPCQuest] Auto-assigned player = {player.name}");
        }
        else
        {
            Debug.LogError("[NPCQuest] No GameObject tagged 'Player' found! Please tag your Player.");
        }

        if (questTarget == null)
            Debug.LogError("[NPCQuest] QuestTarget NOT assigned!");
    }

    void Update()
    {
        if (player == null || questTarget == null) return;

        float distToNPC   = Vector3.Distance(player.position, transform.position);
        bool  nearNPC     = distToNPC <= interactRadius;

        // DEBUG: player entering NPC radius
        if (nearNPC && !questStarted)
            Debug.Log($"[NPCQuest] Player is within interactRadius (dist={distToNPC:F2})");

        // 1) Start the quest
        if (!questStarted && nearNPC && Input.GetKeyDown(KeyCode.E))
        {
            questStarted = true;
            Debug.Log($"[NPCQuest] Quest started: {questDescription}");
        }
        // 2) Travel to target
        else if (questStarted && !targetReached)
        {
            float distToTarget = Vector3.Distance(player.position, questTarget.position);
            Debug.Log($"[NPCQuest] Dist→Target: {distToTarget:F2}");
            if (distToTarget <= targetRadius)
            {
                targetReached = true;
                Debug.Log("[NPCQuest] Objective reached — go back to NPC");
            }
        }
        // 3) Return & complete
        else if (targetReached && !questCompleted && nearNPC && Input.GetKeyDown(KeyCode.E))
        {
            questCompleted = true;
            Debug.Log("[NPCQuest] Quest complete! Bumping TaskManager.");
            TaskManager.Instance.IncrementCompletedTasks();
        }
    }

    void OnGUI()
    {
        if (player == null || questTarget == null) return;

        float w = Screen.width, h = Screen.height;

        if (!questStarted && Vector3.Distance(player.position, transform.position) <= interactRadius)
            GUI.Box(new Rect(w/2 -100, h - 100, 200, 40), "Press E to start quest");

        if (targetReached && !questCompleted && Vector3.Distance(player.position, transform.position) <= interactRadius)
            GUI.Box(new Rect(w/2 -100, h - 100, 200, 40), "Press E to complete quest");

        if (questCompleted && !messageAck)
        {
            Rect box = new Rect(w/2 -150, h/2 -75, 300, 150);
            GUI.Box(box, completionMessage);
            if (GUI.Button(new Rect(w/2 -50, h/2 +20, 100, 30), "OK"))
                messageAck = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);

        if (questTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(questTarget.position, targetRadius);
        }
    }
}
