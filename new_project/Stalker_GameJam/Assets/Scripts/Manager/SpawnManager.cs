using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    static public SpawnManager instance;

    [SerializeField] GameObject rangeMonster;
    [SerializeField] GameObject meleeMonster;
    float enemyBuff = 1.0f;
    public Vector2 SpawnRangeMinMax;
    Coroutine DefaultSpawnCoroutine;
    Coroutine QuestSpawnCoroutine;

    public void WhenPlayerDeath()
    {
        if (DefaultSpawnCoroutine != null)
            StopCoroutine(DefaultSpawnCoroutine);
        if (QuestSpawnCoroutine != null)
            StopCoroutine(QuestSpawnCoroutine);
    }

    public void BeginWave1Coroutine()
    {
        GameManager.instance.currentStatus = EWaveStatus.Wave1;
        QuestSpawnCoroutine = null;

        DefaultGroupUI.instance.playerCurrentObjectText.text = "Wave 1 Started!";
        PlayerUI.instance.missionIndex++;

        IEnumerator MySpawnCoroutine()
        {
            for (int i = 0; i < 5; ++i)
            {
                Instantiate(rangeMonster, GetSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < 20; ++i)
            {
                Instantiate(meleeMonster, GetSpawnPosition(), Quaternion.identity);
            }
            yield return new WaitForSeconds(40f);
            // 여기에 보상
            GameManager.instance.currentStatus = EWaveStatus.Default2;
            DefaultGroupUI.instance.playerCurrentObjectText.text = "Go To Shop 2!";
            PlayerUI.instance.missionIndex++;
        }

        QuestSpawnCoroutine = StartCoroutine(MySpawnCoroutine());
    }

    public void BeginWave2Coroutine()
    {
        GameManager.instance.currentStatus = EWaveStatus.Wave2;
        QuestSpawnCoroutine = null;

        DefaultGroupUI.instance.playerCurrentObjectText.text = "Wave 2 Started!";
        PlayerUI.instance.missionIndex++;

        IEnumerator MySpawnCoroutine()
        {
            for (int i = 0; i < 15; ++i)
            {
                Instantiate(rangeMonster, GetSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < 25; ++i)
            {
                Instantiate(meleeMonster, GetSpawnPosition(), Quaternion.identity);
            }
            yield return new WaitForSeconds(40f);
            // 여기에 보상
            GameManager.instance.currentStatus = EWaveStatus.Default3;
            DefaultGroupUI.instance.playerCurrentObjectText.text = "Go To Shop 3!";
            PlayerUI.instance.missionIndex++;


        }

        QuestSpawnCoroutine = StartCoroutine(MySpawnCoroutine());
    }

    public void BeginWave3Coroutine()
    {
        GameManager.instance.currentStatus = EWaveStatus.Wave3;
        QuestSpawnCoroutine = null;

        DefaultGroupUI.instance.playerCurrentObjectText.text = "Wave 3 Started!";
        PlayerUI.instance.missionIndex++;

        IEnumerator MySpawnCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < 10; ++i)
                {
                    Instantiate(rangeMonster, GetSpawnPosition(), Quaternion.identity);
                }
                for (int i = 0; i < 10; ++i)
                {
                    Instantiate(meleeMonster, GetSpawnPosition(), Quaternion.identity);
                }
                yield return new WaitForSeconds(20f);
            }
        }

        QuestSpawnCoroutine = StartCoroutine(MySpawnCoroutine());
    }

    public void ApplyEnemyBuff(float value)
    {
        enemyBuff *= value;
    }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        BeginDefaultSpawnCoroutine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginDefaultSpawnCoroutine()
    {
        if (DefaultSpawnCoroutine != null)
            StopCoroutine(DefaultSpawnCoroutine);

        IEnumerator MySpawnCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < 5; ++i)
                {
                    Instantiate(rangeMonster, GetSpawnPosition(), Quaternion.identity);
                }
                for (int i = 0; i < 5; ++i)
                {
                    Instantiate(meleeMonster, GetSpawnPosition(), Quaternion.identity);
                }
                yield return new WaitForSeconds(30f);
            }
        }

        DefaultSpawnCoroutine = StartCoroutine(MySpawnCoroutine());
    }


    private Vector3 GetSpawnPosition()
    {
        float rad = Random.Range(0f, Mathf.PI * 2f);
        Vector3 result = Player.instance.transform.position +
            (new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad))
                * Random.Range(SpawnRangeMinMax.x, SpawnRangeMinMax.y));
        result.y = 1f;
        return result;
    }
}
