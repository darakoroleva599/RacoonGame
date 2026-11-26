
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public enum Level
    {
        Level1,
        Level2
    }

    [System.Serializable]
    public class Quest
    {
        public string questName;
        public TextMeshProUGUI questText;

        [HideInInspector]
        public bool isCompleted = false;
    }

    [Header("Level Settings")]
    [SerializeField] private Level currentLevel = Level.Level1;
    [SerializeField] public Quest[] quests;

    public static QuestManager Instance { get; private set; }


    private static readonly HashSet<string> completedQuestNames = new HashSet<string>();

    private readonly string[] level1QuestOrder =
    {
        "FlowerPot",
        "Oven",
        "Fridge",
        "Pie"
    };

    private readonly string[] level2QuestOrder =
    {
        "Bookshelf",
        "ParrotCage",
        "Safe",
        "Phone"
    };

    private string[] GetQuestOrder()
    {
        return currentLevel == Level.Level1
            ? level1QuestOrder
            : level2QuestOrder;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        RestoreCompletedQuests();


        if (currentLevel == Level.Level2)
        {
            if (BookshelfPuzzleManager.WasCompleted)
                RegisterQuestCompleted("Bookshelf");

            if (SafePuzzleManager.WasCompleted)
                RegisterQuestCompleted("Safe");

            if (PhonePuzzleManager.WasCompleted)
                RegisterQuestCompleted("Phone");
        }

        UpdateQuestDisplay();
    }


    public void CompleteQuest(string questName)
    {
        RegisterQuestCompleted(questName);
        UpdateQuestDisplay();
    }


    public static void RegisterQuestCompleted(string questName)
    {
        if (string.IsNullOrEmpty(questName))
            return;

        completedQuestNames.Add(questName);

        if (Instance != null)
        {
            Instance.MarkCompletedLocal(questName);
            Instance.UpdateQuestDisplay();
        }

    }

    private void RestoreCompletedQuests()
    {
        if (quests == null)
            return;

        foreach (Quest quest in quests)
        {
            if (quest == null)
                continue;

            quest.isCompleted = completedQuestNames.Contains(quest.questName);
        }
    }

    private void MarkCompletedLocal(string questName)
    {
        if (quests == null)
            return;

        foreach (Quest quest in quests)
        {
            if (quest == null)
                continue;

            if (quest.questName == questName)
            {
                quest.isCompleted = true;
                return;
            }
        }
    }

    public void UpdateQuestDisplayPublic()
    {
        UpdateQuestDisplay();
    }

    public bool IsQuestCompleted(string questName)
    {
        return completedQuestNames.Contains(questName);
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
            if (IsQuestCompleted(questName))
                continue;

            foreach (Quest quest in quests)
            {
                if (quest == null)
                    continue;

                if (quest.questName == questName)
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
            if (quest == null || quest.questText == null)
                continue;

            bool shouldShow = quest.questName == currentQuest;

            quest.questText.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                quest.questText.fontStyle = FontStyles.Normal;
                quest.questText.color = new Color(
                    0.545f,
                    0.271f,
                    0.075f,
                    1f
                );
            }
        }

    }
}
