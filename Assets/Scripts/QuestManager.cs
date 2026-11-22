using UnityEngine;
using TMPro;


public class QuestManager : MonoBehaviour
{
    public enum Level { Level1, Level2 }


    [System.Serializable]
    public class Quest
    {
        public string questName;
        public TextMeshProUGUI questText;
        [HideInInspector] public bool isCompleted = false;
    }

    [Header("Level Settings")]
    [SerializeField] private Level currentLevel = Level.Level1;
    [SerializeField] public Quest[] quests;
    public static QuestManager Instance { get; private set; }


    private readonly string[] level1QuestOrder = { "FlowerPot", "Oven", "Fridge", "Pie" };
    private readonly string[] level2QuestOrder = { "Bookshelf", "ParrotCage", "Safe", "Phone" };


    private string[] GetQuestOrder() => currentLevel == Level.Level1 ? level1QuestOrder : level2QuestOrder;


    void Awake() { Instance = this; }


    void Start()
    {

        if (currentLevel == Level.Level2)
        {
            if (BookshelfPuzzleManager.WasCompleted)
                MarkCompleted("Bookshelf");
            if (SafePuzzleManager.WasCompleted)
                MarkCompleted("Safe");
        }
        UpdateQuestDisplay();
    }


    public void CompleteQuest(string questName)
    {
        MarkCompleted(questName);
        UpdateQuestDisplay();
    }


    private void MarkCompleted(string questName)
    {
        foreach (Quest quest in quests)
        {
            if (quest.questName == questName)
            {
                quest.isCompleted = true;
                break;
            }
        }
    }


    public void UpdateQuestDisplayPublic() { UpdateQuestDisplay(); }


    public bool IsQuestCompleted(string questName)
    {
        foreach (Quest quest in quests)
            if (quest.questName == questName)
                return quest.isCompleted;
        return false;
    }


    public bool CanStartQuest(string questName)
    {
        foreach (string name in GetQuestOrder())
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
        foreach (string questName in GetQuestOrder())
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
                quest.questText.color = new Color(0.545f, 0.271f, 0.075f, 1f);
            }
        }
    }
}
