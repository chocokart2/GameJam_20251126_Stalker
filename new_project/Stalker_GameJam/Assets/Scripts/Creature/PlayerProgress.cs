using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    static public PlayerProgress instance;


    [Header("Level")]
    public int level = 1;
    public int maxLevel = 30;

    // 현재 레벨에서의 누적 EXP(=킬 수)
    [SerializeField] private int expInLevel = 0;

    [Header("Refs")]
    [SerializeField] private LevelUp levelUpUI;

    public int CurrentExpInLevel => expInLevel;
    public int NeedExpToNext => GetNeedKillCount(level);

    // 몬스터가 주는 exp = 1 (고정)
    public void AddExp(int amount)
    {
        Debug.Log($"PlayerProgress.AddExp({amount}) called.");

        if (amount <= 0) return;
        if (level >= maxLevel) return;

        // LevelUp UI가 열려있을 때는 exp 누적 막고 싶으면 여기서 return 해도 됨.
        // (지금 LevelUp이 Time.timeScale=0이라 몬스터가 더 죽을 일이 거의 없음)
        expInLevel += amount;

        int need = GetNeedKillCount(level);
        if (need <= 0) return;

        if (expInLevel >= need)
        {
            expInLevel = 0;
            level++;

            if (levelUpUI != null)
                levelUpUI.Open(this);
        }
    }

    // 이미지 테이블 그대로: "Next_LV (Kill Monster)"
    private int GetNeedKillCount(int lv)
    {
        // 안전 처리
        if (lv < 1) lv = 1;
        if (lv >= maxLevel) return 0;

        switch (lv)
        {
            case 1: return 3;
            case 2: return 5;
            case 3: return 7;
            case 4: return 10;
            case 5: return 10;
            case 6: return 10;
            case 7: return 13;
            case 8: return 14;
            case 9: return 15;
            case 10: return 17;

            // 11~22 : 25
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
                return 25;

            // 23~29 : 30 (30은 MAX라서 0 처리됨)
            default:
                return 30;
        }
    }

    // ---- 아래는 기존 카드 레벨 딕셔너리(너가 올린 코드) 그대로 유지 ----
    // cardId -> level
    private System.Collections.Generic.Dictionary<string, int> cardLevels =
        new System.Collections.Generic.Dictionary<string, int>();

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


    private void Awake()
    {
        instance = this;
    }
}
