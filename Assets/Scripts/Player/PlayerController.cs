using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private PlayerControls controls;

    private Rigidbody2D rb;
    private Player player;
    private Transform opponent;
    private Animator animator;

    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        FindOpponent();
        FaceOpponent();
    }

    private void Update()
    {
        if (GameManager.Instance && !GameManager.Instance.CanFight)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            animator.SetBool("IsWalking", false);

            return;
        }

        if (player.IsBeingKnockedBack)
        {
            AttackInput();
            return;
        }

        if (player.IsStunned)
        {
            AttackInput();

            animator.SetBool("IsWalking", false);

            return;
        }

        if (!opponent)
        {
            FindOpponent();
        }

        Move();

        AttackInput();

        BlockInput();

        FaceOpponent();
    }

    private void BlockInput()
    {
        if (Input.GetKey(controls.block))
        {
            player.SetBlocking(true);
        }
        else
        {
            player.SetBlocking(false);
        }
    }

    private void FindOpponent()
    {
        var entities = FindObjectsByType<Entity>(FindObjectsSortMode.None);

        foreach (var entity in entities)
        {
            if (entity.gameObject != gameObject)
            {
                opponent = entity.transform;
                break;
            }
        }
    }

    private void FaceOpponent()
    {
        if (!opponent)
        {
            return;
        }

        float direction = opponent.position.x - transform.position.x;

        if (Mathf.Abs(direction) > 0.1f)
        {
            float scaleX = direction > 0 ? 1f : -1f;

            transform.localScale = new Vector3(scaleX, 1f, 1f);
        }
    }

    private void Move()
    {
        moveInput = 0;

        if (player.IsBlocking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("IsWalking", false);
            return;
        }

        if (Input.GetKey(controls.left))
        {
            moveInput = -1;
        }

        if (Input.GetKey(controls.right))
        {
            moveInput = 1;
        }

        animator.SetBool("IsWalking", moveInput != 0);

        rb.linearVelocity = new Vector2(
            moveInput * speed,
            rb.linearVelocity.y
        );
    }

    private void AttackInput()
    {
        if (Input.GetKeyDown(controls.lightAttack))
        {
            player.LightAttack();
        }

        if (Input.GetKeyDown(controls.heavyAttack))
        {
            player.HeavyAttack();
        }
    }

    public void SetControls(
        KeyCode left,
        KeyCode right,
        KeyCode light,
        KeyCode heavy,
        KeyCode block)
    {
        controls.left = left;
        controls.right = right;

        controls.lightAttack = light;
        controls.heavyAttack = heavy;
        controls.block = block;
    }
}