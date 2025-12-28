using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUIController : MonoBehaviour
{
    [SerializeField] private float showSeconds = 2.0f;
    [SerializeField] private string titleSceneName = "TitleScene"; // 네 타이틀 씬 이름으로

    private bool started;

    private void Awake()
    {
        gameObject.SetActive(false); // 시작 비활성 안전장치
    }

    public void ShowAndGoTitle()
    {
        if (started) return;
        started = true;

        gameObject.SetActive(true);

        // 선택: 죽었을 때 커서 풀기
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Invoke(nameof(GoTitle), showSeconds);
    }

    private void GoTitle()
    {
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlayBgm("MainMenu");
        SceneManager.LoadScene(titleSceneName);
    }
}
