using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float speed = 3f;
    public float idleSpeed = 0.1f;
    public float walkSpeed = 1f;

    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 1.0f;
    [SerializeField] private float climbHeight = 0.6f;
    [SerializeField] private float interactionCooldown = 1f;
    [SerializeField] private float climbDuration = 0.5f;
    [SerializeField] private float hidingRadius = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 defaultScale;
    private static Vector3 bookshelfReturnPosition;
    private static bool hasBookshelfReturnPosition = false;

    private bool isOnTaburet = false;
    private bool isOnPodosokonnik = false;
    private bool isCrawling = false;
    private bool isHidden = false;
    public bool IsCrawling => isCrawling;


    public bool IsOnTaburet => isOnTaburet;
    public bool IsOnPodosokonnik => isOnPodosokonnik;
    public bool IsHidden => isHidden;

    private float lastInteractionTime = 0f;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer pepperRenderer;
    private bool pepperWasVisibleBeforeHiding;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        defaultScale = transform.localScale;

        if (hasBookshelfReturnPosition)
        {
            transform.position = bookshelfReturnPosition;
            rb.position = new Vector2(
                bookshelfReturnPosition.x,
                bookshelfReturnPosition.y
            );

            rb.linearVelocity = Vector2.zero;

            hasBookshelfReturnPosition = false;
        }
    }

    void Update()
    {
        if (isCrawling) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(x, y);
        rb.linearVelocity = move.normalized * speed;

        bool isMoving = move.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
            animator.speed = walkSpeed;
        else
            animator.speed = idleSpeed;

        if (x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);
        else if (x > 0)
            transform.localScale = new Vector3(Mathf.Abs(defaultScale.x), defaultScale.y, defaultScale.z);

        if (Input.GetKeyDown(KeyCode.E) && Time.realtimeSinceStartup - lastInteractionTime > interactionCooldown)
        {
            lastInteractionTime = Time.realtimeSinceStartup;
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isCrawling)
        {
            TryClimb();
        }

        if (Input.GetKeyDown(KeyCode.S) && (isOnTaburet || isOnPodosokonnik) && !isCrawling)
        {
            ClimbDown();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleHide();
        }
    }

    private void ToggleHide()
    {
        if (isHidden)
        {
            isHidden = false;

            spriteRenderer.enabled = true;
            GetComponent<Collider2D>().enabled = true;
            gameObject.tag = "Player";

            if (pepperRenderer != null)
                pepperRenderer.enabled = pepperWasVisibleBeforeHiding;
        }
        else
        {
            if (IsNearHidingSpot())
            {
                isHidden = true;

                if (pepperRenderer != null)
                {
                    pepperWasVisibleBeforeHiding = pepperRenderer.enabled;
                    pepperRenderer.enabled = false;
                }

                spriteRenderer.enabled = false;
                GetComponent<Collider2D>().enabled = false;
                gameObject.tag = "Hidden";
            }
        }
    }

    private bool IsNearHidingSpot()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hidingRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("HidingSpot"))
            {
                return true;
            }
        }
        return false;
    }

    private void TryInteract()
    {
        if (isCrawling)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Oven"))
            {
                if (QuestManager.Instance == null ||
                    !QuestManager.Instance.CanStartQuest("Oven"))
                    return;

                Oven oven = hit.GetComponent<Oven>();
                if (oven != null)
                {
                    oven.StartMinigame();
                    return;
                }
            }

            if (hit.CompareTag("FlowerPot") && isOnPodosokonnik)
            {
                if (QuestManager.Instance == null ||
                    !QuestManager.Instance.CanStartQuest("FlowerPot"))
                    return;

                FlowerPot pot = hit.GetComponent<FlowerPot>();
                if (pot != null)
                {
                    pot.StartMinigame();
                    return;
                }
            }

            if (hit.CompareTag("Fridge"))
            {
                if (QuestManager.Instance == null ||
                    !QuestManager.Instance.CanStartQuest("Fridge"))
                    return;

                Fridge fridge = hit.GetComponent<Fridge>();
                if (fridge != null)
                {
                    fridge.StartMinigame();
                    return;
                }
            }

            if (hit.CompareTag("Bookshelf"))
            {
                bookshelfReturnPosition = transform.position;
                hasBookshelfReturnPosition = true;

                SceneManager.LoadScene("BookshelfPuzzle");
                return;
            }

            if (hit.CompareTag("Pie") &&
    Vector2.Distance(transform.position, hit.transform.position) < 1.2f)
            {
                if (QuestManager.Instance == null ||
                    !QuestManager.Instance.CanStartQuest("Pie"))
                    return;

                Pie pie = hit.GetComponent<Pie>();
                if (pie != null)
                {
                    pie.StartMinigame();
                    return;
                }
            }

            if (hit.CompareTag("DoorDistraction") ||
    hit.CompareTag("LampDistraction") ||
    hit.CompareTag("PhoneDistraction") ||
    hit.CompareTag("TVDistraction"))
            {
                if (isOnPodosokonnik)
                    continue;

                DistractionObject distraction = hit.GetComponent<DistractionObject>();

                if (distraction != null)
                {
                    distraction.Activate();
                    return;
                }
            }
        }
    }

    private void TryClimb()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Taburet") && !isOnTaburet && !isOnPodosokonnik)
            {
                StartCoroutine(ClimbRoutine(hit.transform));
                return;
            }

            if (hit.CompareTag("Podosokonnik") && isOnTaburet && !isOnPodosokonnik)
            {
                StartCoroutine(ClimbRoutine(hit.transform));
                return;
            }
        }
    }

    private IEnumerator ClimbRoutine(Transform target)
    {
        isCrawling = true;

        animator.speed = 1f;

        bool pepperWasVisible = pepperRenderer != null && pepperRenderer.enabled;

        if (pepperRenderer != null)
            pepperRenderer.enabled = false;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        animator.SetBool("isCrawling", true);

        Collider2D targetCollider = target.GetComponent<Collider2D>();

        float targetTop = target.position.y;

        if (targetCollider != null)
        {
            Bounds bounds = targetCollider.bounds;
            targetTop = bounds.max.y;
        }

        Vector2 startPosition = rb.position;

        Vector2 endPosition = new Vector2(
            target.position.x,
            targetTop + climbHeight
        );

        float elapsed = 0f;

        while (elapsed < climbDuration)
        {
            elapsed += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(elapsed / climbDuration);

            Vector2 newPosition = Vector2.Lerp(
                startPosition,
                endPosition,
                t
            );

            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }

        rb.position = endPosition;
        rb.linearVelocity = Vector2.zero;

        if (target.CompareTag("Taburet"))
        {
            isOnTaburet = true;
            isOnPodosokonnik = false;
        }
        else if (target.CompareTag("Podosokonnik"))
        {
            isOnTaburet = false;
            isOnPodosokonnik = true;
        }

        animator.SetBool("isCrawling", false);

        isCrawling = false;
        if (pepperRenderer != null)
            pepperRenderer.enabled = pepperWasVisible;
    }

    private void ClimbDown()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y - 1.5f,
            transform.position.z
        );

        isOnTaburet = false;
        isOnPodosokonnik = false;
    }

    public void RestoreState(
        Vector3 savedPosition,
        bool savedOnTaburet,
        bool savedOnPodosokonnik,
        bool savedHidden)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        transform.position = savedPosition;

        if (rb != null)
        {
            rb.position = new Vector2(savedPosition.x, savedPosition.y);
            rb.linearVelocity = Vector2.zero;
        }

        isOnTaburet = savedOnTaburet;
        isOnPodosokonnik = savedOnPodosokonnik;

        isCrawling = false;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.SetBool("isCrawling", false);

        isHidden = savedHidden;

        if (spriteRenderer != null)
            spriteRenderer.enabled = !isHidden;

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
            playerCollider.enabled = !isHidden;

        gameObject.tag = isHidden ? "Hidden" : "Player";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hidingRadius);
    }
}