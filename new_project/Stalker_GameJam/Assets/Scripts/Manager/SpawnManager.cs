using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    static public SpawnManager instance;
    
    [System.Serializable]
    public class NestedTuple
    {
        public EWaveStatus currentStatus;
        public int rangeMonsterSpawnCount;
        public int meleeMonsterSpawnCount;
        public GameObject rangeMonsterObject;
        public GameObject meleeMonsterObject;
        public float spawnInterval;
    }


    [SerializeField] GameObject rangeMonster;
    [SerializeField] GameObject meleeMonster;
    [SerializeField] Sprite GiftSprite1;
    [SerializeField] Sprite GiftSprite2;
    [SerializeField] Sprite GiftSprite3;
    [SerializeField] NestedTuple[] MonsterSpawnRules;
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
        NestedTuple rule = GetTuple(EWaveStatus.Wave1);
        Debug.Assert(rule != null, "SpawnManager: No spawn rule for Wave1");

        GameManager.instance.currentStatus = EWaveStatus.Wave1;
        QuestSpawnCoroutine = null;

        DefaultGroupUI.instance.playerCurrentObjectText.text = "40초간 가게 방어";
        PlayerUI.instance.missionIndex++;

        IEnumerator MySpawnCoroutine()
        {
            for (int i = 0; i < rule.rangeMonsterSpawnCount; ++i)
            {
                Instantiate(rule.rangeMonsterObject, GetSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < rule.meleeMonsterSpawnCount; ++i)
            {
                Instantiate(rule.meleeMonsterObject, GetSpawnPosition(), Quaternion.identity);
            }
            yield return new WaitForSeconds(40f);
            //yield return new WaitForSeconds(4f);
            // 여기에 보상
            GameManager.instance.currentStatus = EWaveStatus.Default2;
            DefaultGroupUI.instance.playerCurrentObjectText.text = "두번째 선물 가게로 이동";
            PlayerUI.instance.missionIndex++;
            GiftUI.instance.Show(GiftSprite1);
        }

        QuestSpawnCoroutine = StartCoroutine(MySpawnCoroutine());
    }

    public void BeginWave2Coroutine()
    {
        NestedTuple rule = GetTuple(EWaveStatus.Wave2);
        Debug.Assert(rule != null, "SpawnManager: No spawn rule for Wave2");

        GameManager.instance.currentStatus = EWaveStatus.Wave2;
        QuestSpawnCoroutine = null;

        DefaultGroupUI.instance.playerCurrentObjectText.text = "60초간 가게 방어";
        PlayerUI.instance.missionIndex++;

        IEnumerator MySpawnCoroutine()
        {
            for (int i = 0; i < rule.rangeMonsterSpawnCount; ++i)
            {
                Instantiate(rule.rangeMonsterObject, GetSpawnPosition(), Quaternion.identity);
            }
            for (int i = 0; i < rule.meleeMonsterSpawnCount; ++i)
            {
                Instantiate(rule.meleeMonsterObject, GetSpawnPosition(), Quaternion.identity);
            }
            yield return new WaitForSeconds(60f);
            //yield return new WaitForSeconds(4f);
            // 여기에 보상
            GameManager.instance.currentStatus = EWaveStatus.Default3;
            DefaultGroupUI.instance.playerCurrentObjectText.text = "선물을 장식할 예쁜 꽃 찾기";
            PlayerUI.instance.missionIndex++;

            GiftUI.instance.Show(GiftSprite2);
        }

        QuestSpawnCoroutine = StartCoroutine(MySpawnCoroutine());
    }

    public void BeginWave3Coroutine()
    {
        NestedTuple rule = GetTuple(EWaveStatus.Wave3);
        Debug.Assert(rule != null, "SpawnManager: No spawn rule for Wave3");

        GameManager.instance.currentStatus = EWaveStatus.Wave3;
        QuestSpawnCoroutine = null;
        GiftUI.instance.Show(GiftSprite3);

        DefaultGroupUI.instance.playerCurrentObjectText.text = "사랑하는 너의 집으로 이동";
        PlayerUI.instance.missionIndex++;

        IEnumerator MySpawnCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < rule.rangeMonsterSpawnCount; ++i)
                {
                    Instantiate(rule.rangeMonsterObject, GetSpawnPosition(), Quaternion.identity);
                }
                for (int i = 0; i < rule.meleeMonsterSpawnCount; ++i)
                {
                    Instantiate(rule.meleeMonsterObject, GetSpawnPosition(), Quaternion.identity);
                }
                yield return new WaitForSeconds(rule.spawnInterval);
            }
        }

        QuestSpawnCoroutine = StartCoroutine(MySpawnCoroutine());
    }

    public void GoalReached()
    {
        Debug.Log("!!!! Goal Reached! !!!!");
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
                if (GameManager.instance.currentStatus != EWaveStatus.Default1 &&
                    GameManager.instance.currentStatus != EWaveStatus.Default2 &&
                    GameManager.instance.currentStatus != EWaveStatus.Default3)
                {
                    yield return null;
                    continue;
                }
                
                NestedTuple rule = GetTuple(GameManager.instance.currentStatus);

                for (int i = 0; i < rule.rangeMonsterSpawnCount; ++i)
                {
                    Instantiate(rule.rangeMonsterObject, GetSpawnPosition(), Quaternion.identity);
                }
                for (int i = 0; i < rule.meleeMonsterSpawnCount; ++i)
                {
                    Instantiate(rule.meleeMonsterObject, GetSpawnPosition(), Quaternion.identity);
                }
                yield return new WaitForSeconds(rule.spawnInterval);
            }
        }

        DefaultSpawnCoroutine = StartCoroutine(MySpawnCoroutine());
    }

    private NestedTuple GetTuple(EWaveStatus status)
    {
        foreach (NestedTuple tuple in MonsterSpawnRules)
        {
            if (tuple.currentStatus == status)
                return tuple;
        }
        return null;
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
