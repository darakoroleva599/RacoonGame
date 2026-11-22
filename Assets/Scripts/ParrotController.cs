using UnityEngine;

public class ParrotController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float visionRadius = 3f;
    [SerializeField] private LayerMask detectionLayers;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private HumanController humanController;
    [SerializeField] private Transform cageCheckPoint;

    private Transform raccoon;
    private bool raccoonDetected;
    private bool cageCovered;


    private static bool cageCoveredSaved;

    public static bool IsCageCovered => cageCoveredSaved;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            raccoon = player.transform;


        if (cageCoveredSaved)
        {
            cageCovered = true;
            raccoonDetected = false;
        }
    }

    private void Update()
    {
        if (cageCovered || raccoon == null)
            return;

        float distance = Vector2.Distance(transform.position, raccoon.position);

        if (distance <= visionRadius)
        {
            if (!raccoonDetected)
            {
                raccoonDetected = true;
                Scream();
            }
        }
        else
        {
            raccoonDetected = false;
        }
    }

    private void Scream()
    {
        if (animator != null)
            animator.SetTrigger("Scream");

        if (humanController != null && cageCheckPoint != null)
            humanController.ReactToParrot(cageCheckPoint);
    }

    public void CoverCage()
    {
        cageCovered = true;
        cageCoveredSaved = true;
        raccoonDetected = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}