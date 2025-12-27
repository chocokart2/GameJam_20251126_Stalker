using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleCanvas : MonoBehaviour
{
    public void OnClickStart() 
    { 
        Time.timeScale = 1f;

        SceneManager.LoadScene("daehak_Scene");
    }

    public void OnClickExit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
