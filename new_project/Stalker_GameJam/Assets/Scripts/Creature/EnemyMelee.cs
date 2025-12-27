using UnityEngine;
using System.Collections;
public class EnemyMelee : Creature
{
    private enum EnemyState { Search, Approach, Attack, Dead }

    [Header("AI Refs")]
    [SerializeField] private Transform target;

    [Header("AI Ranges")]
    [SerializeField] private float searchRange = 25f;
    [SerializeField] private float meleeRange = 1.8f;
    [SerializeField] private float attackStopBuffer = 0.1f;
    [SerializeField] private float meleeSpawnForward = 0.8f;

    [Header("AI Search Rule")]
    [SerializeField] private float notFoundDieSeconds = 120f;

    [Header("Rotate")]
    [SerializeField] private float turnDegPerSec = 720f;

    [Header("Reward")]
    [SerializeField] private int expReward = 1;

    [Header("Animator")]
    public Animator EnemyMeleeAnimator;

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
        if (aimFlatWS.sqrMagnitude < 0.0001f)
            aimFlatWS = transform.forward;

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

                // 쿨다운이면 근접 공격 발동(성공했을 때만 트리거)
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

        // 3) 애니: MoveSpeed 갱신 (Idle/Run 전이 핵심)
        AnimMoveSpeed(rb.velocity);
    }

    // =========================
    // Animator Helpers
    // =========================

    private void AnimMoveSpeed(Vector3 velocity)
    {
        if (EnemyMeleeAnimator == null) return;

        velocity.y = 0f;
        float speed = velocity.magnitude;
        EnemyMeleeAnimator.SetFloat("MoveSpeed", speed);
    }

    private void AnimShot()
    {
        if (EnemyMeleeAnimator == null) return;

        EnemyMeleeAnimator.ResetTrigger("OnShot");
        EnemyMeleeAnimator.SetTrigger("OnShot");
    }

    private void AnimDie()
    {
        if (EnemyMeleeAnimator == null) return;

        EnemyMeleeAnimator.ResetTrigger("OnDie");
        EnemyMeleeAnimator.SetTrigger("OnDie");
    }

    // =========================
    // Melee Attack
    // =========================

    private bool TryMeleeAttack()
    {
        if (WeaponData == null) return false;
        if (WeaponData.attackPrefab == null) return false;
        if (Time.time < nextMeleeTime) return false;

        int fr = Mathf.Max(1, WeaponData.fireRate);
        float cd = 60f / fr;
        nextMeleeTime = Time.time + cd;

        Vector3 spawnPos = (FirePoint ? FirePoint.position : transform.position)
                         + transform.forward * meleeSpawnForward;

        Quaternion rot = Quaternion.LookRotation(aimFlatWS, Vector3.up);

        Attack atk = Instantiate(WeaponData.attackPrefab, spawnPos, rot);

        EnemyMeleeAttack meleeAtk = atk as EnemyMeleeAttack;
        if (meleeAtk != null)
        {
            meleeAtk.ConfigureMelee(
                WeaponData.damage,
                WeaponData.projectileSpeed,
                WeaponData.range,
                0f,
                0f,
                transform,
                Type
            );
        }
        else
        {
            atk.Configure(WeaponData.damage, 0f, transform, 0f, Type);
        }

        // 공격이 실제로 발동된 "이 순간"에만 트리거
        AnimShot();

        return true;
    }

    // =========================
    // Target / Range
    // =========================

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

    // =========================
    // Death / Reward
    // =========================

    private void EnterDead(string reason)
    {
        if (state == EnemyState.Dead) return;

        state = EnemyState.Dead;

        AnimDie();

        Debug.Log($"EnemyMelee Dead ({reason})");

        StartCoroutine(Co_DeathSequence());
    }

    private IEnumerator Co_DeathSequence()
    {
        enabled = false;

        if (Rigidbody != null)
        {
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.isKinematic = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float deathAnimTime = 3f;

        if (EnemyMeleeAnimator != null)
        {
            yield return null;

            AnimatorStateInfo st =
                EnemyMeleeAnimator.GetCurrentAnimatorStateInfo(0);

            if (st.IsName("Die"))
                deathAnimTime = st.length;
        }

        yield return new WaitForSeconds(deathAnimTime);

        base.Die();
    }

    protected override void Die()
    {
    }

    private void GiveExpToPlayer()
    {
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
