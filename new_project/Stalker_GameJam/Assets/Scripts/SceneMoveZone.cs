using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

public class SceneMoveZone : MonoBehaviour
{
    [Tooltip("이동하고 싶은 씬의 이름을 정확히 적으세요")]
    public string targetSceneName;

    // BoxCollider(IsTrigger 체크된)에 무언가 들어왔을 때 실행
    private void OnTriggerEnter(Collider other)
    {
        // 들어온 물체의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 감지! 씬 이동 중...");
            SceneManager.LoadScene(targetSceneName);
        }
    }
}