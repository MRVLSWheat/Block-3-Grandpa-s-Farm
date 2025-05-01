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
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Call this whenever a quest is completed.
    /// </summary>
    public void IncrementCompletedTasks()
    {
        CompletedTasks++;
    }
}
