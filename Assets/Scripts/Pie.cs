using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Pie : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 25f;
    [SerializeField] private Sprite spoiledPieSprite;
    [SerializeField] private Transform returnPoint;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private GameObject dustCloud;

    private bool isActivated = false;
    private HumanController humanController;
    private SpriteRenderer spriteRenderer;

    private static List<GameObject> hiddenObjects = new List<GameObject>();

    void Start()
    {
        humanController = FindObjectOfType<HumanController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (dustCloud != null)
        {
            Animator anim = dustCloud.GetComponent<Animator>();
            if (anim != null) anim.SetBool("IsActive", false);
        }
    }

    void OnEnable() { SceneManager.sceneUnloaded += OnMinigameSceneUnloaded; }
    void OnDisable() { SceneManager.sceneUnloaded -= OnMinigameSceneUnloaded; }

    public void StartMinigame()
    {
        if (isActivated) return;
        if (!FridgeData.pepperTaken) return;
        if (PieData.Instance != null) PieData.Instance.annoyanceAmount = annoyanceAmount;
        PieData.wasCompleted = false;
        HideMainScene();
        SceneManager.LoadScene("PieMinigame", LoadSceneMode.Additive);
    }

    private void HideMainScene()
    {
        hiddenObjects.Clear();
        List<GameObject> pieParents = new List<GameObject>();
        Transform parent = transform;
        while (parent != null) { pieParents.Add(parent.gameObject); parent = parent.parent; }

        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj == gameObject || pieParents.Contains(obj) ||
                obj.GetComponent<OvenData>() != null ||
                obj.GetComponent<FlowerPotData>() != null ||
                obj.GetComponent<FridgeData>() != null ||
                obj.GetComponent<PieData>() != null ||
                obj.GetComponent<QuestManager>() != null ||
                obj.name.Contains("EventSystem") || obj.name.Contains("GameData") ||
                obj.name.Contains("Main Camera") || obj.name.Contains("Camera") ||
                obj.name.Contains("Colliders"))
                continue;
            if (!obj.activeSelf) continue;
            obj.SetActive(false);
            hiddenObjects.Add(obj);
        }
    }

    private void ShowMainScene()
    {
        foreach (GameObject obj in hiddenObjects) { if (obj != null) obj.SetActive(true); }
        hiddenObjects.Clear();
        Time.timeScale = 1;
    }

    private void OnMinigameSceneUnloaded(Scene scene)
    {
        Time.timeScale = 1;
        if (scene.name == "PieMinigame")
        {
            ShowMainScene();
            if (PieData.wasCompleted) CompletePrank();
        }
    }

    private void CompletePrank()
    {
        isActivated = true;
        if (spriteRenderer != null && spoiledPieSprite != null) spriteRenderer.sprite = spoiledPieSprite;
        if (humanController != null)
        {
            humanController.AddAnnoyance(annoyanceAmount);
            humanController.CheckPrank(checkPoint, returnPoint, 3f);
            humanController.OnCheckPrankStopped += ShowDust;
            humanController.OnCheckPrankLeaving += HideDust;
            humanController.OnCheckPrankFinished += HideDust;
        }
        if (QuestManager.Instance != null) QuestManager.Instance.CompleteQuest("Pie");
        PepperDisplay.Hide();
    }

    private void ShowDust()
    {
        if (dustCloud != null)
        {
            dustCloud.SetActive(true);
            Animator anim = dustCloud.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("IsActive", true);
                anim.speed = 0.7f;
            }
        }
    }

    private void HideDust()
    {
        if (dustCloud != null && dustCloud.activeSelf)
        {
            Animator anim = dustCloud.GetComponent<Animator>();
            if (anim != null) anim.SetBool("IsActive", false);
            dustCloud.SetActive(false);
        }

        if (humanController != null)
        {
            humanController.OnCheckPrankStopped -= ShowDust;
            humanController.OnCheckPrankLeaving -= HideDust;
            humanController.OnCheckPrankFinished -= HideDust;
        }
    }
}