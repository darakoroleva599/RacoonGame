using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [System.Serializable]
    public class Quest
    {
        public string questName;
        public TextMeshProUGUI questText;
        [HideInInspector] public bool isCompleted = false;
    }

    [SerializeField] private Quest[] quests;

    public static QuestManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateQuestDisplay();
    }

    public void CompleteQuest(string questName)
    {
        foreach (Quest quest in quests)
        {
            if (quest.questName == questName)
            {
                quest.isCompleted = true;
                break;
            }
        }
        UpdateQuestDisplay();
    }

    public bool IsQuestCompleted(string questName)
    {
        foreach (Quest quest in quests)
        {
            if (quest.questName == questName)
                return quest.isCompleted;
        }
        return false;
    }

    private void UpdateQuestDisplay()
    {
        foreach (Quest quest in quests)
        {
            if (quest.questText != null)
            {
                if (quest.isCompleted)
                {
                    quest.questText.fontStyle = FontStyles.Strikethrough;
                    quest.questText.color = Color.gray;
                }
                else
                {
                    quest.questText.fontStyle = FontStyles.Normal;
                    quest.questText.color = Color.white;
                }
            }
        }
    }
}