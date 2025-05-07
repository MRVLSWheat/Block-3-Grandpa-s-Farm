using UnityEngine;
using TMPro;
using System.Collections;

public class RewardPopupController : MonoBehaviour {
    [Tooltip("Panel that contains the reward popup UI")]
    public GameObject popupPanel;
    [Tooltip("Text component for the reward message")]
    public TMP_Text rewardText;
    [Tooltip("How long the popup stays visible")]
    public float displayDuration = 2f;

    void Awake() {
        // Ensure it starts hidden
        popupPanel.SetActive(false);
        // Subscribe to the quest-complete event
        QuestManager.Instance.OnQuestComplete += ShowReward;
    }

    void OnDestroy() {
        // Clean up subscription
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestComplete -= ShowReward;
    }

    void ShowReward(QuestSO quest) {
        Debug.Log($"[RewardPopup] ShowReward called for quest: {quest.questTitle}");
        rewardText.text = $"Quest '{quest.questTitle}' complete! You earned a reward.";
        StartCoroutine(DisplayPopup());
    }

    IEnumerator DisplayPopup() {
        Debug.Log("[RewardPopup] Activating panel");
        popupPanel.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        Debug.Log("[RewardPopup] Hiding panel");
        popupPanel.SetActive(false);
    }
}
