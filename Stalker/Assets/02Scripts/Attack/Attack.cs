using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected int damage;

    [SerializeField] protected float knockbackForce;
    [SerializeField] protected Transform knockbackOrigin;

    [SerializeField] protected float stunDuration;
    [SerializeField] protected CreatureType immuneTargetType;

    protected Creature owner;

    public void SetOwner(Creature o) { owner = o; }

    protected bool CanHit(Creature target)
    {
        if (target == null) return false;
        if (target.Type == immuneTargetType) return false;
        if (owner != null && target.Type == owner.Type) return false; // 아군 판정
        if (target.IsDead) return false;
        return true;
    }
    protected void ApplyHit(Creature target)
    {
        if (!CanHit(target)) return;

        // 1) 데미지
        if (damage != 0)
            target.TakeDamage(damage);

        // 2) 넉백
        if (knockbackForce > 0f)
            ApplyKnockback(target);

        // 3) 스턴
        if (stunDuration > 0f)
            target.ApplyStun(stunDuration);

        // 4) 파생 처리(총알 파괴, 이펙트 등)
        OnHit(target);
    }

    protected virtual void ApplyKnockback(Creature target)
    {
        Rigidbody rb = target.Rigidbody;
        if (rb == null) return;

        Vector3 originPos = (knockbackOrigin != null) ? knockbackOrigin.position : transform.position;

        Vector3 dir = (target.transform.position - originPos);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        rb.AddForce(dir.normalized * knockbackForce, ForceMode.Impulse);
    }

    protected abstract void OnHit(Creature target);
}
