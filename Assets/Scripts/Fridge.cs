using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Fridge : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 25f;

    private bool isActivated = false;
    private HumanController humanController;

    private static List<GameObject> hiddenObjects = new List<GameObject>();

    void Start()
    {
        humanController = FindObjectOfType<HumanController>();
    }

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnMinigameSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnMinigameSceneUnloaded;
    }

    public void StartMinigame()
    {
        if (isActivated) return;

        if (FridgeData.Instance != null)
        {
            FridgeData.Instance.annoyanceAmount = annoyanceAmount;
            FridgeData.Instance.fridgePosition = transform.position;
        }

        FridgeData.pepperTaken = false;

        HideMainScene();
        SceneManager.LoadScene("FridgeMinigame", LoadSceneMode.Additive);
    }

    private void HideMainScene()
    {
        hiddenObjects.Clear();

        List<GameObject> fridgeParents = new List<GameObject>();
        Transform parent = transform;
        while (parent != null)
        {
            fridgeParents.Add(parent.gameObject);
            parent = parent.parent;
        }

        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj == gameObject ||
                fridgeParents.Contains(obj) ||
                obj.GetComponent<OvenData>() != null ||
                obj.GetComponent<FlowerPotData>() != null ||
                obj.GetComponent<FridgeData>() != null ||
                obj.GetComponent<QuestManager>() != null ||
                obj.name.Contains("EventSystem") ||
                obj.name.Contains("GameData") ||
                obj.name.Contains("Main Camera") ||
                obj.name.Contains("Camera"))
            {
                continue;
            }

            if (!obj.activeSelf)
                continue;

            obj.SetActive(false);
            hiddenObjects.Add(obj);
        }
    }

    private void ShowMainScene()
    {
        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        hiddenObjects.Clear();
        Time.timeScale = 1;
    }

    private void OnMinigameSceneUnloaded(Scene scene)
    {
        if (scene.name == "FridgeMinigame")
        {
            ShowMainScene();

            if (FridgeData.pepperTaken)
            {
                CompletePrank();
            }
        }
    }

    private void CompletePrank()
    {
        isActivated = true;

        if (humanController != null)
        {
            humanController.AddAnnoyance(annoyanceAmount);
            humanController.ReactToPrank(transform.position, 2f);
        }

        if (QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest("Fridge");
        PepperDisplay.Show();
    }
}