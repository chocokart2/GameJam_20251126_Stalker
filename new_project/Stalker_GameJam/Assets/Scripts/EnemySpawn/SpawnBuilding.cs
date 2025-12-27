using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBuilding : MonoBehaviour
{
    [SerializeField] int buildWave; // 1~3의 값. 적정한 대상이여야만 가능


    // 스파게티 경보
    void OnTriggerStay(Collider other)
    {
        // 플레이어 일 것
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        // 상호작용을 누른 상태일 것
        if (Input.GetKeyDown(KeyCode.E) == false)
        {
            return;
        }

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
            default:
                // "No".
                return;
        }

    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
