using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillId;
    public float cooldown;
}
