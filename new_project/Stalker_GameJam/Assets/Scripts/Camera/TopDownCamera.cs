using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -8f);
    [SerializeField] private float followSpeed = 25f;

    [Header("Fixed View")]
    [Range(10f, 89f)]
    [SerializeField] private float pitch = 60f;
    [SerializeField] private float fixedYaw = 0f; // 카메라 방향 고정

    private void LateUpdate()
    {
        if (target == null) return;

        Quaternion yawRot = Quaternion.Euler(0f, fixedYaw, 0f);

        Vector3 desiredPos = target.position + yawRot * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(pitch, fixedYaw, 0f);
    }

    public void SetTarget(Transform t) => target = t;
}
