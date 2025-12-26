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
}
