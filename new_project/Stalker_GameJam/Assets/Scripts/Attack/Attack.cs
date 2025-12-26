using UnityEngine;

/// <summary>
///     모든 공격 범위 게임오브젝트에 대한 서술
/// </summary>
public abstract class Attack : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected int damage;
    [SerializeField] protected float knockbackForce;
    [SerializeField] protected Transform knockbackOrigin;
    [SerializeField] protected float stunDuration;
    [SerializeField] protected CreatureType immuneTargetType;

    // Bullet 메서드의 컨피규어 메서드와 비슷함
    public void Configure(
        int _damage,
        float _knockbackForce,
        Transform _knockbackOrigin,
        float _stunDuration,
        CreatureType _immuneTargetType)
    {
        this.damage = _damage;
        this.knockbackForce = _knockbackForce;
        this.knockbackOrigin = _knockbackOrigin;
        this.stunDuration = _stunDuration;
        this.immuneTargetType = _immuneTargetType;
    }
    public void Configure(
        PushSkillData data,
        Creature creature)
    {
        this.damage = data.damage;
        this.knockbackForce = data.knockbackForce;
        this.knockbackOrigin = creature.transform;
        this.stunDuration = data.stunDuration;
        this.immuneTargetType = creature.Type;
    }


    //protected Creature owner;

    //public void SetOwner(Creature o) { owner = o; }

    protected bool CanHit(Creature target)
    {
        if (target == null) return false;
        if (target.Type == immuneTargetType) return false;
        //if (owner != null && target.Type == owner.Type) return false; // 아군 판정
        if (target.IsDead) return false;
        return true;
    }

    // KR: 결론. 모든 공격 반영 로직은 이 부모 클래스에서 처리한다.
    // EN: In conclusion, all attack application logic is handled in this parent class.
    // KR: 근거 1) 자식 공격 오브젝트들은 전부 충돌체를 가짐. 그리고 충돌 시 처리하는 내용은 "면역 판정, 데미지 반영, 넉백 반영"을 다 똑같이 처리하기 때문. 바뀌는것은 해당 함수 내 변수의 값일 뿐 로직은 바뀌지 않아서 다형성, 가상함수, 함수 오버라이드도 필요하지 않음.
    // EN: Rationale 1) All child attack objects have colliders. And the content processed upon collision is the same: "immunity check, damage application, knockback application". The only thing that changes are the values of the variables within that function, so polymorphism, virtual functions, and function overrides are not necessary.
    private void OnTriggerEnter(Collider collider)
    //protected void ApplyHit(Creature target)
    {
        Creature target = collider.GetComponentInParent<Creature>();
        if (target == null) return;

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

    protected virtual void OnHit(Creature target) { }
}
