using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Oven : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 25f;
    [SerializeField] private AudioClip ovenDingSound;
    [SerializeField] private Transform returnPoint;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private GameObject ovenOnSprite;
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
        if (ovenOnSprite != null) ovenOnSprite.SetActive(false);
        if (dustCloud != null)
        {
            Animator anim = dustCloud.GetComponent<Animator>();
            if (anim != null) anim.SetBool("IsActive", false);
        }
    }

    void OnEnable() { SceneManager.sceneUnloaded += OnSceneUnloaded; }
    void OnDisable() { SceneManager.sceneUnloaded -= OnSceneUnloaded; }

    public void StartMinigame()
    {
        if (isActivated) return;
        if (OvenData.Instance != null)
        {
            OvenData.Instance.annoyanceAmount = annoyanceAmount;
            OvenData.Instance.ovenPosition = transform.position;
        }
        OvenData.wasCompleted = false;
        HideMainScene();
        SceneManager.LoadScene("OvenMiniGame", LoadSceneMode.Additive);
    }

    private void HideMainScene()
    {
        hiddenObjects.Clear();
        List<GameObject> ovenParents = new List<GameObject>();
        Transform parent = transform;
        while (parent != null) { ovenParents.Add(parent.gameObject); parent = parent.parent; }

        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj == gameObject || ovenParents.Contains(obj) ||
                obj.GetComponent<OvenData>() != null || obj.GetComponent<FlowerPotData>() != null ||
                obj.GetComponent<QuestManager>() != null || obj.GetComponent<SaveManager>() != null ||
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

    private void OnSceneUnloaded(Scene scene)
    {
        Time.timeScale = 1;
        if (scene.name.ToLower().Contains("ovenknob"))
        {
            ShowMainScene();
            if (OvenData.wasCompleted) CompletePrank();
        }
    }

    private void CompletePrank()
    {
        isActivated = true;
        if (ovenDingSound != null) audioSource.PlayOneShot(ovenDingSound);
        if (ovenOnSprite != null) ovenOnSprite.SetActive(true);
        if (humanController != null)
        {
            humanController.AddAnnoyance(annoyanceAmount);
            humanController.CheckPrank(checkPoint, returnPoint, 3f);
            humanController.OnCheckPrankStopped += ShowDust;
            humanController.OnCheckPrankLeaving += HideDust;
            humanController.OnCheckPrankFinished += HideDust;
            humanController.OnCheckPrankFinished += HideOvenSprite;
        }
        if (QuestManager.Instance != null) QuestManager.Instance.CompleteQuest("Oven");
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

    private void HideOvenSprite()
    {
        if (ovenOnSprite != null) ovenOnSprite.SetActive(false);
        if (humanController != null) humanController.OnCheckPrankFinished -= HideOvenSprite;
    }
}