using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefaultGroupUI : MonoBehaviour
{
    public Image healthBar;
    public Image levelExpBar;
    public Image[] giftSlot;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI playerCurrentObjectText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 시간 업데이트
        int time = (int)Time.time;
        int minutes = time / 60;
        int seconds = time % 60;
        timeText.text = $"{minutes:D2}:{seconds:D2}";

        // 플레이어 체력
        healthBar.fillAmount =
            (float)GameManager.Instance.Player.CurrentHealth
            / GameManager.Instance.Player.MaxHealth;
        healthText.text =
            $"{GameManager.Instance.Player.CurrentHealth:D2}/{GameManager.Instance.Player.MaxHealth:D2}";

        // 플레이어 경험치
        // TODO - 다음 머지에 추가



    }
}
