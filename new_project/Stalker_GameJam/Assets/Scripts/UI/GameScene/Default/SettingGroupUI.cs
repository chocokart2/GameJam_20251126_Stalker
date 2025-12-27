using UnityEngine;
using UnityEngine.UI;

public class SettingsGroupUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool pauseGameWhenOpen = true;

    [Header("Sound")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private Slider masterSlider;   // 너 Hierarchy의 AudioMix/Slider 연결

    [Header("Buttons")]
    [SerializeField] private Button exitButton;     // 너 Hierarchy의 Button(Exit) 연결

    private bool isOpen;

    private void Awake()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(Close);

        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);

        SetOpen(false);
    }

    private void Start()
    {
        // 저장된 값으로 슬라이더 초기화
        if (soundManager != null && masterSlider != null)
            masterSlider.SetValueWithoutNotify(soundManager.Master01);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen) Close();
            else Open();
        }
    }

    public void Open() => SetOpen(true);
    public void Close() => SetOpen(false);

    private void SetOpen(bool open)
    {
        isOpen = open;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = open ? 1.0f : 0.0f;
            canvasGroup.interactable = open;
            canvasGroup.blocksRaycasts = open;
        }
        else
        {
            // CanvasGroup 안 쓰면 이 방식(단, 이렇게 꺼버리면 ESC로 다시 못 켤 수 있음)
            gameObject.SetActive(open);
        }

        if (pauseGameWhenOpen)
            Time.timeScale = open ? 0.0f : 1.0f;

        // 열 때 슬라이더 값 동기화(혹시 외부에서 값 바뀌었을 때)
        if (open && soundManager != null && masterSlider != null)
            masterSlider.SetValueWithoutNotify(soundManager.Master01);
    }

    private void OnMasterSliderChanged(float value)
    {
        if (soundManager != null)
            soundManager.SetMaster(value);
    }
}
