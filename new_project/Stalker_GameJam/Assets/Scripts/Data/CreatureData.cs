using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "Scriptable Objects/CreatureData")]
public class CreatureData : ScriptableObject
{
    public CreatureType creatureType;

    public string memberName;
    public int maxHealth;
    public float moveSpeed;
    //
    //public AttackObject attackObjectPrefab;

    public PushSkillData pushSkillData;

    public CreatureData(CreatureData one)
    {
        creatureType = one.creatureType;
        memberName = one.memberName;
        maxHealth = one.maxHealth;
        moveSpeed = one.moveSpeed;
        pushSkillData = one.pushSkillData;
    }
}
