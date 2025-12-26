using UnityEngine;

public enum CardRarity { Normal, Rare, Epic }
public enum StatType
{
    Move_Speed,
    Char_HP,
    Gun_Damage,
    Gun_FireRate,
    Gun_MaxAmmo,
    Push_Force
}

public enum CalcType
{
    Add,            // 예: HP +10
    MultiplyPercent // 예: 0.1 = +10%
}

[CreateAssetMenu(fileName = "Card_", menuName = "Game/CardData")]
public class CardData : ScriptableObject
{
    [Header("Identity")]
    public string cardId;
    public CardRarity rarity;
    public string cardName;
    [TextArea] public string description;

    [Header("Effect")]
    public StatType statType;
    public CalcType calcType;

    // MultiplyPercent면 0.1 = +10%, 0.3 = +30%
    // Add면 +10, +25 같은 "실수/정수 증가량"
    public float value;

    [Header("Limits")]
    public int maxLevel = 99;
    public string prerequisiteCardId; // 없으면 빈칸
}
