using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

public class Game1Base : MonoBehaviour
{
    [Header("玩家头部")]
    public Transform playerHead;
    [Header("XRSocket组件")]
    public XRSocketInteractor socket;
    [Header("机器人站位")]
    public Transform robotPosition;
    [Header("方块出现位置")]
    public Transform[] points;

    [Header("放置成功的提示语")] public string[] putText;

    [Header("放置失败的提示语")] public string[] putFailText;
    
    [Header("放置下一个物品的提示语")] public string[] putDownText;
    //当前需要放置的物体颜色
    private Color currentColor;
    //当前需要放置的物体形状
    private string currentShape;
    //关卡1判断是否放置
    private bool judgeNext;
    //关卡2判断是否放置
    private bool judgeNext2;
    //通用当前游戏进度
    private int currentIndex;
    //关卡1预制体 str
    private string[] level1Prefabs = {
        "Prefabs/hyh/Game1/level1/BlueCube",
        "Prefabs/hyh/Game1/level1/GreenCube",
        "Prefabs/hyh/Game1/level1/YellowCube",
        "Prefabs/hyh/Game1/level1/RedCube",
        "Prefabs/hyh/Game1/level1/QinCube",
        "Prefabs/hyh/Game1/level1/ZiCube"
    };
    //关卡2预制体 str
    private string[] level2Prefabs = {
        "Prefabs/hyh/Game1/level2/BlueShpere",
        "Prefabs/hyh/Game1/level2/GreenShpere",
        "Prefabs/hyh/Game1/level2/YellowShpere",
        "Prefabs/hyh/Game1/level2/RedShpere",
        "Prefabs/hyh/Game1/level2/QinShpere",
        "Prefabs/hyh/Game1/level2/ZiShpere",
        "Prefabs/hyh/Game1/level2/BlueCylinder",
        "Prefabs/hyh/Game1/level2/GreenCylinder",
        "Prefabs/hyh/Game1/level2/YellowCylinder",
        "Prefabs/hyh/Game1/level2/RedCylinder",
        "Prefabs/hyh/Game1/level2/QinCylinder",
        "Prefabs/hyh/Game1/level2/ZiCylinder"
    };
    //关卡1的六个预制体
    private GameObject[] level1CubeList = new GameObject[0];
    //关卡2的六个预制体
    private GameObject[] level2CubeList = new GameObject[0] ;
    //随机报点
    private List<int> RandomList = new List<int>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RobotObj.Instance.SpeechText("你要玩这个游戏嘛,让我们开始吧",GameLevel1Start);
        }
    }
    //离开的时候初始化数据
    private void OnTriggerExit(Collider other)
    {
        judgeNext = false;
        currentIndex = 0;
        if (level1CubeList != null)
        {
            foreach (var item in level1CubeList)
            {
                Destroy(item);
            }
        }

        if (level2CubeList != null)
        {
            foreach (var item in level2CubeList)
            {
                Destroy(item);
            }
        }


    }
    private Tween lookAtTween;
    public float lookAtSpeed = 0.5f;
    //关卡1开始
    void GameLevel1Start()
    {
        RobotObj.Instance.SetDestination(robotPosition.position,(() =>
        {
            // 如果正在执行旋转Tween，先杀掉它
            if (lookAtTween != null && lookAtTween.IsActive())
            {
                lookAtTween.Kill();
            }
            // 计算需要面向的方向（保持Y轴不变，只在水平面上旋转）
            Vector3 targetPosition = PlayerObj.Instance.transform.position;
            Vector3 robotPosition = RobotObj.Instance.transform.position;
            targetPosition.y = robotPosition.y; // 保持y轴高度相同

            // 计算朝向
            Vector3 direction = targetPosition - robotPosition;
            if (direction != Vector3.zero) // 防止零向量
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                lookAtTween = RobotObj.Instance.transform.DORotateQuaternion(targetRotation, lookAtSpeed)
                    .SetEase(Ease.OutSine);
            }
        }));
        currentIndex = 0;
        RandomList.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            RandomList.Add(i);
        }
        socket.selectEntered.AddListener(JudgeObj);
        level1CubeList = new GameObject[points.Length];
        Shuffle(level1Prefabs);
        //生成第一关的六个方块
        for (int i = 0; i < points.Length; i++)
        {
            level1CubeList[i] = Instantiate(Resources.Load<GameObject>(level1Prefabs[i]),transform);
            level1CubeList[i].transform.position = points[i].position;
        }
        Judge(level1CubeList);
    }
    //关卡2开始
    void GameLevel2Start()
    {

        currentIndex = 0;
        RandomList.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            RandomList.Add(i);
        }
        socket.selectEntered.RemoveListener(JudgeObj);
        socket.selectEntered.AddListener(JudgeObj2);
        level2CubeList = new GameObject[points.Length];
        Shuffle(level2Prefabs);
        for (int i = 0; i < points.Length; i++)
        {
            level2CubeList[i] = Instantiate(Resources.Load<GameObject>(level2Prefabs[i]),transform);
            level2CubeList[i].transform.position = points[i].position;
        }
        Judeg2(level2CubeList);
    }
    //判断是否放置正确,颜色正确
    void JudgeObj(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.GetComponent<MeshRenderer>().material.color == currentColor)
        {
            Destroy(arg0.interactableObject.transform.gameObject);
            currentIndex++;
            print(currentIndex);
            if (currentIndex == 6)
            {
                RobotObj.Instance.SpeechText("你真是太厉害了，那我们现在增加一点难度",GameLevel2Start);
                return;
            }
            string newputText = putText[Random.Range(0, putText.Length)];
            RobotObj.Instance.SpeechText(newputText, () =>
            {
                Judge(level1CubeList);
            });
        }
        else
        {
            string newputFailText = putFailText[Random.Range(0, putFailText.Length)];
            RobotObj.Instance.SpeechText(newputFailText);
        }
    }
    //判断是否放置正确,颜色和形状正确
    void JudgeObj2(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.GetComponent<MeshRenderer>().material.color == currentColor &&
            ShapeTurnText(arg0.interactableObject.transform.GetComponent<MeshFilter>()) == currentShape)
        {
            Destroy(arg0.interactableObject.transform.gameObject);
            currentIndex++;
            if (currentIndex == 6)
            {
                RobotObj.Instance.SpeechText("你真是太厉害了，游戏结束啦，我们去玩其他游戏吧", StopAllCoroutines);
                return;
            }
            string newputText = putText[Random.Range(0, putText.Length)];
            RobotObj.Instance.SpeechText(newputText, () =>
            {
                Judeg2(level2CubeList);
            });
        }
        else
        {
            string newputFailText = putFailText[Random.Range(0, putFailText.Length)];
            RobotObj.Instance.SpeechText(newputFailText);
        }
    }
    //下一个方块
    void Next(string speakText,Color c)
    {
        RobotObj.Instance.SpeechText(speakText);
        currentColor = c;
    }
    void Next(string speakText,Color c,string ShapeText)
    {
        RobotObj.Instance.SpeechText(speakText);
        currentColor = c;
        currentShape = ShapeText;
    }
    //打乱数组
    static void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0,i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
    //颜色转文字
    string ColorTurnText(Color c)
    {
        if (c == Color.blue) return "蓝色";
        if (c == Color.green) return "绿色";
        if (c == Color.yellow) return "黄色";
        if (c == Color.red) return "红色";
        if (c == Color.cyan) return "青色";
        if (c == Color.magenta) return "紫色";
        return "";
    }
    //形状转文字
    string ShapeTurnText(MeshFilter filter)
    {
        if (filter == null || filter.mesh == null)
        {
            return "";
        }

        string meshName = filter.mesh.name.ToLower(); // 将名称转换为小写，忽略大小写

        if (meshName.Contains("cube")) return "方块";
        if (meshName.Contains("sphere")) return "小球";
        if (meshName.Contains("cylinder")) return "圆柱";

        return "";
    }
    //判断是否放置正确，并随机下一个方块
    void Judge(GameObject[] objList)
    {
        int randomIndex = Random.Range(0, RandomList.Count);
        //当前需要放置物品的颜色计算
        string currentColorText = ColorTurnText(objList[RandomList[randomIndex]].GetComponent<MeshRenderer>().material.color);
        //当前需要放置物品的形状计算
        string currentMeshText = ShapeTurnText(objList[RandomList[randomIndex]].GetComponent<MeshFilter>());
        //当前说的话
        string newputDownText = putDownText[Random.Range(0, putDownText.Length)];
        Next(newputDownText + currentColorText + currentMeshText,objList[RandomList[randomIndex]].GetComponent<MeshRenderer>().material.color);
        RandomList.Remove(RandomList[randomIndex]);
    }

    void Judeg2(GameObject[] objList)
    {
        int randomIndex = Random.Range(0, RandomList.Count);
        string currentColorText = ColorTurnText(objList[RandomList[randomIndex]].GetComponent<MeshRenderer>().material.color) ;
        string currentMeshText = ShapeTurnText(objList[RandomList[randomIndex]].GetComponent<MeshFilter>());
        string newputDownText = putDownText[Random.Range(0, putDownText.Length)];
        Next(newputDownText + currentColorText + currentMeshText,objList[RandomList[randomIndex]].GetComponent<MeshRenderer>().material.color,currentMeshText);
        RandomList.Remove(RandomList[randomIndex]);
    }
}

