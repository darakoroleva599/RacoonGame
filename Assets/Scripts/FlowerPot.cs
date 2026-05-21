using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowerPot : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 25f;
    [SerializeField] private AudioClip potFallSound;
    [SerializeField] private GameObject dirtParticles;

    private bool isActivated = false;
    private HumanController humanController;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        humanController = FindObjectOfType<HumanController>();

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

        SceneManager.LoadScene("FlowerPotMinigame", LoadSceneMode.Additive);
    }

    private void OnMinigameSceneUnloaded(Scene scene)
    {
        if (scene.name == "FlowerPotMinigame")
        {
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

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

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
    }
}