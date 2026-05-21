using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 3f;
    public float idleSpeed = 0.1f;
    public float walkSpeed = 1f;

    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private float climbHeight = 0.6f;
    [SerializeField] private float interactionCooldown = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 defaultScale;

    private bool isOnTaburet = false;
    private bool isOnPodosokonnik = false;

    private float lastInteractionTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        defaultScale = transform.localScale;
    }

    void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Q) && (isOnTaburet || isOnPodosokonnik))
        {
            ClimbDown();
        }
    }

    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Oven"))
            {
                Oven oven = hit.GetComponent<Oven>();
                if (oven != null)
                {
                    oven.StartMinigame();
                    return;
                }
            }

            if (hit.CompareTag("Taburet") && !isOnTaburet && !isOnPodosokonnik)
            {
                ClimbOn(hit.transform);
                return;
            }

            if (hit.CompareTag("Podosokonnik") && isOnTaburet && !isOnPodosokonnik)
            {
                ClimbOn(hit.transform);
                return;
            }

            if (hit.CompareTag("FlowerPot") && isOnPodosokonnik)
            {
                FlowerPot pot = hit.GetComponent<FlowerPot>();
                if (pot != null)
                {
                    pot.StartMinigame();
                    return;
                }
            }
        }
    }

    private void ClimbOn(Transform target)
    {
        Collider2D targetCollider = target.GetComponent<Collider2D>();
        float targetTop = target.position.y;

        if (targetCollider != null)
        {
            Bounds bounds = targetCollider.bounds;
            targetTop = bounds.max.y;
        }

        transform.position = new Vector3(
            target.position.x,
            targetTop + climbHeight,
            transform.position.z
        );

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}