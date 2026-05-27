using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HumanController : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitAtWaypoint = 1f;

    [Header("Vision Settings")]
    [SerializeField] private float visionRadius = 3f;
    [SerializeField, Range(0, 360)] private float visionAngle = 90f;

    [Header("Annoyance System")]
    [SerializeField] private float maxAnnoyance = 100f;
    [SerializeField] private float currentAnnoyance = 0f;
    [SerializeField] private UnityEngine.UI.Slider annoyanceBar;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 defaultScale;
    private int currentWaypointIndex = 0;
    private bool isReacting = false;
    private Transform raccoon;
    private Coroutine patrolCoroutine;

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

        UpdateAnnoyanceUI();

        if (waypoints.Length > 0)
            patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (CanSeeRaccoon())
        {
            CatchRaccoon();
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (!isReacting && waypoints.Length > 0)
            {
                Transform target = waypoints[currentWaypointIndex];
                yield return StartCoroutine(MoveToTarget(target.position));

                animator.SetBool("isMoving", false);
                yield return new WaitForSeconds(waitAtWaypoint);

                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            yield return null;
        }
    }

    private IEnumerator MoveToTarget(Vector3 target)
    {
        animator.SetBool("isMoving", true);

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            if (isReacting)
            {
                animator.SetBool("isMoving", false);
                rb.linearVelocity = Vector2.zero;
                yield break;
            }

            Vector3 direction = (target - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);
            else if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    private bool CanSeeRaccoon()
    {
        if (raccoon == null) return false;

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
        currentAnnoyance = Mathf.Min(currentAnnoyance + amount, maxAnnoyance);
        UpdateAnnoyanceUI();

        if (currentAnnoyance >= maxAnnoyance)
        {
            StartCoroutine(LeaveRoom());
        }
    }

    private void UpdateAnnoyanceUI()
    {
        if (annoyanceBar != null)
            annoyanceBar.value = currentAnnoyance / maxAnnoyance;
    }

    public void ReactToPrank(Vector3 prankLocation, float reactionTime = 3f)
    {
        StopAllCoroutines();
        StartCoroutine(ReactionRoutine(prankLocation, reactionTime));
    }

    private IEnumerator ReactionRoutine(Vector3 targetLocation, float duration)
    {
        isReacting = true;
        yield return StartCoroutine(MoveToTarget(targetLocation));
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(duration);
        isReacting = false;

        if (patrolCoroutine != null)
            StopCoroutine(patrolCoroutine);
        if (waypoints.Length > 0)
            patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    private IEnumerator LeaveRoom()
    {
        isReacting = true;
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(2f);
    }

    private void CatchRaccoon()
    {
        if (Time.timeScale == 0) return;

        Time.timeScale = 0;
        OnCaughtRaccoon?.Invoke();
    }

    public float GetAnnoyanceProgress()
    {
        return currentAnnoyance / maxAnnoyance;
    }

    public bool IsReacting()
    {
        return isReacting;
    }

    public void ResetMovementState()
    {
        StopAllCoroutines();

        isReacting = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("isMoving", false);

        if (waypoints != null && waypoints.Length > 0)
            patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

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