using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMgr : MonoSingleton<RoomMgr>
{



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RobotObj.Instance.SetDestination(PlayerObj.Instance.transform.position);
    }
}
