using UnityEngine;

public class HpbarBillboard : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private bool lockX = true;
    [SerializeField] private bool lockZ = true;

    private void LateUpdate()
    {
        if (cam == null)
        {
            Camera c = Camera.main;
            if (c == null) return;
            cam = c.transform;
        }

        // "나 -> 카메라" 방향
        Vector3 dir = cam.position - transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        Vector3 e = rot.eulerAngles;
        if (lockX) e.x = 0f;   // 위아래 기울기 방지
        if (lockZ) e.z = 0f;   // 옆으로 눕는 현상 방지

        transform.rotation = Quaternion.Euler(e);
    }
}