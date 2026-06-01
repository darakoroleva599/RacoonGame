using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Pie : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 25f;
    [SerializeField] private Sprite spoiledPieSprite;

    private bool isActivated = false;
    private HumanController humanController;
    private SpriteRenderer spriteRenderer;

    private static List<GameObject> hiddenObjects = new List<GameObject>();

    void Start()
    {
        humanController = FindObjectOfType<HumanController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        if (!FridgeData.pepperTaken)
        {
            return;
        }

        if (PieData.Instance != null)
            PieData.Instance.annoyanceAmount = annoyanceAmount;

        PieData.wasCompleted = false;

        HideMainScene();
        SceneManager.LoadScene("PieMinigame", LoadSceneMode.Additive);
    }

    private void HideMainScene()
    {
        hiddenObjects.Clear();

        List<GameObject> pieParents = new List<GameObject>();
        Transform parent = transform;
        while (parent != null)
        {
            pieParents.Add(parent.gameObject);
            parent = parent.parent;
        }

        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj == gameObject ||
                pieParents.Contains(obj) ||
                obj.GetComponent<OvenData>() != null ||
                obj.GetComponent<FlowerPotData>() != null ||
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
        if (scene.name == "PieMinigame")
        {
            ShowMainScene();

            if (PieData.wasCompleted)
            {
                CompletePrank();
            }
        }
    }

    private void CompletePrank()
    {
        isActivated = true;

        if (spriteRenderer != null && spoiledPieSprite != null)
            spriteRenderer.sprite = spoiledPieSprite;

        if (humanController != null)
        {
            humanController.AddAnnoyance(annoyanceAmount);
            humanController.ReactToPrank(transform.position, 2f);
        }

        if (QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest("Pie");
        PepperDisplay.Hide();
    }
}