using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "PushSkillData", menuName = "Scriptable Objects/PushSkillData")]
public class PushSkillData : SkillData
{
    public float range;

    public int damage;
    public float knockbackForce;
    public float stunDuration;
    public GameObject prefab;
}
