using UnityEngine;

public class MeleeAttack : Attack
{
    [Header("Skill")]
    [SerializeField] private PushSkillData pushData;

    public void Activate(Creature ownerCreature, PushSkillData data)
    {
        if (ownerCreature == null || data == null) return;

        SetOwner(ownerCreature);
        pushData = data;

        damage = data.damage;
        knockbackForce = data.knockbackForce;
        stunDuration = data.stunDuration;

        knockbackOrigin = (ownerCreature.KnockbackOrigin != null)
            ? ownerCreature.KnockbackOrigin
            : ownerCreature.transform;

        DoPushOnce();
    }

    private void DoPushOnce()
    {
        if (pushData == null || owner == null) return;

        Vector3 center = (knockbackOrigin != null) ? knockbackOrigin.position : owner.transform.position;
        float radius = pushData.range;

        Collider[] hits = Physics.OverlapSphere(center, radius);
        for (int i = 0; i < hits.Length; i++)
        {
            Creature target = hits[i].GetComponentInParent<Creature>();
            if (target == null) continue;

            ApplyHit(target);
        }
    }

    protected override void OnHit(Creature target)
    {
        // « ø‰«œ∏È VFX/SFX »≈∏∏ µ÷∂Û.
    }
}
