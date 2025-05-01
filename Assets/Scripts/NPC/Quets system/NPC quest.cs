using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [Header("Quest Setup")]
    public Transform questTarget;        // The structure/NPC to find

    [Header("Distances")]
    public float interactRadius     = 3f;  // How close to talk / turn in
    public float targetRadius       = 3f;  // How close to reach the target

    [Header("UI Texts")]
    [TextArea] public string questDescription    = "Find that structure.";
    [TextArea] public string foundMessage        = "You found the target! Return to quest giver.";
    [TextArea] public string completionMessage   = "Great job! Quest complete.";

    [Header("Timings")]
    public float startMessageDuration      = 2f;  // Seconds to show on start
    public float foundMessageDuration      = 2f;  // Seconds to show when target reached
    public float completionMessageDuration = 2f;  // Seconds to show on complete

    // Internal state
    bool  questStarted;
    bool  targetReached;
    bool  questCompleted;
    float startMessageTimer;
    float foundMessageTimer;
    float completionMessageTimer;

    Transform player;

    void Awake()
    {
        // Auto-assign player by "Player" tag
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || questTarget == null) return;

        // Countdown any active message timers
        if (startMessageTimer      > 0f) startMessageTimer      -= Time.deltaTime;
        if (foundMessageTimer      > 0f) foundMessageTimer      -= Time.deltaTime;
        if (completionMessageTimer > 0f) completionMessageTimer -= Time.deltaTime;

        float distToNPC   = Vector3.Distance(player.position, transform.position);
        bool  nearNPC     = distToNPC <= interactRadius;

        // 1) Accept the quest
        if (!questStarted && nearNPC && Input.GetKeyDown(KeyCode.E))
        {
            questStarted       = true;
            startMessageTimer  = startMessageDuration;
        }
        // 2) Travel to target
        else if (questStarted && !targetReached)
        {
            if (Vector3.Distance(player.position, questTarget.position) <= targetRadius)
            {
                targetReached      = true;
                foundMessageTimer  = foundMessageDuration;
            }
        }
        // 3) Return & complete
        else if (targetReached && !questCompleted && nearNPC && Input.GetKeyDown(KeyCode.E))
        {
            questCompleted         = true;
            completionMessageTimer = completionMessageDuration;
            TaskManager.Instance.IncrementCompletedTasks();
        }
    }

    void OnGUI()
    {
        if (player == null || questTarget == null) return;

        float w = Screen.width, h = Screen.height;

        // A) Start-popup
        if (startMessageTimer > 0f)
        {
            GUI.Box(new Rect(w/2 -150, h/2 -50, 300, 100), questDescription);
            return;
        }

        // B) Found-popup
        if (foundMessageTimer > 0f)
        {
            GUI.Box(new Rect(w/2 -150, h/2 -50, 300, 100), foundMessage);
            return;
        }

        // C) Completion-popup
        if (completionMessageTimer > 0f)
        {
            GUI.Box(new Rect(w/2 -150, h/2 -50, 300, 100), completionMessage);
            return;
        }

        // D) Prompt to start quest
        if (!questStarted && Vector3.Distance(player.position, transform.position) <= interactRadius)
        {
            GUI.Box(new Rect(w/2 -100, h -100, 200, 40), "Press E to start quest");
        }

        // E) Prompt to complete quest
        if (targetReached && !questCompleted &&
            Vector3.Distance(player.position, transform.position) <= interactRadius)
        {
            GUI.Box(new Rect(w/2 -100, h -100, 200, 40), "Press E to complete quest");
        }
    }
}
