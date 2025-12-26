using UnityEngine;

public class MeleeAttack : Attack
{
    public void Start()
    {
        // VFX/SFX 소환

        Destroy(gameObject, 1f);
    }

    //[Header("Skill")]
    //[SerializeField] private PushSkillData pushData;

    // 아마 GPT가 불릿의 컨피규어 메서드를 다시 넣은 듯.
    //public void Activate(Creature ownerCreature, PushSkillData data)
    //{
    //    if (ownerCreature == null || data == null) return;

    //    //SetOwner(ownerCreature);
    //    pushData = data;

    //    damage = data.damage;
    //    knockbackForce = data.knockbackForce;
    //    stunDuration = data.stunDuration;

    //    knockbackOrigin = (ownerCreature.KnockbackOrigin != null)
    //        ? ownerCreature.KnockbackOrigin
    //        : ownerCreature.transform;

    //    DoPushOnce();
    //}

    //private void DoPushOnce()
    //{
    //    Debug.Assert(pushData != null, "PushData is null in MeleeAttack.DoPushOnce");
    //    Debug.Assert(knockbackOrigin != null, "KnockbackOrigin is null in MeleeAttack.DoPushOnce");

    //    if (pushData == null) return;
    //    //if (pushData == null || owner == null) return;

    //    Vector3 center = knockbackOrigin.position;
    //    //Vector3 center = (knockbackOrigin != null) ? knockbackOrigin.position : owner.transform.position;
    //    float radius = pushData.range;

    //    Collider[] hits = Physics.OverlapSphere(center, radius);
    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        Creature target = hits[i].GetComponentInParent<Creature>();
    //        if (target == null) continue;

    //        //ApplyHit(target);
    //    }
    //}

    protected override void OnHit(Creature target)
    {
        // 필요하면 VFX/SFX 훅만 둬라.
    }
}
