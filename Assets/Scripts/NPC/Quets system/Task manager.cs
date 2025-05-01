// TaskManager.cs
// Attach this to your GameProcesses GameObject.

using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    public int CompletedTasks { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.Log("[TaskManager] Duplicate instance destroyed");
            return;
        }
        Instance = this;
        Debug.Log("[TaskManager] Awake → singleton instance set");
    }

    public void IncrementCompletedTasks()
    {
        CompletedTasks++;
        Debug.Log($"[TaskManager] Total tasks completed: {CompletedTasks}");
    }
}
