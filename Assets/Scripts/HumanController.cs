using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HumanController : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitAtWaypoint = 1f;

    [Header("Vision Settings")]
    [SerializeField] private float visionRadius = 3f;
    [SerializeField, Range(0, 360)] private float visionAngle = 90f;

    [Header("Detection Settings")]
    [SerializeField] private GameObject eyeIndicator;
    [SerializeField] private Image detectionBar;
    [SerializeField] private float detectionTime = 3f;
    [SerializeField] private float decaySpeed = 1.5f;

    [Header("Annoyance System")]
    [SerializeField] private float maxAnnoyance = 100f;
    [SerializeField] private float currentAnnoyance = 0f;
    [SerializeField] private Slider annoyanceBar;

    [Header("Distraction")]
    [SerializeField] private GameObject exclamationMark;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 defaultScale;
    private int currentWaypointIndex = 0;
    private bool isReacting = false;
    private Transform raccoon;

    private float waitTimer = 0f;
    private bool isWaiting = false;

    private bool isDistracted = false;
    private float distractionTimer = 0f;
    private Transform[] distractionPath;
    private int distractionPathIndex = 0;

    private float detectionProgress = 0f;
    private bool isDetecting = false;

    public System.Action OnCaughtRaccoon;

    void Start()
    {
        Time.timeScale = 1;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        defaultScale = transform.localScale;

        raccoon = GameObject.FindGameObjectWithTag("Player").transform;

        if (eyeIndicator != null) eyeIndicator.SetActive(false);
        if (detectionBar != null) detectionBar.gameObject.SetActive(false);
        if (exclamationMark != null) exclamationMark.SetActive(false);

        UpdateAnnoyanceUI();
    }

    void Update()
    {
        HandleDetection();

        if (isReacting)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            return;
        }

        if (isDistracted)
        {
            HandleDistraction();
            return;
        }

        HandlePatrol();
    }

    private void HandlePatrol()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        if (isWaiting)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > 0.1f)
        {
            MoveTowards(target.position);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            isWaiting = true;
            waitTimer = waitAtWaypoint;
        }
    }

    private void HandleDistraction()
    {
        if (distractionPathIndex < distractionPath.Length)
        {
            Transform target = distractionPath[distractionPathIndex];
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > 0.1f)
            {
                MoveTowards(target.position);
            }
            else
            {
                if (distractionPathIndex == 0 && distractionTimer > 100f)
                {
                    isDistracted = false;
                    if (exclamationMark != null) exclamationMark.SetActive(false);
                    return;
                }
                distractionPathIndex++;
            }
            return;
        }

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);

        distractionTimer -= Time.deltaTime;
        if (distractionTimer <= 0f)
        {
            distractionPathIndex = 0;
            distractionTimer = 999f;
        }
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        animator.SetBool("isMoving", true);

        if (direction.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);
        else if (direction.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);
    }

    public void StartDistraction(Transform[] path, float duration)
    {
        isDistracted = true;
        distractionPath = path;
        distractionPathIndex = 0;
        distractionTimer = duration;
        isWaiting = false;

        if (exclamationMark != null) exclamationMark.SetActive(true);
    }

    private void HandleDetection()
    {
        if (CanSeeRaccoon())
        {
            if (!isDetecting)
            {
                isDetecting = true;
                if (eyeIndicator != null) eyeIndicator.SetActive(true);
                if (detectionBar != null) detectionBar.gameObject.SetActive(true);
            }

            detectionProgress += Time.deltaTime;
            detectionProgress = Mathf.Clamp(detectionProgress, 0f, detectionTime);
            if (detectionBar != null) detectionBar.fillAmount = detectionProgress / detectionTime;

            if (detectionProgress >= detectionTime) CatchRaccoon();
        }
        else
        {
            if (isDetecting)
            {
                detectionProgress -= Time.deltaTime * decaySpeed;
                detectionProgress = Mathf.Clamp(detectionProgress, 0f, detectionTime);
                if (detectionBar != null) detectionBar.fillAmount = detectionProgress / detectionTime;

                if (detectionProgress <= 0f)
                {
                    isDetecting = false;
                    if (eyeIndicator != null) eyeIndicator.SetActive(false);
                    if (detectionBar != null) detectionBar.gameObject.SetActive(false);
                }
            }
        }
    }

    private bool CanSeeRaccoon()
    {
        if (raccoon == null) return false;
        if (raccoon.CompareTag("Hidden")) return false;

        Vector3 directionToRaccoon = raccoon.position - transform.position;
        float distance = directionToRaccoon.magnitude;
        if (distance > visionRadius) return false;

        Vector3 facingDirection = Vector3.down;
        float angle = Vector3.Angle(facingDirection, directionToRaccoon);
        if (angle > visionAngle / 2) return false;

        return true;
    }

    public void AddAnnoyance(float amount)
    {
        currentAnnoyance += amount;
        currentAnnoyance = Mathf.Min(currentAnnoyance, maxAnnoyance);
        UpdateAnnoyanceUI();
        if (currentAnnoyance >= maxAnnoyance) isReacting = true;
    }

    private void UpdateAnnoyanceUI()
    {
        if (annoyanceBar != null) annoyanceBar.value = currentAnnoyance / maxAnnoyance;
    }

    public void ReactToPrank(Vector3 prankLocation, float reactionTime = 3f)
    {
        isReacting = true;
        Invoke("StopReacting", reactionTime);
    }

    private void StopReacting()
    {
        isReacting = false;
    }

    private void CatchRaccoon()
    {
        if (Time.timeScale == 0) return;
        Time.timeScale = 0;
        OnCaughtRaccoon?.Invoke();
    }

    public float GetAnnoyanceProgress() { return currentAnnoyance / maxAnnoyance; }
    public bool IsReacting() { return isReacting; }

    public static void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 direction = Vector3.down;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, visionAngle / 2) * direction;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -visionAngle / 2) * direction;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionRadius);
    }
}