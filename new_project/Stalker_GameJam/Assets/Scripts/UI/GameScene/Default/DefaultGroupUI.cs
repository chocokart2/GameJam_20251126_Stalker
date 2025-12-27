using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefaultGroupUI : MonoBehaviour
{
    public static DefaultGroupUI instance;

    public Image healthBar;
    public Image skillCooltimeBar;
    public Image levelExpBar;
    public Image[] giftSlot;
    public TextMeshProUGUI bulletText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI playerCurrentObjectText;
    Player player;
    public PlayerProgress progress;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;

        if (player != null && progress == null)
            progress = player.GetComponent<PlayerProgress>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            // 플레이어 오브젝트가 Destroy된 상태 (MissingReference 방지)
            if (healthBar != null) healthBar.fillAmount = 0f;
            if (healthText != null) healthText.text = "0/0";
            if (levelExpBar != null) levelExpBar.fillAmount = 0f;

            enabled = false;  // UI 업데이트 중단
            return;
        }
        // 시간 업데이트
        int time = (int)Time.time;
        int minutes = time / 60;
        int seconds = time % 60;
        timeText.text = $"{minutes:D2}:{seconds:D2}";

        // 탄약 업데이트
        bulletText.text = $"{player.CurrentBulletCount:D2}/{player.MaxAmmo:D2}";


        // 플레이어 체력
        healthBar.fillAmount =
            (float)player.CurrentHealth
            / player.MaxHealth;
        healthText.text =
            $"{player.CurrentHealth:D2}";

        // 플레이어 스킬 쿨타임

        if (player.PushSkillCooldown > 0f)
        {
            skillCooltimeBar.fillAmount =
               1f - (player.PushSkillCooldown / player.PushSkillData.cooldown);
        }
        else
        {
            skillCooltimeBar.fillAmount = 0f;
        }

        
        // 플레이어 경험치
        if (progress != null && levelExpBar != null)
        {
            int need = Mathf.Max(1, progress.NeedExpToNext);
            levelExpBar.fillAmount = (float)progress.CurrentExpInLevel / need;
        }
    }

        // TODO - 다음 머지에 추가
        // 선물 리스트 보관
        public bool TryAddGiftToFirstEmpty(Sprite giftSprite)
        {
        if (giftSprite == null) return false;
        if (giftSlot == null || giftSlot.Length == 0) return false;

        for (int i = 0; i < giftSlot.Length; i++)
        {
            Image slot = giftSlot[i];
            if (slot == null) continue;

            // 빈칸 기준: sprite가 null
            if (slot.sprite == null)
            {
                slot.sprite = giftSprite;
                slot.enabled = true;  // 혹시 비활성/숨김이면 켜주기
                slot.color = Color.white; // 알파 0으로 해놨던 경우 대비
                return true;
            }
        }
        return false; // 빈칸 없음
    }
}
