// NPCDialogue.cs
using UnityEngine;
using System;

public class NPCDialogue : MonoBehaviour
{
    [Serializable]
    public class DialogueEntry
    {
        [Tooltip("Inclusive lower bound for disturbanceValue")]
        public float minDisturbance;
        [Tooltip("Exclusive upper bound for disturbanceValue")]
        public float maxDisturbance;
        [TextArea(2,5)]
        [Tooltip("What the NPC will say when the disturbance is in this range")]
        public string text;
    }

    [Header("Dialogue Tiers (enter your text & ranges here)")]
    public DialogueEntry[] entries;

    [Header("Player & Interaction")]
    [Tooltip("Tag on your Player GameObject")]
    public string playerTag = "Player";
    [Tooltip("How close the player must be to interact")]
    public float detectionRange = 3f;
    [Tooltip("Which key to press to talk")]
    public KeyCode interactKey = KeyCode.E;

    [Header("On-Screen Box Settings")]
    [Tooltip("Width & height of the dialogue box in pixels")]
    public Vector2 guiBoxSize = new Vector2(300, 80);
    [Tooltip("Vertical offset from bottom of screen")]
    public float bottomOffset = 10f;

    [Header("Font Size Settings")]
    [Tooltip("Starting (maximum) font size for auto-shrink")]
    public int maxFontSize = 24;
    [Tooltip("Smallest allowed font size")]
    public int minFontSize = 10;

    [Header("Linger Settings")]
    [Tooltip("Seconds to keep showing after leaving range")]
    public float lingerDuration = 2f;

    // internals
    Transform _player;
    bool      _showDialogue = false;
    string    _currentLine  = "";
    float     _leaveTimer   = 0f;

    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag(playerTag);
        if (go != null) _player = go.transform;
        else            Debug.LogError($"[NPCDialogue] No GameObject tagged '{playerTag}' in scene.");

        if (entries == null || entries.Length == 0)
            Debug.LogWarning($"[{name}] has no DialogueEntry items!");
    }

    void Update()
    {
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist <= detectionRange;

        if (inRange && Input.GetKeyDown(interactKey))
        {
            TriggerDialogue();
            _leaveTimer = 0f;
        }

        if (_showDialogue && !inRange)
        {
            _leaveTimer += Time.deltaTime;
            if (_leaveTimer >= lingerDuration)
                _showDialogue = false;
        }
        else if (inRange)
        {
            _leaveTimer = 0f;
        }
    }

    void TriggerDialogue()
    {
        float d = DisturbanceManager.Instance.disturbanceValue;
        foreach (var e in entries)
        {
            if (d >= e.minDisturbance && d < e.maxDisturbance)
            {
                _currentLine = e.text;
                _showDialogue = true;
                return;
            }
        }
        _currentLine = "...";
        _showDialogue = true;
    }

    void OnGUI()
    {
        // 1) Early-out if the quest UI popup is active
        if (NPCQuest.PopupActive)
            return;

        // 2) Then your normal dialogue check
        if (!_showDialogue)
            return;

        // 3) Draw the dialogue box
        float w = guiBoxSize.x;
        float h = guiBoxSize.y;
        float x = (Screen.width  - w) / 2f;
        float y = Screen.height - h - bottomOffset;
        Rect boxRect = new Rect(x, y, w, h);

        GUIStyle style = new GUIStyle(GUI.skin.box)
        {
            wordWrap  = true,
            alignment = TextAnchor.MiddleCenter
        };

        GUIContent content = new GUIContent(_currentLine);
        int fontSize = maxFontSize;
        style.fontSize = fontSize;

        float requiredHeight = style.CalcHeight(content, w);
        while (requiredHeight > h && fontSize > minFontSize)
        {
            fontSize--;
            style.fontSize = fontSize;
            requiredHeight = style.CalcHeight(content, w);
        }

        GUI.Box(boxRect, content, style);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
