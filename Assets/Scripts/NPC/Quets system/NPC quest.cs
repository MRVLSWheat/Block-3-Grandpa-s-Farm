using UnityEngine;

/// <summary>
/// Handles quest start, navigation markers for quest giver and target, and completion entirely at runtime.
/// Allows manual assignment of the quest target or falls back to lookup by tag or name.
/// Auto-generates a basic 3D arrow marker mesh if no prefab is provided.
/// Displays styled popups at the bottom-center of the screen and dynamic quest stage text at the top-right corner.
/// </summary>
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
    [Tooltip("Text displayed while searching for the target.")]
    [TextArea] public string searchingStageText = "Find the target.";
    [Tooltip("Text displayed after finding the target, prompting return.")]
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

    void Awake()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogError("[NPCQuest] No GameObject tagged 'Player' in scene.");

        if (questTarget == null)
        {
            try
            {
                var qtObj = GameObject.FindGameObjectWithTag(questTargetIdentifier);
                if (qtObj != null) questTarget = qtObj.transform;
            }
            catch (UnityException)
            {
                Debug.LogWarning($"[NPCQuest] Tag '{questTargetIdentifier}' not defined; attempting name lookup.");
            }
            if (questTarget == null)
            {
                var foundByName = GameObject.Find(questTargetIdentifier);
                if (foundByName != null) questTarget = foundByName.transform;
            }
            if (questTarget == null)
                Debug.LogError($"[NPCQuest] Could not find quest target. Please assign in inspector or check identifier '{questTargetIdentifier}'.");
        }

        arrowPrefab = arrowPrefabOverride != null ? arrowPrefabOverride : CreateDefaultArrow();
    }

    void Update()
    {
        if (player == null || questTarget == null || arrowPrefab == null)
            return;

        startMessageTimer = Mathf.Max(0f, startMessageTimer - Time.deltaTime);
        foundMessageTimer = Mathf.Max(0f, foundMessageTimer - Time.deltaTime);
        completionMessageTimer = Mathf.Max(0f, completionMessageTimer - Time.deltaTime);

        float distToGiver = Vector3.Distance(player.position, transform.position);
        bool nearQuestGiver = distToGiver <= interactRadius;
        float distToTarget = Vector3.Distance(player.position, questTarget.position);
        bool nearQuestTarget = distToTarget <= targetRadius;

        if (!questStarted && nearQuestGiver && Input.GetKeyDown(KeyCode.E))
        {
            questStarted = true;
            startMessageTimer = startMessageDuration;
        }
        else if (questStarted && !targetReached && nearQuestTarget)
        {
            targetReached = true;
            foundMessageTimer = foundMessageDuration;
        }
        else if (targetReached && !questCompleted && nearQuestGiver && Input.GetKeyDown(KeyCode.E))
        {
            questCompleted = true;
            completionMessageTimer = completionMessageDuration;
            TaskManager.Instance.IncrementCompletedTasks();
        }

        Transform markerDestination = null;
        if (!questStarted) markerDestination = transform;
        else if (questStarted && !targetReached) markerDestination = questTarget;
        else if (targetReached && !questCompleted) markerDestination = transform;

        if (markerDestination != null)
        {
            if (arrowInstance == null)
            {
                arrowInstance = Instantiate(arrowPrefab);
                arrowInstance.name = "QuestArrow";
            }
            arrowInstance.transform.position = markerDestination.position + arrowOffset;
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
        float popupWidth = 400f;
        float popupHeight = 100f;
        float popupX = (w - popupWidth) / 2;
        float popupY = h - popupHeight - 10f;

        // Style for popups
        GUIStyle popupStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            fontSize = 24,
            fontStyle = FontStyle.Bold
        };

        // Popups at bottom-center with styled text
        if (startMessageTimer > 0f)
        {
            GUI.Box(new Rect(popupX, popupY, popupWidth, popupHeight), questDescription, popupStyle);
            return;
        }
        if (foundMessageTimer > 0f)
        {
            GUI.Box(new Rect(popupX, popupY, popupWidth, popupHeight), foundMessage, popupStyle);
            return;
        }
        if (completionMessageTimer > 0f)
        {
            GUI.Box(new Rect(popupX, popupY, popupWidth, popupHeight), completionMessage, popupStyle);
            return;
        }

        // Interaction prompts
        if (!questStarted && Vector3.Distance(player.position, transform.position) <= interactRadius)
            GUI.Box(new Rect(w / 2 - 100, h - 100, 200, 40), "Press E to start quest");
        if (targetReached && !questCompleted && Vector3.Distance(player.position, transform.position) <= interactRadius)
            GUI.Box(new Rect(w / 2 - 100, h - 100, 200, 40), "Press E to complete quest");

        // Dynamic top-right quest stage text
        if (questStarted && !questCompleted)
        {
            string stageText = !targetReached ? searchingStageText : returningStageText;
            float stageWidth = 500f;
            float stageHeight = 120f;
            float stageX = w - stageWidth - 10f;
            float stageY = 10f;
            GUIStyle stageStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 24,
                fontStyle = FontStyle.Bold
            };
            GUI.Box(new Rect(stageX, stageY, stageWidth, stageHeight), stageText, stageStyle);
        }
    }

    GameObject CreateDefaultArrow()
    {
        var go = new GameObject("DefaultArrowPrefab");
        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Standard"));

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[] {
            new Vector3(0f, 1f, 0f),
            new Vector3(-0.5f, 0f,  0.5f),
            new Vector3( 0.5f, 0f,  0.5f),
            new Vector3( 0.5f, 0f, -0.5f),
            new Vector3(-0.5f, 0f, -0.5f)
        };
        int[] tris = new int[] {
            0,1,2,
            0,2,3,
            0,3,4,
            0,4,1,
            4,2,1,
            4,3,2
        };
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        filter.mesh = mesh;

        return go;
    }
}
