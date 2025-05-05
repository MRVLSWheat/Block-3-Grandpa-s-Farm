using UnityEngine;

public class NPCQuest : MonoBehaviour
{
    [Header("Quest Setup")]
    [Tooltip("Drag the quest target here. If left empty, will attempt lookup by tag or name.")]
    public Transform questTarget;
    [Tooltip("Tag or name used only if questTarget is not manually assigned.")]
    public string questTargetIdentifier = "QuestTarget";

    [Header("Marker Prefab (Optional)")]
    [Tooltip("Drag an arrow prefab here if you want a custom marker; otherwise a default mesh will be generated.")]
    public GameObject arrowPrefabOverride;

    [Header("Accessibility")]
    [Tooltip("Dyslexia-friendly font to use for all UI text")]
    public Font dyslexicFont;

    [Tooltip("Offset applied to the arrow above the target or quest giver position")]
    public Vector3 arrowOffset = new Vector3(0f, 2f, 0f);

    [Header("Distances")]
    public float interactRadius = 3f;
    public float targetRadius = 3f;

    [Header("UI Texts")]
    [TextArea] public string questDescription = "Find that structure.";
    [TextArea] public string foundMessage = "You found the target! Return to quest giver.";
    [TextArea] public string completionMessage = "Great job! Quest complete.";

    [Header("Quest Stage Texts")]
    [TextArea] public string searchingStageText = "Go find the target.";
    [TextArea] public string returningStageText = "Return to the quest giver.";

    [Header("Timings")]
    public float startMessageDuration = 2f;
    public float foundMessageDuration = 2f;
    public float completionMessageDuration = 2f;

    // Internal state
    bool questStarted;
    bool targetReached;
    bool questCompleted;
    float startMessageTimer;
    float foundMessageTimer;
    float completionMessageTimer;

    Transform player;
    GameObject arrowPrefab;
    GameObject arrowInstance;

    /// <summary>
    /// True while any of the quest pop-ups (start/found/complete) are on screen.
    /// </summary>
    public static bool PopupActive { get; private set; }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (questTarget == null)
        {
            // Try tag lookup
            try
            {
                questTarget = GameObject.FindGameObjectWithTag(questTargetIdentifier)?.transform;
            }
            catch
            {
                questTarget = GameObject.Find(questTargetIdentifier)?.transform;
            }
        }

        arrowPrefab = arrowPrefabOverride != null ? arrowPrefabOverride : CreateDefaultArrow();
    }

    void Update()
    {
        PopupActive = (startMessageTimer > 0f) || (foundMessageTimer > 0f) || (completionMessageTimer > 0f);

        if (player == null || questTarget == null || arrowPrefab == null)
            return;

        // Timers
        startMessageTimer = Mathf.Max(0f, startMessageTimer - Time.deltaTime);
        foundMessageTimer = Mathf.Max(0f, foundMessageTimer - Time.deltaTime);
        completionMessageTimer = Mathf.Max(0f, completionMessageTimer - Time.deltaTime);

        float distToGiver = Vector3.Distance(player.position, transform.position);
        bool nearGiver = distToGiver <= interactRadius;
        float distToTarget = Vector3.Distance(player.position, questTarget.position);
        bool nearTarget = distToTarget <= targetRadius;

        // Accept
        if (!questStarted && nearGiver && Input.GetKeyDown(KeyCode.E))
        {
            questStarted = true;
            startMessageTimer = startMessageDuration;
        }
        // Found target
        else if (questStarted && !targetReached && nearTarget)
        {
            targetReached = true;
            foundMessageTimer = foundMessageDuration;
        }
        // Complete
        else if (targetReached && !questCompleted && nearGiver && Input.GetKeyDown(KeyCode.E))
        {
            questCompleted = true;
            completionMessageTimer = completionMessageDuration;
            TaskManager.Instance.IncrementCompletedTasks();
        }

        // Arrow logic
        Transform markerDest = null;
        if (!questStarted) markerDest = transform;
        else if (questStarted && !targetReached) markerDest = questTarget;
        else if (targetReached && !questCompleted) markerDest = transform;

        if (markerDest != null)
        {
            if (arrowInstance == null)
            {
                arrowInstance = Instantiate(arrowPrefab);
                arrowInstance.name = "QuestArrow";
            }
            arrowInstance.transform.position = markerDest.position + arrowOffset;
            arrowInstance.transform.LookAt(player.position + Vector3.up);
            arrowInstance.transform.Rotate(180f, 0f, 0f, Space.Self);
        }
        else if (arrowInstance != null)
        {
            Destroy(arrowInstance);
            arrowInstance = null;
        }
    }

    void OnGUI()
    {
        if (player == null || questTarget == null) return;

        float w = Screen.width;
        float h = Screen.height;
        float boxW = 400f;
        float boxH = 100f;
        float x = (w - boxW) / 2f;
        float y = h - boxH - 10f;

        // Popup style
        GUIStyle popupStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            fontStyle = FontStyle.Bold,
            font = dyslexicFont ?? GUI.skin.box.font
        };
        popupStyle.fontSize = 24;

        GUIContent content;
        int fontSize;
        float requiredHeight;

        // A) Start
        if (startMessageTimer > 0f)
        {
            content = new GUIContent(questDescription);
            fontSize = popupStyle.fontSize;
            requiredHeight = popupStyle.CalcHeight(content, boxW);
            while (requiredHeight > boxH && fontSize > 10)
            {
                fontSize--;
                popupStyle.fontSize = fontSize;
                requiredHeight = popupStyle.CalcHeight(content, boxW);
            }
            GUI.Box(new Rect(x, y, boxW, boxH), content, popupStyle);
            return;
        }
        // B) Found
        if (foundMessageTimer > 0f)
        {
            content = new GUIContent(foundMessage);
            fontSize = popupStyle.fontSize;
            requiredHeight = popupStyle.CalcHeight(content, boxW);
            while (requiredHeight > boxH && fontSize > 10)
            {
                fontSize--;
                popupStyle.fontSize = fontSize;
                requiredHeight = popupStyle.CalcHeight(content, boxW);
            }
            GUI.Box(new Rect(x, y, boxW, boxH), content, popupStyle);
            return;
        }
        // C) Completion
        if (completionMessageTimer > 0f)
        {
            content = new GUIContent(completionMessage);
            fontSize = popupStyle.fontSize;
            requiredHeight = popupStyle.CalcHeight(content, boxW);
            while (requiredHeight > boxH && fontSize > 10)
            {
                fontSize--;
                popupStyle.fontSize = fontSize;
                requiredHeight = popupStyle.CalcHeight(content, boxW);
            }
            GUI.Box(new Rect(x, y, boxW, boxH), content, popupStyle);
            return;
        }

        // Interaction
        float distGUI = Vector3.Distance(player.position, transform.position);
        bool nearGUI = distGUI <= interactRadius;
        if (!questStarted && nearGUI)
            GUI.Box(new Rect((w - 200f) / 2f, h - 100f, 200f, 40f), "Press E to start quest");
        else if (targetReached && !questCompleted && nearGUI)
            GUI.Box(new Rect((w - 200f) / 2f, h - 100f, 200f, 40f), "Press E to complete quest");

        // Stage text top-right
        if (questStarted && !questCompleted)
        {
            content = new GUIContent(!targetReached ? searchingStageText : returningStageText);
            float stageW = 400f;
            float stageH = 100f;
            float stageX = w - stageW - 10f;
            float stageY = 10f;
            GUIStyle stageStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                font = dyslexicFont ?? GUI.skin.box.font
            };
            stageStyle.fontSize = 24;
            fontSize = stageStyle.fontSize;
            requiredHeight = stageStyle.CalcHeight(content, stageW);
            while (requiredHeight > stageH && fontSize > 10)
            {
                fontSize--;
                stageStyle.fontSize = fontSize;
                requiredHeight = stageStyle.CalcHeight(content, stageW);
            }
            GUI.Box(new Rect(stageX, stageY, stageW, stageH), content, stageStyle);
        }
    }

    GameObject CreateDefaultArrow()
    {
        var go = new GameObject("DefaultArrowPrefab");
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Standard"));

        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3(0f,1f,0f),
            new Vector3(-0.5f,0f,0.5f),
            new Vector3(0.5f,0f,0.5f),
            new Vector3(0.5f,0f,-0.5f),
            new Vector3(-0.5f,0f,-0.5f)
        };
        mesh.triangles = new int[] {0,1,2, 0,2,3, 0,3,4, 0,4,1, 4,2,1, 4,3,2};
        mesh.RecalculateNormals();
        mf.mesh = mesh;

        return go;
    }
}
