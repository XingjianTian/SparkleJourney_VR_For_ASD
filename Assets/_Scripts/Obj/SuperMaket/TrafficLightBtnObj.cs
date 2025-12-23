using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TrafficLightBtnObj : MonoBehaviour
{
    public TextMeshProUGUI txtLightCountDownTxt;
    public float totalWaitTime = 10;

    [Header("LightColor")] public Material matGreen;
    public Material matGray;
    public Material matRed;

    public List<MeshRenderer> beGreenList = new List<MeshRenderer>();
    public List<MeshRenderer> beRedList = new List<MeshRenderer>();
    public List<MeshRenderer> beGrayList = new List<MeshRenderer>();
    private XRSimpleInteractable _simpleInteractable;

    private bool isStarted;
    // Start is called before the first frame update
    void Start()
    {
        _simpleInteractable = GetComponent<XRSimpleInteractable>();
        _simpleInteractable.hoverEntered.AddListener((v =>
        {
            if (isStarted)
            {
                return;
            }

            isStarted = true;
            //红绿等开始倒计时
            StartCoroutine(StartTimer());
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartTimer()
    {
        txtLightCountDownTxt.text = totalWaitTime.ToString();
        WaitForSeconds wait = new WaitForSeconds(1);
        while (totalWaitTime > 0)
        {
            yield return wait;
            totalWaitTime -= 1;
            txtLightCountDownTxt.text = totalWaitTime.ToString();
        }
        //切换为绿灯
        foreach (var item in beGreenList)
        {
            item.material = matGreen;
        }

        foreach (var item in beRedList)
        {
            item.material = matRed;
        }

        foreach (var item in beGrayList)
        {
            item.material = matGray;
        }

        txtLightCountDownTxt.text = "";
    }

}
