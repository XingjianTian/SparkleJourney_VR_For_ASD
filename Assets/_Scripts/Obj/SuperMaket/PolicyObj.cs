using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicyObj : MonoSingleton<PolicyObj>
{
    private Animator _animator;

    public List<Transform> wayPointList;

    public PassageController p;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        SayHellow();
        StartCoroutine(HandlePassage());
    }

    private bool isShouldPass;
    IEnumerator HandlePassage()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (true)
        {
            if (p.IsPassageAllowed() && !isShouldPass)
            {
                isShouldPass = true;
                RobotObj.Instance.SpeechText("就是现在，通过路口，然后进入超市吧吧");
                NavUtility.Instance.ShowNavPath(PlayerObj.Instance.transform, wayPointList[0].position);
                for (int i = 0; i < wayPointList.Count-1; i++)
                {

                    NavUtility.Instance.ShowNavPath(wayPointList[i], wayPointList[i + 1].position);
                }
            }
            // else if (!p.IsPassageAllowed()&& isShouldPass)
            // {
            //     isShouldPass = false;
            //     RobotObj.Instance.SpeechText("不着急，我们可以等待下一个绿灯哦");
            //     NavUtility.Instance.HideAllNavPath();
            // }

            yield return waitForSeconds;
        }
    }

    public void SayHellow()
    {
        _animator.SetTrigger("Hellow");
        Text2SpeechHandle.Instance.Send("你好呀，小朋友！",(clip =>
        {
            TimerMgr.Instance.StartTimer(4f, (() =>
            {
                MusicMgr.Instance.effCore.PlaySound(clip);
                RobotObj.Instance.SpeechText("今天我们的任务是去超市购物，你可以买点喜欢的商品哦", () =>
                {
                    RobotObj.Instance.SpeechText("现在我们要先通过马路，记住，只有在绿灯的时候才能通过哦");
                });
            }));
        }),tone: "3");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
