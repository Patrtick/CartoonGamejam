using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    [Header("ХП")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] private Image healthBar;
    [SerializeField] private int teamId;

    private float currentHealth;
    protected Animator animator;

    public int TeamId => teamId;

    private int consecutiveHits;
    private Entity lastAttacker;
    private float stunTimeLeft;
    private float knockbackResistanceTime;
    private float lastHitTime;

    private const float HitWindow = 2f;

    public bool IsStunned { get; private set; }

    public bool IsBeingKnockedBack => knockbackResistanceTime > 0;

    public static event System.Action<Entity> OnEntityDeath;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();

        UpdateHealthBar();
    }

    protected virtual void Update()
    {
        if (IsStunned)
        {
            stunTimeLeft -= Time.deltaTime;

            if (stunTimeLeft <= 0)
            {
                IsStunned = false;
                consecutiveHits = 0;
            }
        }
        else if (consecutiveHits > 0 && Time.time > lastHitTime + HitWindow)
        {
            consecutiveHits = 0;
        }

        if (knockbackResistanceTime > 0)
        {
            knockbackResistanceTime -= Time.deltaTime;
        }
    }

    public void SetHealthBar(Image bar)
    {
        healthBar = bar;

        UpdateHealthBar();
    }

    public void SetTeam(int id)
    {
        teamId = id;
    }

    public virtual void TakeDamage(float damage, Entity attacker = null, bool ignoreStun = false)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth > 0)
        {
            animator.SetTrigger("Hit");
        }

        if (attacker != null && !ignoreStun)
        {
            if (attacker.lastAttacker == this)
            {
                attacker.ResetHits();
            }

            if (lastAttacker == attacker && Time.time <= lastHitTime + HitWindow)
            {
                consecutiveHits++;
            }
            else
            {
                lastAttacker = attacker;
                consecutiveHits = 1;
            }

            lastHitTime = Time.time;

            if (consecutiveHits >= 3)
            {
                Stun();
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Stun()
    {
        IsStunned = true;
        stunTimeLeft = 3f;
    }

    private void ResetHits()
    {
        consecutiveHits = 0;
        IsStunned = false;
    }

    public void ApplyKnockback(Vector2 force)
    {
        var rb = GetComponent<Rigidbody2D>();

        if (rb == null) return;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        knockbackResistanceTime = 0.4f;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    protected void Die()
    {
        animator.SetBool("IsDead", true);

        var col = GetComponent<Collider2D>();

        if (col)
        {
            col.enabled = false;
        }

        var rb = GetComponent<Rigidbody2D>();

        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1.2f);

        OnEntityDeath?.Invoke(this);

        Destroy(gameObject);
    }
}