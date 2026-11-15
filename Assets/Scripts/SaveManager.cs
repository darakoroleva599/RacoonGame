using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public bool gameStarted = false;
    public float annoyanceProgress = 0f;
    public bool ovenCompleted = false;
    public bool flowerPotCompleted = false;
    public bool fridgeCompleted = false;
    public bool pieCompleted = false;
    public bool pepperTaken = false;
    public bool pepperVisible = false;
    public bool doorCompleted = false;
    public bool lampCompleted = false;
    public bool phoneCompleted = false;
    public string savedSceneName = "Kitchen";

    public bool hasSavedPlayerState = false;
    public Vector3 savedPlayerPosition;
    public bool savedPlayerOnTaburet = false;
    public bool savedPlayerOnPodosokonnik = false;
    public bool savedPlayerHidden = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (!gameStarted && SceneManager.GetActiveScene().name == "Kitchen")
        {
            gameStarted = true;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Kitchen" && gameStarted)
        {
            LoadProgress();
        }
    }


    public void SaveProgress()
    {
        HumanController human = FindObjectOfType<HumanController>();

        if (human != null)
            annoyanceProgress = human.GetAnnoyanceProgress();

        PlayerMove player = FindObjectOfType<PlayerMove>();

        if (player != null && !player.IsCrawling)
        {
            savedPlayerPosition = player.transform.position;
            savedPlayerOnTaburet = player.IsOnTaburet;
            savedPlayerOnPodosokonnik = player.IsOnPodosokonnik;
            savedPlayerHidden = player.IsHidden;
            hasSavedPlayerState = true;
        }

        pepperTaken = FridgeData.pepperTaken;
        pepperVisible = PepperDisplay.IsVisible();

        if (QuestManager.Instance != null)
        {
            ovenCompleted = QuestManager.Instance.IsQuestCompleted("Oven");
            flowerPotCompleted = QuestManager.Instance.IsQuestCompleted("FlowerPot");
            fridgeCompleted = QuestManager.Instance.IsQuestCompleted("Fridge");
            pieCompleted = QuestManager.Instance.IsQuestCompleted("Pie");
            doorCompleted = QuestManager.Instance.IsQuestCompleted("DoorDistraction");
            lampCompleted = QuestManager.Instance.IsQuestCompleted("LampDistraction");
            phoneCompleted = QuestManager.Instance.IsQuestCompleted("PhoneDistraction");
        }
    }



    public void LoadProgress()
    {
        Debug.Log("LoadProgress: QuestManager = " +
    (QuestManager.Instance == null ? "NULL" : "FOUND"));
        HumanController human = FindObjectOfType<HumanController>();

        if (human != null)
            human.SetAnnoyanceProgress(annoyanceProgress);

        PlayerMove player = FindObjectOfType<PlayerMove>();

        if (hasSavedPlayerState && player != null)
        {
            player.RestoreState(
                savedPlayerPosition,
                savedPlayerOnTaburet,
                savedPlayerOnPodosokonnik,
                savedPlayerHidden
            );
        }

        FridgeData.pepperTaken = pepperTaken;

        if (pepperVisible)
            PepperDisplay.Show();
        else
            PepperDisplay.Hide();

        if (QuestManager.Instance != null)
        {
            if (ovenCompleted) QuestManager.Instance.CompleteQuest("Oven");
            if (flowerPotCompleted) QuestManager.Instance.CompleteQuest("FlowerPot");
            if (fridgeCompleted) QuestManager.Instance.CompleteQuest("Fridge");
            if (pieCompleted) QuestManager.Instance.CompleteQuest("Pie");
            if (doorCompleted) QuestManager.Instance.CompleteQuest("DoorDistraction");
            if (lampCompleted) QuestManager.Instance.CompleteQuest("LampDistraction");
            if (phoneCompleted) QuestManager.Instance.CompleteQuest("PhoneDistraction");
        }
    }



    public void ResetProgress()
    {
        gameStarted = true;

        annoyanceProgress = 0f;

        hasSavedPlayerState = false;
        savedPlayerPosition = Vector3.zero;
        savedPlayerOnTaburet = false;
        savedPlayerOnPodosokonnik = false;
        savedPlayerHidden = false;

        ovenCompleted = false;
        flowerPotCompleted = false;
        fridgeCompleted = false;
        pieCompleted = false;
        pepperTaken = false;
        pepperVisible = false;
        doorCompleted = false;
        lampCompleted = false;
        phoneCompleted = false;

        FridgeData.pepperTaken = false;
        PepperDisplay.Hide();
    }

}