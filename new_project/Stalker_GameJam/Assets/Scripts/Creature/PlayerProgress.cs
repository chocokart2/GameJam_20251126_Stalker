using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public int level = 1;

    // cardId -> level
    private Dictionary<string, int> cardLevels = new Dictionary<string, int>();

    public int GetCardLevel(string cardId)
    {
        int lv = 0;
        cardLevels.TryGetValue(cardId, out lv);
        return lv;
    }

    public void AddCardLevel(CardData card)
    {
        if (card == null) return;

        int cur = GetCardLevel(card.cardId);
        cur++;
        if (cur > card.maxLevel) cur = card.maxLevel;
        cardLevels[card.cardId] = cur;
    }

    public bool HasCard(string cardId)
    {
        return GetCardLevel(cardId) > 0;
    }
}
