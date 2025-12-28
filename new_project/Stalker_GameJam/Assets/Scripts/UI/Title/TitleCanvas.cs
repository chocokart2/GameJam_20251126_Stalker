using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleCanvas : MonoBehaviour
{
    public void Awake()
    {
        SoundManager.Instance.PlayBgm("MainMenu");
    }

    public void OnClickStart() 
    { 
        Time.timeScale = 1f;

        SceneManager.LoadScene("daehak_Scene");
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlayBgm("IngameBGM");
    }

    public void OnClickExit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
