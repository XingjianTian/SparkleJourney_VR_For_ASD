using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DrawSceneMgr : MonoBehaviour
{
    public InputActionReference rightBKey;
    // Start is called before the first frame update
    void Start()
    {
        RobotObj.Instance.SpeechText("小朋友，欢迎来到3D绘画空间，在这里你可以尽情创作",(() =>
        {
            TimerMgr.Instance.StartTimer(1, () =>
            {
                RobotObj.Instance.SpeechText("当你不想完时，可以按下右手柄的B键可以返回房间哦",(() =>
                {
                }));
            });
        }));
        rightBKey.action.performed += Back2Home;
    }

    private void Back2Home(InputAction.CallbackContext context)
    {
        rightBKey.action.performed -= Back2Home;
        SceneManager.LoadSceneAsync("1_HomeScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
