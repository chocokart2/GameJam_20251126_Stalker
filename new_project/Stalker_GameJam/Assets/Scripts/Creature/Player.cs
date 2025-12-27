using Autodesk.Fbx;
using UnityEngine;
using static UnityEngine.Mesh;

public class Player : Creature
{
    static public Player instance;

    public float PushSkillCooldown => Mathf.Max(nextPushTime - Time.time, 0);

    [Header("Move")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Aim (Mouse Raycast)")]
    [SerializeField] private LayerMask aimMask;         // Ground만 체크 추천
    [SerializeField] private float aimMaxDistance = 500f;

    [Header("Push Skill")]
    [SerializeField] PushSkillData pushSkillData;
    public PushSkillData PushSkillData => pushSkillData;
    float nextPushTime = 0f;
    public float instancePushSkillCooldown;

    private Vector2 moveInput;
    private bool wantsFire;
    private bool wantsPush;

    private bool hasAim;
    private Vector3 aimDir; // y=0 평면 방향
    private float aimDirLengthSqr;

    [SerializeField] private GameObject creatureModel;
    [SerializeField] Animator playerAnimator;

    public override bool TryReload()
    {
        SoundManager.Instance.PlaySfx("Reload");
        return base.TryReload();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        SoundManager.Instance.PlaySfx("Hit");
    }


    private void Awake()
    {
        base.Awake();

        instance = this;
    }

    private void Start()
    {

    }

protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.M))
        {
            creatureModel.transform.Rotate(0, 30, 0);
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.sqrMagnitude > 0.1)
        {
            // 움직임
            
        }
        UpdateAimByMouse();

        wantsFire = Input.GetMouseButton(0);
        if (wantsFire)
        {
            bool isFired = TryFire();
            if (isFired)
            {
                playerAnimator.SetTrigger("Shot");
                SoundManager.Instance.PlaySfx("GunShot");
            }
        }

        wantsPush = Input.GetKeyDown(KeyCode.Space);
        if (wantsPush)
        {
            TryPushSkill();
        }

        if (Input.GetKeyDown(KeyCode.R)) TryReload();
    }

    private void FixedUpdate()
    {
        if (IsDead || IsStunned) return;

        Rigidbody rb = Rigidbody;
        if (rb == null) return;

        // 카메라 고정이니까(TopDownCamera fixedYaw), 카메라 기준 이동이 싫으면 그냥 월드 기준으로 써도 됨.
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        playerAnimator.SetFloat("MoveSpeed", move.sqrMagnitude);

        if (hasAim && (aimDirLengthSqr > 1f))
        {
            creatureModel.transform.forward = aimDir.normalized;
        }

        if (move.sqrMagnitude > 1f) move.Normalize();

        Vector3 vel = rb.velocity;
        float spd = GetFinalMoveSpeed();
        vel.x = move.x * spd;
        vel.z = move.z * spd;
        rb.velocity = vel;
    }

    // 총알 “회전”만 마우스 방향으로
    protected override Quaternion GetFireRotation()
    {
        if (hasAim && aimDir.sqrMagnitude > 0.0001f)
            return Quaternion.LookRotation(aimDir, Vector3.up);

        return base.GetFireRotation();
    }

    private void UpdateAimByMouse()
    {
        hasAim = false;

        // 화면 밖이면 아예 갱신 중지 (원치 않는 튐 방지)
        Vector3 m = Input.mousePosition;
        if (m.x < 0f || m.y < 0f || m.x > Screen.width || m.y > Screen.height)
            return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(m);

        // 1) Ground 레이어 Raycast 우선
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, aimMaxDistance, aimMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 dir = hit.point - FirePoint.position; // 총알 발사점 기준이 더 정확
            dir.y = 0f;

            aimDirLengthSqr = dir.sqrMagnitude;
            if (dir.sqrMagnitude > 0.0001f)
            {
                aimDir = dir.normalized;
                hasAim = true;
            }
            return;
        }

        // 2) 플랜B: 바닥 콜라이더가 없을 때를 대비한 평면 교차
        Plane plane = new Plane(Vector3.up, new Vector3(0f, FirePoint.position.y, 0f));
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 p = ray.GetPoint(enter);
            Vector3 dir = p - FirePoint.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.0001f)
            {
                aimDir = dir.normalized;
                hasAim = true;
            }
        }
    }

    private float GetFinalMoveSpeed()
    {
        StatModifier stat = GetComponent<StatModifier>();
        if (stat == null) return moveSpeed;
        return stat.Eval(StatType.Move_Speed, moveSpeed);
    }
    protected override void Die()
    {
        playerAnimator.SetTrigger("Die");

        Debug.Log("Player has died!");

        PlayerUI.instance.CallWhenDeath();
        SpawnManager.instance.WhenPlayerDeath();
        SoundManager.Instance.PlaySfx("P_Die");

        //Destroy(gameObject);
        Destroy(this); // 컴포넌트만 죽여보기
    }

    private bool CanPushNow()
    {
        if (IsDead || IsStunned) return false;
        if (pushSkillData == null) return false;
        if (Time.time < nextPushTime) return false;
        return true;
    }

    private void TryPushSkill()
    {
        Debug.Assert(pushSkillData != null, "Push Skill Data is not assigned!");
        Debug.Assert(pushSkillData.prefab != null, "Push Skill Prefab is not assigned!");

        if (!CanPushNow()) return;
        nextPushTime = Time.time + pushSkillData.cooldown;

        SoundManager.Instance.PlaySfx("Push");
        GameObject go = Instantiate(pushSkillData.prefab, transform.position, transform.rotation);
        MeleeAttack meleeAttack = go.GetComponent<MeleeAttack>();

        float finalKb = pushSkillData.knockbackForce;
        StatModifier stat = GetComponent<StatModifier>();
        if (stat != null)
            finalKb = stat.Eval(StatType.Push_Force, finalKb);

        // 여기만 바꾸면 Push_Force 카드가 진짜로 반영됨
        meleeAttack.Configure(
            pushSkillData.damage,
            finalKb,
            transform,                 // 넉백 기준점
            pushSkillData.stunDuration,
            Type                       // 자기 타입 면역
        );
    }

}
