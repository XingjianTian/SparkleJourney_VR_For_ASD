using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public float cdTime = 1;
    public bool cding;
    
    private static GameMgr instance;
    public static GameMgr Instance => instance;
    private void Awake()
    {
        instance = this;
    }
    
    public string ColorToText(ColorName name)
    {
        switch (name)
        {
            case ColorName.Blue:
                return "Prefabs/hyh/HomeGame/Blue";
            case ColorName.Green:
                return "Prefabs/hyh/HomeGame/Green";
            case ColorName.Red:
                return "Prefabs/hyh/HomeGame/Red";
            case ColorName.Yellow:
                return "Prefabs/hyh/HomeGame/Yellow";
            case ColorName.Orange:
                return "Prefabs/hyh/HomeGame/Orange";
            case ColorName.Zi:
                return "Prefabs/hyh/HomeGame/Zi";
            default:
                return "";
        }
    }
    
    public void CDJudge()
    {
        if (!cding) return;
        cdTime -= Time.deltaTime;
        if (cdTime <= 0)
        {
            cding = false;
            cdTime = 1;
        }
    }
}
