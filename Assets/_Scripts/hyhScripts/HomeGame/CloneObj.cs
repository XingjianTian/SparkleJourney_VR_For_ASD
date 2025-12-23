using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneObj : GameMgr
{
    private void OnTriggerEnter(Collider other)
    {
        if (cding) return;
        if (other.CompareTag("HomeObj"))
        {
            string resourcesText = GameMgr.Instance.ColorToText(other.transform.parent.GetComponent<ColorID>().ObjColorName);
            Instantiate(Resources.Load<GameObject>(resourcesText), other.transform.position, other.transform.rotation);
            cding = true;
        }
    }
    private void Update()
    {
        CDJudge();
    }
}
