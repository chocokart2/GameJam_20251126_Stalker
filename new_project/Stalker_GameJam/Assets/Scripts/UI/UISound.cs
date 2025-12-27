using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 인터페이스 사용을 위해 필요

// 버튼에 이 스크립트를 붙이기만 하면 됩니다.
public class UISound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [Header("사운드 이름 입력")]
    public string clickSoundName = "Button_Click"; // 기본값
    public string hoverSoundName = "Button_Hover"; // 마우스 올렸을 때 (필요 없다면 비워두기)

    // 클릭했을 때 자동 실행
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlayUi(clickSoundName);
    }

    // 마우스 올렸을 때 (선택사항)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(hoverSoundName))
            SoundManager.Instance.PlayUi(hoverSoundName);
    }
}