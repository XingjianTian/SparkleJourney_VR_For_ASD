using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : GameMgr
{
    private string[] objList = 
    {
        "Prefabs/hyh/HomeGame/Blue",
        "Prefabs/hyh/HomeGame/Red",
        "Prefabs/hyh/HomeGame/Yellow",
        "Prefabs/hyh/HomeGame/Green",
        "Prefabs/hyh/HomeGame/Orange",
        "Prefabs/hyh/HomeGame/Zi"
    };
    private void OnTriggerEnter(Collider other)
    {
        if (cding) return;
        if (other.CompareTag("HomeObj"))
        {
            string resourcesText = GameMgr.Instance.ColorToText(other.transform.parent.GetComponent<ColorID>().ObjColorName);
            int index = Array.IndexOf(objList, resourcesText) + 1;
            if (index >= objList.Length) index = 0;
            Destroy(other.transform.parent.gameObject);
            Instantiate(Resources.Load<GameObject>(objList[index]), other.transform.position, other.transform.rotation);
            cding = true;
        }
    }

    private void Update()
    {
        CDJudge();
    }
}
