using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuilding : MonoBehaviour
{
    [SerializeField] int buildWave; // 1~3의 값. 적정한 대상이여야만 가능
    [SerializeField] float radius; // 상호작용 범위
    float radSqur;

    // Start is called before the first frame update
    void Start()
    {
        radSqur = radius * radius;
    }

    // Update is called once per frame
    void Update()
    {
        // 상호작용을 누른 상태일 것
        if (Input.GetKeyDown(KeyCode.E) == false)
        {
            return;
        }
        // 플레이어랑 가까울 것
        if ((transform.position - Player.instance.transform.position).sqrMagnitude
            < radSqur)
        {
            // 적절한 웨이브에 맞는 건물일 것
            switch (GameManager.instance.currentStatus)
            {
                case EWaveStatus.Default1:
                    if (buildWave != 1) return;
                    SpawnManager.instance.BeginWave1Coroutine();
                    break;
                case EWaveStatus.Default2:
                    if (buildWave != 2) return;
                    SpawnManager.instance.BeginWave2Coroutine();
                    break;
                case EWaveStatus.Default3:
                    if (buildWave != 3) return;
                    SpawnManager.instance.BeginWave3Coroutine();
                    break;
                case EWaveStatus.Wave3:
                    if (buildWave != 4) return;
                    SpawnManager.instance.GoalReached();
                    break;
                default:
                    // "No".
                    return;
            }
        }
    }
}
