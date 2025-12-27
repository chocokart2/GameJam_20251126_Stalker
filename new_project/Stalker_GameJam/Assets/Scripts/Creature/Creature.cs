using System;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int CurrentHealth => currentHealth;
    public int CurrentBulletCount => currentBulletCount;
    public int MaxAmmo => GetFinalMaxAmmo();

    public int MaxHealth => GetFinalMaxHealth();

    [Header("Data")]
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private WeaponData weaponData;

    [Header("Refs")]
    [SerializeField] private Transform firePoint;
    [Obsolete("크리쳐의 트랜스폼이 넉벡오리진입니다.")]
    [SerializeField] private Transform knockbackOrigin;
    [SerializeField] private Rigidbody rb;

    [Header("Runtime")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int currentBulletCount;


    [SerializeField] private StatModifier statMod;
    public float AttackRange => weaponData != null ? weaponData.range : 0f;
    public float ProjectileSpeed => weaponData != null ? weaponData.projectileSpeed : 0f;

    private bool reloading;
    private float stunRemain;

    private float nextFireTime;
    //protected float nextPushTime;

    public CreatureType Type => creatureData != null ? creatureData.creatureType : CreatureType.Enemy;
    public float MoveSpeed => creatureData != null ? creatureData.moveSpeed : 0f;


    public bool IsDead => currentHealth <= 0;
    public bool IsStunned => stunRemain > 0f;

    public Rigidbody Rigidbody => rb != null ? rb : (rb = GetComponent<Rigidbody>());
    public Transform FirePoint => firePoint != null ? firePoint : transform;
    [Obsolete("크리쳐의 트랜스폼이 넉벡오리진입니다.")] public Transform KnockbackOrigin => knockbackOrigin;

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
            currentHealth = GetFinalMaxHealth();

        if (weaponData != null)
            currentBulletCount = GetFinalMaxAmmo();

        reloading = false;
        stunRemain = 0f;
        nextFireTime = 0f;
        //nextPushTime = 0f;
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

        int fr = GetFinalFireRatePerMin();
        float fireInterval = (fr > 0) ? (60f / fr) : 0.1f;

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
        int maxAmmo = GetFinalMaxAmmo();
        if (currentBulletCount >= maxAmmo) return false;

        reloading = true;
        Invoke(nameof(FinishReload), weaponData.bulletReloadTime);
        return true;
    }

    private void FinishReload()
    {
        reloading = false;

        if (weaponData != null)
            currentBulletCount = GetFinalMaxAmmo();
    }

    private void SpawnAttackFromWeapon()
    {
        if (weaponData == null || weaponData.attackPrefab == null) return;

        Attack atk = Instantiate(weaponData.attackPrefab, FirePoint.position, GetFireRotation());
        //atk.SetOwner(this);

        // Bullet이면 무기 데이터로 세팅
        Bullet bullet = atk as Bullet;
        if (bullet != null)
        {
            bullet.Configure(
                GetFinalWeaponDamage(),
                weaponData.projectileSpeed,
                weaponData.range,
                0f,                 // 총알 스턴 필요하면 WeaponData에 추가해서 넣어라
                0f,                 // 총알 넉백 필요하면 WeaponData에 추가해서 넣어라
                transform
            );
        }
    }
    protected virtual Quaternion GetFireRotation()
    {
        return FirePoint.rotation;
    }

    // this push is only for player
    // ------------------------
    // Push Skill Runtime
    // ------------------------
    //public bool CanPushNow(PushSkillData pushData)
    //{
    //    if (IsDead || IsStunned) return false;
    //    if (pushData == null) return false;
    //    if (Time.time < nextPushTime) return false;
    //    return true;
    //}

    // this push is only for player
    //public bool TryPush(MeleeAttack meleePrefab, PushSkillData pushData)
    //{
    //    if (!CanPushNow(pushData)) return false;
    //    if (meleePrefab == null) return false;

    //    nextPushTime = Time.time + pushData.cooldown;
    // 
    //    // MeleeAttack은 “발동 즉시 판정”이라 Instantiate 후 즉시 Activate 호출하고 끝
    //    MeleeAttack melee = Instantiate(meleePrefab, transform.position, transform.rotation);
    //    melee.Activate(this, pushData);

    //    // 히트박스가 지속되지 않으니 바로 제거
    //    Destroy(melee.gameObject);

    //    return true;
    //}

    // ------------------------
    private int GetFinalMaxHealth()
    {
        int baseHp = creatureData != null ? creatureData.maxHealth : 0;
        if (statMod == null) statMod = GetComponent<StatModifier>();
        if (statMod == null) return baseHp;
        return Mathf.Max(1, Mathf.RoundToInt(statMod.Eval(StatType.Char_HP, baseHp)));
    }

    private int GetFinalWeaponDamage()
    {
        int baseDmg = weaponData != null ? weaponData.damage : 0;
        if (statMod == null) statMod = GetComponent<StatModifier>();
        if (statMod == null) return baseDmg;
        return Mathf.Max(0, Mathf.RoundToInt(statMod.Eval(StatType.Gun_Damage, baseDmg)));
    }

    private int GetFinalFireRatePerMin()
    {
        int baseRate = weaponData != null ? weaponData.fireRate : 0;
        if (statMod == null) statMod = GetComponent<StatModifier>();
        if (statMod == null) return baseRate;
        return Mathf.Max(1, Mathf.RoundToInt(statMod.Eval(StatType.Gun_FireRate, baseRate)));
    }

    private int GetFinalMaxAmmo()
    {
        int baseAmmo = weaponData != null ? weaponData.maxAmmo : 0;
        if (statMod == null) statMod = GetComponent<StatModifier>();
        if (statMod == null) return baseAmmo;
        return Mathf.Max(0, Mathf.RoundToInt(statMod.Eval(StatType.Gun_MaxAmmo, baseAmmo)));
    }

    //LevelUp이 호출할 공개 메서드
    public void RefreshRuntimeStats(bool healToFull)
    {
        int finalMaxHp = GetFinalMaxHealth();
        currentHealth = healToFull ? finalMaxHp : Mathf.Clamp(currentHealth, 0, finalMaxHp);

        int finalMaxAmmo = GetFinalMaxAmmo();
        currentBulletCount = Mathf.Clamp(currentBulletCount, 0, finalMaxAmmo);
    }
}
