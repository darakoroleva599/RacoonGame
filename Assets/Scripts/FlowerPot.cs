using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 25f;
    [SerializeField] private AudioClip potFallSound;
    [SerializeField] private GameObject dirtParticles;
    [SerializeField] private Transform returnPoint;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private GameObject dustCloud;

    private bool isActivated = false;
    private HumanController humanController;
    private AudioSource audioSource;

    private static List<GameObject> hiddenObjects = new List<GameObject>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        humanController = FindObjectOfType<HumanController>();
        if (dirtParticles != null) dirtParticles.SetActive(false);
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
        if (FlowerPotData.Instance != null)
        {
            FlowerPotData.Instance.annoyanceAmount = annoyanceAmount;
            FlowerPotData.Instance.potPosition = transform.position;
        }
        FlowerPotData.wasCompleted = false;
        HideMainScene();
        SceneManager.LoadScene("FlowerPotMiniGame", LoadSceneMode.Additive);
    }

    private void HideMainScene()
    {
        hiddenObjects.Clear();
        List<GameObject> potParents = new List<GameObject>();
        Transform parent = transform;
        while (parent != null) { potParents.Add(parent.gameObject); parent = parent.parent; }

        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj == gameObject || potParents.Contains(obj) ||
                obj.GetComponent<OvenData>() != null || obj.GetComponent<FlowerPotData>() != null ||
                obj.GetComponent<QuestManager>() != null ||
                obj.name.Contains("EventSystem") || obj.name.Contains("GameData") ||
                obj.name.Contains("Main Camera") || obj.name.Contains("Camera"))
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
        if (scene.name == "FlowerPotMiniGame")
        {
            ShowMainScene();
            if (FlowerPotData.wasCompleted) CompletePrank();
        }
    }

    private void CompletePrank()
    {
        isActivated = true;
        if (dirtParticles != null) dirtParticles.SetActive(true);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (potFallSound != null && audioSource != null) audioSource.PlayOneShot(potFallSound);
        if (humanController != null)
        {
            humanController.AddAnnoyance(annoyanceAmount);
            humanController.CheckPrank(checkPoint, returnPoint, 3f);
            humanController.OnCheckPrankStopped += ShowDust;
            humanController.OnCheckPrankLeaving += HideDust;
            humanController.OnCheckPrankFinished += HideDust;
        }
        if (QuestManager.Instance != null) QuestManager.Instance.CompleteQuest("FlowerPot");
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