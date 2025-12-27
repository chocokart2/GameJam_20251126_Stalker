using UnityEngine;

public class HitMaterialInstance : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;

    private void Awake()
    {
        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.material = new Material(r.material); // 개별 머티리얼로 분리
        }
    }
}
