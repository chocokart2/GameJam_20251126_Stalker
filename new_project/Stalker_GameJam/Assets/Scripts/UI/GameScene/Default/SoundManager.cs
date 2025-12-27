using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

[System.Serializable]
public class SoundData
{
    public string name;     // 소리 이름 (예: "Button_Click")
    public AudioClip clip;  // 실제 파일
}

public class SoundManager : MonoBehaviour
{
    // [1] 싱글톤 패턴: 어디서든 SoundManager.Instance로 접근 가능
    public static SoundManager Instance;

    private const string PREF_MASTER = "PREF_MASTER_VOLUME";

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string masterParam = "MasterVolume";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Sound Library")]
    [SerializeField] private List<SoundData> soundList; // 인스펙터에서 등록할 리스트
    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();

    public float Master01 { get; private set; } = 1.0f;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음

            // 리스트를 딕셔너리로 변환 (빠른 검색을 위해)
            foreach (var sound in soundList)
            {
                if (!soundDict.ContainsKey(sound.name))
                    soundDict.Add(sound.name, sound.clip);
            }

            Load();
            Apply();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- 재생 메서드 (이름으로 호출) ---

    public void PlayBgm(string name)
    {
        if (soundDict.TryGetValue(name, out AudioClip clip))
        {
            if (bgmSource.clip == clip) return; // 이미 재생 중이면 무시
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else Debug.LogWarning($"BGM 못찾음: {name}");
    }

    public void PlaySfx(string name)
    {
        if (soundDict.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else Debug.LogWarning($"SFX 못찾음: {name}");
    }

    public void PlayUi(string name)
    {
        if (soundDict.TryGetValue(name, out AudioClip clip))
        {
            uiSource.PlayOneShot(clip); // UI 소리는 겹쳐도 되므로 OneShot
        }
        else Debug.LogWarning($"UI Sound 못찾음: {name}");
    }

    // --- 기존 볼륨 조절 로직 유지 ---

    public void SetMaster(float value01)
    {
        Master01 = Mathf.Clamp01(value01);
        Apply();
        PlayerPrefs.SetFloat(PREF_MASTER, Master01);
    }

    public void Load()
    {
        Master01 = PlayerPrefs.GetFloat(PREF_MASTER, 1.0f);
    }

    public void Apply()
    {
        if (mixer == null) return;
        mixer.SetFloat(masterParam, Linear01ToDb(Master01));
    }

    private float Linear01ToDb(float v01)
    {
        float v = Mathf.Max(0.0001f, v01);
        return Mathf.Log10(v) * 20.0f;
    }
}
