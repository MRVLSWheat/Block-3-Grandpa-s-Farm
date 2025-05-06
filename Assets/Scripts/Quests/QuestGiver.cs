using UnityEngine;

public class QuestGiver : MonoBehaviour {
    public QuestSO quest;
    bool playerInRange;

    void Update() {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)) {
            QuestManager.Instance.StartQuest(quest);
            Debug.Log("Started quest: " + quest.questTitle);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) playerInRange = true;
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}