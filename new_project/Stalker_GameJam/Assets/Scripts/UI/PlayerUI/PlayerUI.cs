using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    static public PlayerUI instance;

    public List<Transform> MissionPosition;
    public int missionIndex;
    public Transform navigatorArrow;
    //public GameObject PushCooltimeUI;

    public void CallWhenDeath()
    {
        instance.missionIndex = 0;
        instance = null;
    }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        navigatorArrow.LookAt(MissionPosition[missionIndex]);
        // PushCooltimeUI.transform.forward = Camera.main.transform.forward;
    }
}
