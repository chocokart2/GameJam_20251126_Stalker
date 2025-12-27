using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBarUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Creature owner;   // Enemy(=Creature)
    [SerializeField] private Image fill;       // Filled ¿ÃπÃ¡ˆ

    private void Awake()
    {
        if (owner == null) owner = GetComponentInParent<Creature>();
    }

    private void LateUpdate()
    {
        if (owner == null || fill == null) return;

        int maxHp = owner.MaxHealth;
        int curHp = owner.CurrentHealth;

        float t = 1f;
        if (maxHp > 0) t = Mathf.Clamp01((float)curHp / maxHp);

        fill.fillAmount = t;
        gameObject.SetActive(!owner.IsDead);
    }
}
