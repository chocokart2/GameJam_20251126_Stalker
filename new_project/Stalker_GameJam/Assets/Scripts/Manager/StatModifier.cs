using System.Collections.Generic;
using UnityEngine;

public class StatModifier : MonoBehaviour
{
    [SerializeField] private List<CardData> allCards = new List<CardData>();
    private PlayerProgress progress;

    private void Awake()
    {
        progress = GetComponent<PlayerProgress>();

        if (progress == null)
            progress = PlayerProgress.instance;

        if (progress == null)
            progress = GetComponentInParent<PlayerProgress>();
    }


    // Final = (Base + AddSum) * Mul
    public float Eval(StatType type, float baseValue)
    {
        if (progress == null) return baseValue;

        float addSum = 0f;
        float mul = 1f;

        for (int i = 0; i < allCards.Count; i++)
        {
            CardData c = allCards[i];
            if (c == null) continue;
            if (c.statType != type) continue;

            int lv = progress.GetCardLevel(c.cardId);
            if (lv <= 0) continue;

            if (c.calcType == CalcType.Add)
            {
                addSum += c.value * lv;
            }
            else // MultiplyPercent
            {
                // value=0.1이면 레벨 1당 +10%
                mul *= (1f + c.value * lv);
            }
        }

        return (baseValue + addSum) * mul;
    }

}
