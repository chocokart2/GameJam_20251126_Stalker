using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUp : MonoBehaviour
{
    [Header("UI")]
    public GameObject levelUpGroup;
    public Button[] rewardButtons = new Button[3];
    public TMP_Text[] rewardTexts = new TMP_Text[3];

    [Header("Card Pool")]
    public List<CardData> cardPool = new List<CardData>();

    [Header("Rarity Weight")]
    public int weightNormal = 70;
    public int weightRare = 25;
    public int weightEpic = 5;

    private PlayerProgress progress;
    private CardData[] offered = new CardData[3];

    private void Awake()
    {
        if (levelUpGroup == null) levelUpGroup = gameObject;
        levelUpGroup.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            int idx = i;
            if (rewardButtons[i] != null)
                rewardButtons[i].onClick.AddListener(() => OnClickReward(idx));
        }
    }

    // 외부(레벨업 발생 코드)에서 호출
    public void Open(PlayerProgress playerProgress)
    {
        progress = playerProgress;
        if (progress == null) return;

        Time.timeScale = 0f;
        levelUpGroup.SetActive(true);

        GenerateThreeCards();
        RefreshUI();
    }

    private void Close()
    {
        levelUpGroup.SetActive(false);
        Time.timeScale = 1f;
    }

    private void GenerateThreeCards()
    {
        for (int i = 0; i < 3; i++) offered[i] = null;

        // 중복 방지
        List<CardData> picked = new List<CardData>();

        for (int slot = 0; slot < 3; slot++)
        {
            CardData c = PickOneCard(picked);
            picked.Add(c);
            offered[slot] = c;
        }
    }

    private CardData PickOneCard(List<CardData> exclude)
    {
        // 1) rarity 먼저 뽑기
        CardRarity rarity = RollRarity();

        // 2) 후보 모으기(조건 체크)
        List<CardData> candidates = new List<CardData>();
        for (int i = 0; i < cardPool.Count; i++)
        {
            CardData c = cardPool[i];
            if (c == null) continue;
            if (c.rarity != rarity) continue;

            // 중복 후보 제외
            bool already = false;
            for (int k = 0; k < exclude.Count; k++)
                if (exclude[k] == c) { already = true; break; }
            if (already) continue;

            // maxLevel 도달이면 제외
            int curLv = progress.GetCardLevel(c.cardId);
            if (curLv >= c.maxLevel) continue;

            // prerequisite
            if (!string.IsNullOrEmpty(c.prerequisiteCardId))
            {
                if (!progress.HasCard(c.prerequisiteCardId)) continue;
            }

            candidates.Add(c);
        }

        // 후보가 없으면 rarity 완화(전체에서 뽑기)
        if (candidates.Count == 0)
        {
            for (int i = 0; i < cardPool.Count; i++)
            {
                CardData c = cardPool[i];
                if (c == null) continue;

                bool already = false;
                for (int k = 0; k < exclude.Count; k++)
                    if (exclude[k] == c) { already = true; break; }
                if (already) continue;

                int curLv = progress.GetCardLevel(c.cardId);
                if (curLv >= c.maxLevel) continue;

                if (!string.IsNullOrEmpty(c.prerequisiteCardId))
                    if (!progress.HasCard(c.prerequisiteCardId)) continue;

                candidates.Add(c);
            }
        }

        if (candidates.Count == 0) return null;

        int r = Random.Range(0, candidates.Count);
        return candidates[r];
    }

    private CardRarity RollRarity()
    {
        int total = weightNormal + weightRare + weightEpic;
        int r = Random.Range(0, total);

        r -= weightNormal;
        if (r < 0) return CardRarity.Normal;
        r -= weightRare;
        if (r < 0) return CardRarity.Rare;
        r -= weightEpic;
        if (r < 0) return CardRarity.Epic;

        return CardRarity.Epic;
    }

    private void RefreshUI()
    {
        for (int i = 0; i < 3; i++)
        {
            CardData c = offered[i];

            if (rewardButtons[i] != null)
                rewardButtons[i].interactable = (c != null);

            if (rewardTexts[i] != null)
            {
                if (c == null) rewardTexts[i].text = "NULL";
                else rewardTexts[i].text = c.cardName;
            }
        }
    }

    private void OnClickReward(int index)
    {
        if (progress == null) return;
        if (index < 0 || index >= 3) return;

        CardData picked = offered[index];
        if (picked == null) return;

        // 1) 카드 누적 반영
        progress.AddCardLevel(picked);
        Creature creature = progress.GetComponent<Creature>();
        if (creature != null)
            creature.RefreshRuntimeStats(true);
        // 2) HP = MaxHP 요구사항 처리(선택)
        //    네 Creature 구조에 따라 "최종 MaxHP" 계산 후 currentHealth 채우는 함수를 호출하면 됨.
        //    여기서는 훅만 남겨둠:
        // ApplyHealToFull();

        // 3) 닫기 + 재개
        Close();
    }

    // 여기서 Player/Creature에 연결해서 currentHP = finalMaxHP로 맞춰주면 다이어그램 그대로 됨
    // private void ApplyHealToFull() { ... }
}
