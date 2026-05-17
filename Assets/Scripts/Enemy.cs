using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private float speed = 3f;

    [Header("Коллайдеры атак")]
    [SerializeField] private GameObject lightAttackCollider;
    [SerializeField] private GameObject heavyAttackCollider;

    private Transform player;

    private bool isAttacking;

    private float lastLightAttackTime;
    private float lastHeavyAttackTime;

    private const float LightAttackCooldown = 0.5f;
    private const float HeavyAttackCooldown = 2f;

    private GameObject playerObj;

    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();

        playerObj = GameObject.FindGameObjectWithTag("Player");

        FindPlayer();

        if (player != null)
        {
            FacePlayer(Mathf.Sign(player.position.x - transform.position.x));
        }
    }

    private void FindPlayer()
    {
        if (playerObj)
        {
            player = playerObj.transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (GameManager.Instance != null && !GameManager.Instance.CanFight)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            animator.SetBool("IsWalking", false);

            return;
        }

        if (IsBeingKnockedBack)
        {
            TryAttack();
            return;
        }

        if (IsStunned)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            animator.SetBool("IsWalking", false);

            return;
        }

        if (!player)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");

            FindPlayer();

            if (!player)
                return;
        }

        MoveToPlayer();

        TryAttack();
    }

    private void MoveToPlayer()
    {
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            animator.SetBool("IsWalking", false);

            return;
        }

        var directionX = Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);

        animator.SetBool("IsWalking", true);

        FacePlayer(directionX);
    }

    private void FacePlayer(float directionX)
    {
        if (Mathf.Abs(player.position.x - transform.position.x) > 0.1f)
        {
            float scaleX = directionX > 0 ? 1f : -1f;

            transform.localScale = new Vector3(scaleX, 1f, 1f);
        }
    }

    private void TryAttack()
    {
        var distance = Mathf.Abs(player.position.x - transform.position.x);

        if (distance < 1.2f)
        {
            var randomAttack = Random.Range(0, 2);

            if (randomAttack == 0)
            {
                if (isAttacking || Time.time < lastLightAttackTime + LightAttackCooldown)
                    return;

                animator.SetTrigger("LightAttack");

                StartCoroutine(AttackCoroutine(lightAttackCollider, 0.15f, true));
            }
            else
            {
                if (isAttacking || Time.time < lastHeavyAttackTime + HeavyAttackCooldown)
                    return;

                animator.SetTrigger("HeavyAttack");

                StartCoroutine(AttackCoroutine(heavyAttackCollider, 0.3f, false));
            }
        }
    }

    private IEnumerator AttackCoroutine(GameObject attackCollider, float duration, bool isLight)
    {
        isAttacking = true;

        if (isLight)
        {
            lastLightAttackTime = Time.time;
        }
        else
        {
            lastHeavyAttackTime = Time.time;
        }

        attackCollider.SetActive(true);

        yield return new WaitForSeconds(duration);

        attackCollider.SetActive(false);

        isAttacking = false;
    }
}