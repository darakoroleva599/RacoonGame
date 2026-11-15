
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [System.Serializable]
    public class Quest
    {
        public string questName;
        public TextMeshProUGUI questText;

        [HideInInspector]
        public bool isCompleted = false;
    }

    [SerializeField] public Quest[] quests;

    public static QuestManager Instance { get; private set; }

    private readonly string[] questOrder =
    {
        "FlowerPot",
        "Oven",
        "Fridge",
        "Pie"
    };

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

    public void UpdateQuestDisplayPublic()
    {
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

    public bool CanStartQuest(string questName)
    {
        string[] questOrder =
        {
        "FlowerPot",
        "Oven",
        "Fridge",
        "Pie"
    };

        foreach (string name in questOrder)
        {
            if (name == questName)
                return !IsQuestCompleted(name);

            if (!IsQuestCompleted(name))
                return false;
        }

        return false;
    }

    private void UpdateQuestDisplay()
    {
        string currentQuest = null;

        foreach (string questName in questOrder)
        {
            foreach (Quest quest in quests)
            {
                if (quest.questName == questName && !quest.isCompleted)
                {
                    currentQuest = questName;
                    break;
                }
            }

            if (currentQuest != null)
                break;
        }

        foreach (Quest quest in quests)
        {
            if (quest.questText == null)
                continue;

            bool shouldShow = quest.questName == currentQuest;

            quest.questText.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                quest.questText.fontStyle = FontStyles.Normal;
                quest.questText.color =
                    new Color(0.545f, 0.271f, 0.075f, 1f);
            }
        }
    }
}
