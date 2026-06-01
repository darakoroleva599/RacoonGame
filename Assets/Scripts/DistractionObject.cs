using UnityEngine;
using System.Collections;

public class DistractionObject : MonoBehaviour
{
    [SerializeField] private float annoyanceAmount = 10f;
    [SerializeField] private float distractionDuration = 3f;
    [SerializeField] private Transform[] pathPoints;

    private SpriteRenderer distractionSprite;
    private bool isActive = false;
    private HumanController humanController;
    private float timer = 0f;
    private bool countingDown = false;

    void Awake()
    {
        distractionSprite = GetComponent<SpriteRenderer>();
        if (distractionSprite != null)
            distractionSprite.enabled = false;

        humanController = FindObjectOfType<HumanController>();
    }

    void Update()
    {
        if (!countingDown) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            countingDown = false;
            Deactivate();
        }
    }

    public void Activate()
    {
        if (isActive) return;
        isActive = true;
        countingDown = true;
        timer = distractionDuration + 2f;

        if (distractionSprite != null)
            distractionSprite.enabled = true;

        if (humanController != null)
            humanController.StartDistraction(pathPoints, distractionDuration);
    }

    private void Deactivate()
    {
        if (distractionSprite != null)
            distractionSprite.enabled = false;

        if (humanController != null)
            humanController.AddAnnoyance(annoyanceAmount);

        isActive = false;

        if (QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest(gameObject.tag);
    }
}