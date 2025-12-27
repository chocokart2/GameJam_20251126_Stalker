using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponId;
    public WeaponType weaponType;

    public int damage;
    public int fireRate;
    public float range;
    public int maxAmmo;
    public float bulletReloadTime;

    public Attack attackPrefab;
    public float projectileSpeed;

    public WeaponData(WeaponData one)
    {
        this.weaponId = one.weaponId;
        this.weaponType = one.weaponType;
        this.damage = one.damage;
        this.fireRate = one.fireRate;
        this.range = one.range;
        this.maxAmmo = one.maxAmmo;
        this.bulletReloadTime = one.bulletReloadTime;
        this.attackPrefab = one.attackPrefab;
    }
}
