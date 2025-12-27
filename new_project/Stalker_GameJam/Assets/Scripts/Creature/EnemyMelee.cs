using UnityEngine;

public class EnemyMelee : Creature
{
    private enum EnemyState { Search, Approach, Attack, Dead }

    [Header("AI Refs")]
    [SerializeField] private Transform target;

    [Header("AI Ranges")]
    [SerializeField] private float searchRange = 25f;
    [SerializeField] private float meleeRange = 1.8f;     // 근접 공격 사거리(XZ 기준)
    [SerializeField] private float attackStopBuffer = 0.1f; // 너무 딱 붙어서 떨림 방지용
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private float meleeSpawnForward = 0.8f;

    [Header("AI Search Rule")]
    [SerializeField] private float notFoundDieSeconds = 120f;

    [Header("Rotate")]
    [SerializeField] private float turnDegPerSec = 720f;

    [Header("Melee Attack")]
    [SerializeField] private GameObject meleePrefab;  // MeleeAttack가 붙어있는 프리팹
    [SerializeField] private float meleeCooldown = 1.0f;

    [SerializeField] private int meleeDamage = 5;
    [SerializeField] private float meleeKnockbackForce = 4f;
    [SerializeField] private float meleeStunDuration = 0f;

    [Tooltip("프리팹이 스스로 파괴되지 않으면 일정 시간 뒤 제거")]
    [SerializeField] private float meleePrefabLife = 0.2f;

    [Header("Reward")]
    [SerializeField] private int expReward = 1;

    private EnemyState state = EnemyState.Search;
    private float notFoundTimer = 0f;
    private Vector3 desiredMoveDir;

    private Vector3 aimFlatWS = Vector3.forward;
    private float nextMeleeTime = 0f;

    protected override void Update()
    {
        base.Update();
        if (state == EnemyState.Dead) return;

        if (IsDead)
        {
            EnterDead("HP <= 0");
            return;
        }

        bool hasTarget = AcquireTargetInSearchRange();
        if (!hasTarget)
        {
            notFoundTimer += Time.deltaTime;
            state = EnemyState.Search;

            if (notFoundTimer >= notFoundDieSeconds)
                EnterDead("Target Not Found (timeout)");

            desiredMoveDir = Vector3.zero;
            return;
        }

        notFoundTimer = 0f;

        float dist = GetTargetDistanceXZ();
        float atkRange = Mathf.Max(0.1f, meleeRange);

        state = (dist <= atkRange) ? EnemyState.Attack : EnemyState.Approach;

        // 항상 타겟을 바라보도록 회전 목표 갱신 (XZ 평면)
        aimFlatWS = GetDirToTargetXZ();
        if (aimFlatWS.sqrMagnitude < 0.0001f) aimFlatWS = transform.forward;

        switch (state)
        {
            case EnemyState.Search:
                desiredMoveDir = Vector3.zero;
                break;

            case EnemyState.Approach:
                {
                    // 너무 가까우면 멈춰서 떨림 방지
                    if (dist <= atkRange + attackStopBuffer)
                        desiredMoveDir = Vector3.zero;
                    else
                        desiredMoveDir = aimFlatWS;

                    break;
                }

            case EnemyState.Attack:
                desiredMoveDir = Vector3.zero;

                // 쿨다운이면 근접 공격 발동
                TryMeleeAttack();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (state == EnemyState.Dead) return;
        if (IsDead) return;
        if (IsStunned) return;

        Rigidbody rb = Rigidbody;
        if (rb == null) return;

        // 1) 이동
        Vector3 v = rb.velocity;

        if (state == EnemyState.Approach && desiredMoveDir.sqrMagnitude > 0.0001f)
        {
            Vector3 horizontal = desiredMoveDir.normalized * MoveSpeed;
            rb.velocity = new Vector3(horizontal.x, v.y, horizontal.z);
        }
        else
        {
            rb.velocity = new Vector3(0f, v.y, 0f);
        }

        // 2) 회전 (Y축)
        if (aimFlatWS.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(aimFlatWS.normalized, Vector3.up);
            Quaternion next = Quaternion.RotateTowards(rb.rotation, targetRot, turnDegPerSec * Time.fixedDeltaTime);
            rb.MoveRotation(next);
        }
    }

    private bool TryMeleeAttack()
    {
        if (weaponData == null) return false;
        if (weaponData.attackPrefab == null) return false;
        if (Time.time < nextMeleeTime) return false;

        int fr = Mathf.Max(1, weaponData.fireRate);
        float cd = 60f / fr; // Creature.TryFire()와 동일 규칙
        nextMeleeTime = Time.time + cd;

        Vector3 spawnPos = (FirePoint ? FirePoint.position : transform.position)
                         + transform.forward * meleeSpawnForward;

        Quaternion rot = Quaternion.LookRotation(aimFlatWS, Vector3.up);

        Attack atk = Instantiate(weaponData.attackPrefab, spawnPos, rot);

        EnemyMeleeAttack meleeAtk = atk as EnemyMeleeAttack;
        if (meleeAtk != null)
        {
            meleeAtk.ConfigureMelee(
                weaponData.damage,
                weaponData.projectileSpeed,   // melee에선 “짧은 전진 속도”로 재해석 가능
                weaponData.range,             // 이동형일 때 최대 거리
                0f,                           // 넉백 필요하면 WeaponData 확장 추천
                0f,                           // 스턴 필요하면 WeaponData 확장 추천
                transform,
                Type
            );
        }
        else
        {
            // 최소한 팀킬 방지는 반드시 세팅
            atk.Configure(weaponData.damage, 0f, transform, 0f, Type);
        }

        return true;
    }

    private bool AcquireTargetInSearchRange()
    {
        if (target != null) return true;

        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go == null) return false;

        Transform t = go.transform;
        float dist = DistanceXZ(transform.position, t.position);

        if (dist <= searchRange)
        {
            target = t;
            return true;
        }

        return false;
    }

    private float GetTargetDistanceXZ()
    {
        if (target == null) return float.PositiveInfinity;
        return DistanceXZ(transform.position, target.position);
    }

    private Vector3 GetDirToTargetXZ()
    {
        if (target == null) return Vector3.zero;
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        return dir;
    }

    private static float DistanceXZ(Vector3 a, Vector3 b)
    {
        a.y = 0f; b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void EnterDead(string reason)
    {
        state = EnemyState.Dead;
        Debug.Log($"EnemyMelee Dead ({reason})");
        Die();
    }

    protected override void Die()
    {
        GiveExpToPlayer();
        base.Die();
    }

    private void GiveExpToPlayer()
    {
        // Enemy.cs 방식 그대로
        PlayerProgress.instance.AddExp(expReward);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, searchRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
