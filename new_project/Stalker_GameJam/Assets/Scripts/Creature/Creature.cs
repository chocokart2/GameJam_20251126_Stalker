using UnityEngine;

public class Creature : MonoBehaviour
{
    public int CurrentHealth => currentHealth;
    public int MaxHealth => creatureData != null ? creatureData.maxHealth : 0;

    [Header("Data")]
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private WeaponData weaponData;

    [Header("Refs")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform knockbackOrigin;
    [SerializeField] private Rigidbody rb;

    [Header("Runtime")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int currentBulletCount;

    public float AttackRange => weaponData != null ? weaponData.range : 0f;
    public float ProjectileSpeed => weaponData != null ? weaponData.projectileSpeed : 0f;

    private bool reloading;
    private float stunRemain;

    private float nextFireTime;
    private float nextPushTime;

    public CreatureType Type => creatureData != null ? creatureData.creatureType : CreatureType.Enemy;
    public float MoveSpeed => creatureData != null ? creatureData.moveSpeed : 0f;


    public bool IsDead => currentHealth <= 0;
    public bool IsStunned => stunRemain > 0f;

    public Rigidbody Rigidbody => rb != null ? rb : (rb = GetComponent<Rigidbody>());
    public Transform FirePoint => firePoint != null ? firePoint : transform;
    public Transform KnockbackOrigin => knockbackOrigin;

    protected virtual void Awake()
    {
        ApplyData();
    }

    protected virtual void Update()
    {
        // 스턴 타이머
        if (stunRemain > 0f)
        {
            stunRemain -= Time.deltaTime;
            if (stunRemain < 0f) stunRemain = 0f;
        }
    }

    public virtual void ApplyData()
    {
        if (creatureData != null)
            currentHealth = creatureData.maxHealth;

        if (weaponData != null)
            currentBulletCount = weaponData.maxAmmo;

        reloading = false;
        stunRemain = 0f;
        nextFireTime = 0f;
        nextPushTime = 0f;
    }

    // ------------------------
    // Damage / Stun
    // ------------------------
    public virtual void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= Mathf.Abs(damage);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void ApplyStun(float duration)
    {
        if (duration <= 0f) return;
        stunRemain = Mathf.Max(stunRemain, duration);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    // ------------------------
    // Weapon Runtime (fire/reload)
    // ------------------------
    public bool CanFireNow()
    {
        if (IsDead || IsStunned) return false;
        if (weaponData == null) return false;
        if (reloading) return false;
        if (Time.time < nextFireTime) return false;
        if (currentBulletCount <= 0) return false;
        return true;
    }

    public bool TryFire()
    {
        if (!CanFireNow()) return false;

        // fire rate: "분당" 이면 초당 간격 = 60 / fireRate
        float fireInterval = (weaponData.fireRate > 0) ? (60f / weaponData.fireRate) : 0.1f;
        nextFireTime = Time.time + fireInterval;

        currentBulletCount--;

        SpawnAttackFromWeapon();

        if (currentBulletCount <= 0)
            TryReload();

        return true;
    }

    public bool TryReload()
    {
        if (weaponData == null) return false;
        if (reloading) return false;
        if (currentBulletCount >= weaponData.maxAmmo) return false;

        reloading = true;
        Invoke(nameof(FinishReload), weaponData.bulletReloadTime);
        return true;
    }

    private void FinishReload()
    {
        reloading = false;

        if (weaponData != null)
            currentBulletCount = weaponData.maxAmmo;
    }

    private void SpawnAttackFromWeapon()
    {
        if (weaponData == null || weaponData.attackPrefab == null) return;

        Attack atk = Instantiate(weaponData.attackPrefab, FirePoint.position, GetFireRotation());
        atk.SetOwner(this);

        // Bullet이면 무기 데이터로 세팅
        Bullet bullet = atk as Bullet;
        if (bullet != null)
        {
            bullet.Configure(
                weaponData.damage,
                weaponData.projectileSpeed,
                weaponData.range,
                0f,                 // 총알 스턴 필요하면 WeaponData에 추가해서 넣어라
                0f,                 // 총알 넉백 필요하면 WeaponData에 추가해서 넣어라
                KnockbackOrigin != null ? KnockbackOrigin : transform
            );
        }
    }
    protected virtual Quaternion GetFireRotation()
    {
        return FirePoint.rotation;
    }
    // ------------------------
    // Push Skill Runtime
    // ------------------------
    public bool CanPushNow(PushSkillData pushData)
    {
        if (IsDead || IsStunned) return false;
        if (pushData == null) return false;
        if (Time.time < nextPushTime) return false;
        return true;
    }

    public bool TryPush(MeleeAttack meleePrefab, PushSkillData pushData)
    {
        if (!CanPushNow(pushData)) return false;
        if (meleePrefab == null) return false;

        nextPushTime = Time.time + pushData.cooldown;

        // MeleeAttack은 “발동 즉시 판정”이라 Instantiate 후 즉시 Activate 호출하고 끝
        MeleeAttack melee = Instantiate(meleePrefab, transform.position, transform.rotation);
        melee.Activate(this, pushData);

        // 히트박스가 지속되지 않으니 바로 제거
        Destroy(melee.gameObject);

        return true;
    }
}
