using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleSceneMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BeginSceneMgr.currentMode = "End";
        RobotObj.Instance.SpeechText("欢迎来到多人联机空间，和其他小朋友一起完成创作吧");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
