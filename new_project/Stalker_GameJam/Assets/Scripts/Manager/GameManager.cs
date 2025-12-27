using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;

    public EWaveStatus currentStatus;

    public void Awake()
    {
        instance = this;
        currentStatus = EWaveStatus.Default1;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDestroy()
    {
        currentStatus = EWaveStatus.Default1;
        instance = null;
    }
}
