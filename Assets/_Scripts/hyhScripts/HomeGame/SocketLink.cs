using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketLink : MonoBehaviour
{
    public XRSocketInteractor[] socketList;
    public XRSocketInteractor[] disSocketList1;
    public XRSocketInteractor[] disSocketList2;
    public XRSocketInteractor[] disSocketList3;
    public XRSocketInteractor[] disSocketList4;
    private List<XRSocketInteractor> disSocketList = new (); //所有方向的 DisSocket
    public bool isStartShow;
    private XRSocketInteractor thisSocket;
    public int ShowWeighted; //显示权重
    public bool isShowWeightedCompute = true; //是否参与权重计算
    void Start()
    {
        thisSocket = GetComponent<XRSocketInteractor>();
        thisSocket.selectEntered.AddListener((obj) =>
        {
            foreach (var socket in socketList)
            {
                if (!isShowWeightedCompute)
                {
                    print("跳过显示");
                    return;
                }
                //当放下一个物体时 对应的 Socket 列表 显示权重 +1
                socket.GetComponent<SocketLink>().ShowWeighted += 1;
                //显示权重不是 0 就显示
                if (socket.GetComponent<SocketLink>().ShowWeighted != 0)
                {
                    socket.gameObject.SetActive(true);
                }
            }

            print("列表1：");
            DisJudge(disSocketList1,false);
            print("列表2：");
            DisJudge(disSocketList2,false);
            print("列表3：");
            DisJudge(disSocketList3,false);
            print("列表4：");
            DisJudge(disSocketList4,false);
            
            foreach (var socket in disSocketList)
            {
                print("隐藏：" + socket.name);
                if (socket.GetComponent<SocketLink>().ShowWeighted == 0 && !socket.GetComponent<SocketLink>().isShowWeightedCompute)
                {
                    print("隐藏成功：" + socket.name);
                    socket.gameObject.SetActive(false);
                }
            }
        });
        thisSocket.selectExited.AddListener((obj) =>
        {
            foreach (var socket in socketList)
            {
                if (!isShowWeightedCompute) return;
                //当拿走一个物体时 对应的 Socket 列表 显示权重 -1
                socket.GetComponent<SocketLink>().ShowWeighted -= 1;
                //显示权重是 0 就隐藏
                if (socket.GetComponent<SocketLink>().ShowWeighted == 0)
                {
                    print("松手隐藏成功:" + socket.name);
                    socket.gameObject.SetActive(false);
                }
            }
            print("松手列表1：");
            DisJudge(disSocketList1,true);
            print("松手列表2：");
            DisJudge(disSocketList2,true);
            print("松手列表3：");
            DisJudge(disSocketList3,true);
            print("松手列表4：");
            DisJudge(disSocketList4,true);
            
            foreach (var socket in disSocketList)
            {
                print("显示：" + socket.name);
                if (socket.GetComponent<SocketLink>().ShowWeighted != 0 && socket.GetComponent<SocketLink>().isShowWeightedCompute)
                {
                    print("显示成功：" + socket.name);
                    socket.gameObject.SetActive(true);
                }
            }
        });
        for (int i = 0; i < 3; i++)
        {
            if (disSocketList1[i] != null) disSocketList.Add(disSocketList1[i]);
            if (disSocketList2[i] != null) disSocketList.Add(disSocketList2[i]);
            if (disSocketList3[i] != null) disSocketList.Add(disSocketList3[i]);
            if (disSocketList4[i] != null) disSocketList.Add(disSocketList4[i]);
        }
        if (isStartShow) return;
        gameObject.SetActive(false);
    }

    void DisJudge(XRSocketInteractor[] disList,bool isShow)
    {
        if (disList[0] != null)
        {
            if (disList[0].hasSelection)
            {
                print(disList[0].name + "有东西");
                if (disList[1] == null) return;
                SocketLink index2SocketLink = disList[1].GetComponent<SocketLink>();
                index2SocketLink.isShowWeightedCompute = isShow;
                index2SocketLink.ShowWeighted = isShow ? 1 : 0;
                if (disList[2] == null) return;
                SocketLink index3SocketLink = disList[2].GetComponent<SocketLink>();
                index3SocketLink.isShowWeightedCompute = isShow;
                index3SocketLink.ShowWeighted = isShow ? 1 : 0;
            }
            else
            {
                print(disList[0].name + "没有东西");
            }
        }
        if (disList[1] != null)
        {
            if (disList[1].hasSelection)
            {
                print(disList[1].name + "有东西");
                SocketLink index1SocketLink = disList[0].GetComponent<SocketLink>();
                index1SocketLink.isShowWeightedCompute = isShow;
                index1SocketLink.ShowWeighted = isShow ? 1 : 0;
                if (disList[2] == null) return;
                SocketLink index2SocketLink = disList[2].GetComponent<SocketLink>();
                index2SocketLink.isShowWeightedCompute = isShow;
                index2SocketLink.ShowWeighted = isShow ? 1 : 0;
            }
            else
            {
                print(disList[1].name + "没有东西");
            }
        }
        if (disList[2] != null)
        {
            if (disList[2].hasSelection)
            {
                print(disList[2].name + "有东西");
                SocketLink index1SocketLink = disList[0].GetComponent<SocketLink>();
                SocketLink index2SocketLink = disList[1].GetComponent<SocketLink>();
                index1SocketLink.isShowWeightedCompute = index2SocketLink.isShowWeightedCompute = isShow;
                index1SocketLink.ShowWeighted = index2SocketLink.ShowWeighted = isShow ? 1 : 0;
            }
            else
            {
                print(disList[2].name + "没有东西");
            }
        }
    }
}
