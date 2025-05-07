using UnityEngine;

public class QuestGiver : MonoBehaviour {
    public QuestSO quest;             // assign your QuestSO here
    bool playerInRange = false;

    void Update() {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.E)) 
            return;

        // If already completed, don’t do anything
        if (QuestManager.Instance.completedQuests.Contains(quest.questID)) {
            Debug.Log("You have already completed this quest.");
            return;
        }

        // If quest not yet active, start it
        if (!QuestManager.Instance.activeQuests.Contains(quest)) {
            QuestManager.Instance.StartQuest(quest);
            Debug.Log("Started quest: " + quest.questTitle);
            return;
        }

        // Quest is active: check if find objective is done
        // Find the index of the Talk objective
        int talkIndex = quest.objectives.FindIndex(o => o.type == ObjectiveType.Talk);
        int findIndex = quest.objectives.FindIndex(o => o.type == ObjectiveType.Find);

        int findProgress = QuestManager.Instance.GetProgress(quest.questID, findIndex);
        int findRequired = quest.objectives[findIndex].requiredAmount;

        // If find not done yet, remind player
        if (findProgress < findRequired) {
            Debug.Log("You haven’t found them yet!");
            return;
        }

        // Now attempt the talk objective
        int talkProgress = QuestManager.Instance.GetProgress(quest.questID, talkIndex);
        int talkRequired = quest.objectives[talkIndex].requiredAmount;

        if (talkProgress < talkRequired) {
            // Report one unit of talk progress (should complete it)
            QuestManager.Instance.ReportProgress(quest.questID, talkIndex, 1);
            Debug.Log("You talked to the quest giver to complete the quest.");
        } else {
            // Both objectives are done—but QuestManager.CompleteQuest
            // should have run—so this is just a fallback
            Debug.Log("Quest is already in progress."); 
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) playerInRange = true;
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
