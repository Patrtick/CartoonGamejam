using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [Header("Коллайдеры атак")]
    [SerializeField] private GameObject lightAttackCollider;
    [SerializeField] private GameObject heavyAttackCollider;

    [Header("Блок")]
    [SerializeField] private float blockDamageMultiplier = 0.5f;
    [SerializeField] private float blockSpeedMultiplier = 0f;

    private bool isAttacking;
    private bool isBlocking;

    private float lastLightAttackTime;
    private float lastHeavyAttackTime;

    private const float LightAttackCooldown = 0.5f;
    private const float HeavyAttackCooldown = 2f;

    public bool IsBlocking => isBlocking;
    public float BlockSpeedMultiplier => blockSpeedMultiplier;

    public void SetBlocking(bool blocking)
    {
        isBlocking = blocking;

        animator.SetBool("IsBlocking", blocking);
    }

    protected override void Stun()
    {
        base.Stun();
        SetBlocking(false);
    }

    public override void TakeDamage(float damage, Entity attacker = null, bool ignoreStun = false)
    {
        if (isBlocking)
        {
            damage *= blockDamageMultiplier;
            ignoreStun = true;
        }

        base.TakeDamage(damage, attacker, ignoreStun);
    }

    public void LightAttack()
    {
        if (isAttacking || isBlocking || Time.time < lastLightAttackTime + LightAttackCooldown)
            return;

        animator.SetTrigger("LightAttack");

        StartCoroutine(AttackCoroutine(lightAttackCollider, 0.15f, true));
    }

    public void HeavyAttack()
    {
        if (isAttacking || isBlocking || Time.time < lastHeavyAttackTime + HeavyAttackCooldown)
            return;

        animator.SetTrigger("HeavyAttack");

        StartCoroutine(AttackCoroutine(heavyAttackCollider, 0.3f, false));
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