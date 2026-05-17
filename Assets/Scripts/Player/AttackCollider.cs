using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private float damage = 2f;
    [SerializeField] private float knockbackForce;
    [SerializeField] private Entity owner;

    private bool hasHit;
    
    private void OnEnable()
    {
        hasHit = false;

        var rb = GetComponentInParent<Rigidbody2D>();
        if (rb != null)
        {
            rb.WakeUp();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ProcessCollision(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other);
    }

    private void ProcessCollision(Collider2D other)
    {
        if (hasHit) return;

        var entity = other.GetComponent<Entity>();

        if (!entity)
            return;

        if (entity == owner)
            return;

        if (entity.TeamId == owner.TeamId)
            return;

        var otherRb = other.GetComponent<Rigidbody2D>();
        if (otherRb != null)
        {
            otherRb.WakeUp();
        }

        hasHit = true;
        entity.TakeDamage(damage, owner);

        if (knockbackForce > 0)
        {
            Vector2 direction = (entity.transform.position - owner.transform.position).normalized;
            direction.y = 0; 
            entity.ApplyKnockback(direction.normalized * knockbackForce);
        }
    }
}