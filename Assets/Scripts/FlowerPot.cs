using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 25f;
    [SerializeField] private AudioClip potFallSound;
    [SerializeField] private GameObject dirtParticles;
    [SerializeField] private Sprite brokenPotSprite;

    private bool isActivated = false;
    private HumanController humanController;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private static List<GameObject> hiddenObjects = new List<GameObject>();
    private Canvas mainCanvas;
    private Camera mainCamera;
    private Vector3 mainCameraPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        humanController = FindObjectOfType<HumanController>();


        mainCanvas = FindObjectOfType<Canvas>();
        mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCameraPosition = mainCamera.transform.position;
        }

        if (dirtParticles != null)
            dirtParticles.SetActive(false);
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
        while (parent != null)
        {
            potParents.Add(parent.gameObject);
            parent = parent.parent;
        }

        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj == gameObject ||
                potParents.Contains(obj) ||
                obj.GetComponent<OvenData>() != null ||
                obj.GetComponent<FlowerPotData>() != null ||
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


        if (mainCanvas != null)
            mainCanvas.gameObject.SetActive(false);

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);
    }

    private void ShowMainScene()
    {

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            mainCamera.transform.position = mainCameraPosition;
        }

        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        hiddenObjects.Clear();


        if (mainCanvas != null)
            mainCanvas.gameObject.SetActive(true);

        Time.timeScale = 1;


        if (humanController != null)
            humanController.ResetMovementState();
    }

    private void OnMinigameSceneUnloaded(Scene scene)
    {
        if (scene.name == "FlowerPotMiniGame")
        {
            ShowMainScene();

            if (FlowerPotData.wasCompleted)
            {
                CompletePrank();
            }
        }
    }

    private void CompletePrank()
    {
        isActivated = true;

        if (dirtParticles != null)
            dirtParticles.SetActive(true);

        if (spriteRenderer != null && brokenPotSprite != null)
            spriteRenderer.sprite = brokenPotSprite;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (potFallSound != null)
            audioSource.PlayOneShot(potFallSound);

        if (humanController != null)
        {
            humanController.AddAnnoyance(annoyanceAmount);
            humanController.ReactToPrank(transform.position, 2f);
        }
        if (QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest("FlowerPot");
    }
}