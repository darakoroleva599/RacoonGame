using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Oven : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 25f;
    [SerializeField] private AudioClip ovenDingSound;

    private bool isActivated = false;
    private HumanController humanController;
    private AudioSource audioSource;

    private static List<GameObject> hiddenObjects = new List<GameObject>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        humanController = FindObjectOfType<HumanController>();
    }

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

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
        while (parent != null)
        {
            ovenParents.Add(parent.gameObject);
            parent = parent.parent;
        }

        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj == gameObject ||
                ovenParents.Contains(obj) ||
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

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name.ToLower().Contains("ovenknob"))
        {
            ShowMainScene();

            if (OvenData.wasCompleted)
            {
                CompletePrank();
            }
        }
    }

    private void CompletePrank()
    {
        isActivated = true;

        if (ovenDingSound != null)
            audioSource.PlayOneShot(ovenDingSound);

        if (humanController != null)
        {
            humanController.AddAnnoyance(annoyanceAmount);
            humanController.ReactToPrank(transform.position, 2f);
        }

        if (QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest("Oven");
    }
}