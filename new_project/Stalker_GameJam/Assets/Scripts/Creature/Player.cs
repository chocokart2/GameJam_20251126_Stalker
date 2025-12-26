using UnityEngine;

public class Player : Creature
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Aim (Mouse Raycast)")]
    [SerializeField] private LayerMask aimMask;         // Ground만 체크 추천
    [SerializeField] private float aimMaxDistance = 500f;

    private Vector2 moveInput;
    private bool wantsFire;

    private bool hasAim;
    private Vector3 aimDir; // y=0 평면 방향

    protected override void Update()
    {
        base.Update();

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        UpdateAimByMouse();

        wantsFire = Input.GetMouseButton(0);
        if (wantsFire) TryFire();

        if (Input.GetKeyDown(KeyCode.R)) TryReload();
    }

    private void FixedUpdate()
    {
        if (IsDead || IsStunned) return;

        Rigidbody rb = Rigidbody;
        if (rb == null) return;

        // 카메라 고정이니까(TopDownCamera fixedYaw), 카메라 기준 이동이 싫으면 그냥 월드 기준으로 써도 됨.
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        if (move.sqrMagnitude > 1f) move.Normalize();

        Vector3 vel = rb.velocity;
        vel.x = move.x * moveSpeed;
        vel.z = move.z * moveSpeed;
        rb.velocity = vel;
    }

    // ★ 총알 “회전”만 마우스 방향으로
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

    protected override void Die()
    {
        Destroy(gameObject);
        Debug.Log("Player has died!");
    }
}
