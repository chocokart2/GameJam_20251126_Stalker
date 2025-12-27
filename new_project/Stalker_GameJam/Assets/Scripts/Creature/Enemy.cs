using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Creature
{
    private enum EnemyState { Search, Approach, Attack, Dead }

    [Header("AI Refs")]
    [SerializeField] private Transform target;

    [Header("AI Ranges")]
    [SerializeField] private float searchRange = 25f;
    [SerializeField] private float attackRangeOverride = 0f;

    [Header("AI Search Rule")]
    [SerializeField] private float notFoundDieSeconds = 120f;

    [Header("Aim / Fire")]
    [SerializeField] private float turnDegPerSec = 720f;
    [SerializeField] private float fireAngleTolerance = 7f;
    [SerializeField] private bool useLeadShot = true;
    [SerializeField] private float maxLeadTime = 0.5f;

    [Header("Reward")]
    [SerializeField] private int expReward = 1;

    private EnemyState state = EnemyState.Search;
    private float notFoundTimer = 0f;
    private Vector3 desiredMoveDir;

    private Vector3 aimDirWS = Vector3.forward;
    private Vector3 aimFlatWS = Vector3.forward;

    protected override void Update()
    {
        base.Update();
        if (state == EnemyState.Dead) return;

        if (IsDead) { EnterDead("HP <= 0"); return; }

        bool hasTarget = AcquireTargetInSearchRange();

        if (!hasTarget)
        {
            notFoundTimer += Time.deltaTime;
            state = EnemyState.Search;

            if (notFoundTimer >= notFoundDieSeconds)
                EnterDead("Target Not Found (120s)");

            desiredMoveDir = Vector3.zero;
            return;
        }

        notFoundTimer = 0f;

        float atkRange = GetAttackRange();
        float dist = GetTargetDistanceXZ();

        state = (dist <= atkRange) ? EnemyState.Attack : EnemyState.Approach;

        // 조준 방향 갱신 (리드샷 포함)
        aimDirWS = ComputeAimDirection();

        aimFlatWS = aimDirWS;
        aimFlatWS.y = 0f;
        if (aimFlatWS.sqrMagnitude < 0.0001f)
            aimFlatWS = transform.forward;

        switch (state)
        {
            case EnemyState.Search:
                desiredMoveDir = Vector3.zero;
                break;

            case EnemyState.Approach:
                desiredMoveDir = GetDirToTargetXZ();
                break;

            case EnemyState.Attack:
                desiredMoveDir = Vector3.zero;

                // 조준 각도 안이면 계속 발사(연사/쿨다운은 Creature.TryFire가 제어)
                if (IsAimedEnough(aimFlatWS))
                    TryFire();
                break;
        }
        Debug.DrawRay(FirePoint.position, FirePoint.forward * 2f, Color.red);
        Debug.DrawRay(FirePoint.position, aimFlatWS.normalized * 2f, Color.green);

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

        // 2) 회전: 항상 플레이어 바라보기 (Y축)
        if (aimFlatWS.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(aimFlatWS.normalized, Vector3.up);
            Quaternion next = Quaternion.RotateTowards(rb.rotation, targetRot, turnDegPerSec * Time.fixedDeltaTime);
            rb.MoveRotation(next);
        }
    }

    private Vector3 ComputeAimDirection()
    {
        if (target == null) return transform.forward;

        Vector3 from = FirePoint.position;
        Vector3 to = target.position;

        if (!useLeadShot) return (to - from);

        Vector3 v = Vector3.zero;
        Rigidbody trb = target.GetComponent<Rigidbody>();
        if (trb == null) trb = target.GetComponentInChildren<Rigidbody>();
        if (trb != null) v = trb.velocity;

        float speed = ProjectileSpeed;
        if (speed <= 0.01f) speed = 10f;

        float dist = Vector3.Distance(from, to);
        float t = dist / speed;
        if (t > maxLeadTime) t = maxLeadTime;

        Vector3 predicted = to + v * t;
        return (predicted - from);
    }

    private bool IsAimedEnough(Vector3 flatDirWS)
    {
        flatDirWS.y = 0f;
        if (flatDirWS.sqrMagnitude < 0.0001f) return false;

        // FirePoint.forward 대신 "현재 몸체가 보는 방향" 사용
        Vector3 fwd = GetFacingForward();
        fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.0001f) return false;

        float angle = Vector3.Angle(fwd, flatDirWS);
        return angle <= fireAngleTolerance;
    }

    private Vector3 GetFacingForward()
    {
        Rigidbody rb = Rigidbody;
        if (rb != null)
            return rb.rotation * Vector3.forward;   // 물리 회전 기준

        return transform.forward;                   // fallback
    }

    private bool AcquireTargetInSearchRange()
    {
        // 타겟이 파괴됐는지 체크
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

    private float GetAttackRange()
    {
        if (attackRangeOverride > 0f) return attackRangeOverride;
        return AttackRange > 0f ? AttackRange : 2f;
    }

    private float GetTargetDistanceXZ()
    {
        if (target == null) return float.PositiveInfinity;
        return DistanceXZ(transform.position, target.position);
    }

    private static float DistanceXZ(Vector3 a, Vector3 b)
    {
        a.y = 0f; b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private Vector3 GetDirToTargetXZ()
    {
        if (target == null) return Vector3.zero;
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        return dir;
    }

    private void EnterDead(string reason)
    {
        state = EnemyState.Dead;
        Debug.Log($"Enemy Dead ({reason})");
        Die();
    }
    protected override void Die()
    {
        GiveExpToPlayer();
        base.Die();
    }

    private void GiveExpToPlayer()
    {
        // 가장 단순/안전: 태그로 Player 찾고, PlayerProgress를 가져와서 exp 추가
        //GameObject go = GameObject.FindGameObjectWithTag("Player");
        //if (go == null) return;

        //PlayerProgress prog = go.GetComponent<PlayerProgress>();
        //PlayerProgress prog = Player.instance.gameObject.GetComponent<PlayerProgress>();
        //if (prog == null) return;
        
        //prog.AddExp(expReward);
        PlayerProgress.instance.AddExp(expReward);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, searchRange);

        float atk = attackRangeOverride > 0f ? attackRangeOverride : AttackRange;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, atk);
    }
}
