using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GiftUI : MonoBehaviour
{
    [Header("GiftGroup")]
    [SerializeField] private GameObject giftGroupRoot;
    [SerializeField] private TMP_Text titleText;

    [Header("Gift Button")]
    [SerializeField] private Button giftButton;
    [SerializeField] private Image giftButtonImage;

    [Header("Auto Find Target")]
    [SerializeField] private DefaultGroupUI defaultGroupUI; // 드래그 안되면 자동으로 찾음

    private Sprite currentGiftSprite;

    private void Awake()
    {
        if (giftButton != null)
            giftButton.onClick.AddListener(OnClickGift);

        // 드래그가 안되면 런타임에 자동으로 찾아서 세팅
        if (defaultGroupUI == null)
            defaultGroupUI = FindObjectOfType<DefaultGroupUI>(true);

        Hide();
    }

    public void Show(Sprite giftSprite)
    {
        currentGiftSprite = giftSprite;

        if (titleText != null) titleText.text = "GOT A GIFT!";

        if (giftButtonImage != null)
        {
            giftButtonImage.sprite = currentGiftSprite;
            giftButtonImage.enabled = (currentGiftSprite != null);
        }

        if (giftGroupRoot != null) giftGroupRoot.SetActive(true);
        else gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentGiftSprite = null;

        if (giftGroupRoot != null) giftGroupRoot.SetActive(false);
        else gameObject.SetActive(false);
    }

    private void OnClickGift()
    {
        TryRegisterToDefaultSlots();
        Hide();
    }

    private bool TryRegisterToDefaultSlots()
    {
        if (defaultGroupUI == null) return false;
        if (currentGiftSprite == null) return false;

        Image[] slots = defaultGroupUI.giftSlot;
        if (slots == null || slots.Length == 0) return false;

        for (int i = 0; i < slots.Length; i++)
        {
            Image slot = slots[i];
            if (slot == null) continue;

            //너가 말한 방식: "빈칸" = sprite가 null
            if (slot.sprite == null)
            {
                slot.sprite = currentGiftSprite;
                slot.enabled = true;

                var c = slot.color;
                c.a = 1f;
                slot.color = c;
                return true;
            }
        }
        return false; // 빈칸 없음
    }
}
