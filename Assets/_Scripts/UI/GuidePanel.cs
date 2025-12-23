using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GuidePanel : MonoSingleton<GuidePanel>
{

    private List<GameObject> guideImages = new List<GameObject>();

    private int index = 0;

    [Header("GameObject")] public Transform grabObj1;
    public Transform grabObj2;

    public Button btnTrigger;

    private XRGrabInteractable g1;
    private XRGrabInteractable g2;

    [Header("Interactions")]
    public List<string> speechList = new List<string>();


    public InputActionReference LeftStick;

    public InputActionReference LeftGrab;

    public InputActionReference LeftTrigger;


    public InputActionReference RightStickTeleport;
    public InputActionReference RightStickTurn;

    public InputActionReference RightGrab;
    public InputActionReference RightTrigger;

    public List<AudioClip> CheerAudioClipList = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {

        Transform panelTranform = transform.Find("GuidePanel");
        for (int i = 0; i < panelTranform.childCount; i++)
        {
            guideImages.Add(panelTranform.GetChild(i).gameObject);
        }
        RobotObj.Instance.SpeechInResorucesAudio("Music/EnvMusic/Login scenario/手柄介绍开头",(() =>
        {
            RobotObj.Instance.SpeechInResorucesAudio("Music/EnvMusic/Login scenario/基础操作音频",(() =>
            {
                RobotObj.Instance.SpeechText(speechList[index],(() =>
                {
                    LeftStick.action.performed += HandleLeftStick;
                }));
            }));
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int currentImage;
    public void MoveNextImage()
    {
        guideImages[currentImage].SetActive(false);
        currentImage++;
        guideImages[currentImage].SetActive(true);
    }

    //左摇杆
    private void HandleLeftStick(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().magnitude > 0.1f)
        {
            LeftStick.action.performed -= HandleLeftStick;
            // LeftStick.action.Disable();
            index++;

            RobotObj.Instance.SpeechByAudioClip(CheerAudioClipList[UnityEngine.Random.Range(0,CheerAudioClipList.Count)],(
                () =>
                {
                    PXR_Input.SendHapticImpulse(PXR_Input.VibrateType.LeftPICO4U, 0.5f, 1000, 300);
                    MoveNextImage();
                    grabObj1.gameObject.SetActive(true);
                    grabObj2.gameObject.SetActive(true);
                    //进行下一个按键
                    RobotObj.Instance.SpeechText(speechList[index],(() =>
                    {
                        // grabObj1.DOScale(Vector3.one, 0.8f);
                        // grabObj2.DOScale(Vector3.one, 0.8f);

                        g1 = grabObj1.GetComponentInChildren<XRGrabInteractable>();
                        g2 = grabObj2.GetComponentInChildren<XRGrabInteractable>();

                        g1.selectEntered.AddListener(HandleLeftGrab);
                        g2.selectEntered.AddListener(HandleLeftGrab);
                        // LeftGrab.action.performed += HandleLeftGrab;

                    }));
                }));
        }
    }

    //左抓取
    private void HandleLeftGrab(SelectEnterEventArgs @event)
    {
        g1.selectEntered.RemoveListener(HandleLeftGrab);
        g2.selectEntered.RemoveListener(HandleLeftGrab);
        // LeftGrab.action.performed -= HandleLeftGrab;
        // LeftGrab.action.Disable();
        index++;
        RobotObj.Instance.SpeechText("哇！你抓住它啦！你看，它好像在说‘谢谢你带我一起玩’呢！",(
            () =>
            {
                MoveNextImage();
                btnTrigger.gameObject.SetActive(true);

                //进行下一个按键
                RobotObj.Instance.SpeechText(speechList[index],(() =>
                {
                    // LeftTrigger.action.performed += HandleLeftTrigger;
                    btnTrigger.onClick.AddListener(HandleLeftTrigger);
                }));
            }));

    }

    //左扳机
    private void HandleLeftTrigger()
    {
        btnTrigger.onClick.RemoveListener(HandleLeftTrigger);
        btnTrigger.gameObject.SetActive(false);
        // LeftTrigger.action.performed -= HandleLeftTrigger;
        // LeftTrigger.action.Disable();
        index++;
        RobotObj.Instance.SpeechByAudioClip(CheerAudioClipList[UnityEngine.Random.Range(0,CheerAudioClipList.Count)],(
            () =>
            {
                MoveNextImage();
                //进行下一个按键
                RobotObj.Instance.SpeechText(speechList[index],(() =>
                {
                    RightStickTeleport.action.performed += HandleRightTeleport;
                }));
            }));

    }

    private void HandleRightTeleport(InputAction.CallbackContext context)
    {
        RightStickTeleport.action.performed -= HandleRightTeleport;
        index++;
        RobotObj.Instance.SpeechByAudioClip(CheerAudioClipList[UnityEngine.Random.Range(0,CheerAudioClipList.Count)],(
            () =>
            {
                //进行下一个按键
                RobotObj.Instance.SpeechText(speechList[index],(() =>
                {
                    RightStickTurn.action.performed += HandleRightStickTurn;
                }));
            }));
    }

    private void HandleRightStickTurn(InputAction.CallbackContext context)
    {
        RightStickTurn.action.performed -= HandleRightStickTurn;
        index++;
        RobotObj.Instance.SpeechByAudioClip(CheerAudioClipList[UnityEngine.Random.Range(0,CheerAudioClipList.Count)],(
            () =>
            {
                MoveNextImage();
                //进行下一个按键
                RobotObj.Instance.SpeechText(speechList[index],(() =>
                {
                    // RightGrab.action.performed += HandleRightGrab;
                    g1.selectEntered.AddListener(HandleRightGrab);
                    g2.selectEntered.AddListener(HandleRightGrab);
                }));
            }));
    }

    private void HandleRightGrab(SelectEnterEventArgs @eventArgs)
    {
        g1.selectEntered.RemoveListener(HandleRightGrab);
        g2.selectEntered.RemoveListener(HandleRightGrab);
        // RightGrab.action.performed -= HandleRightGrab;
        index++;
        RobotObj.Instance.SpeechByAudioClip(CheerAudioClipList[UnityEngine.Random.Range(0,CheerAudioClipList.Count)],(
            () =>
            {
                MoveNextImage();
                btnTrigger.gameObject.SetActive(true);
                //进行下一个按键
                RobotObj.Instance.SpeechText(speechList[index],(() =>
                {
                    // RightTrigger.action.performed += HandleRgihtTrigger;
                    btnTrigger.onClick.AddListener(HandleRgihtTrigger);
                }));
            }));
    }

    private void HandleRgihtTrigger()
    {
        // RightTrigger.action.performed -= HandleRgihtTrigger;
        btnTrigger.onClick.RemoveListener(HandleRgihtTrigger);
        index++;
        gameObject.SetActive(false);
        RobotObj.Instance.SpeechByAudioClip(CheerAudioClipList[UnityEngine.Random.Range(0,CheerAudioClipList.Count)],(
            () =>
            {
                //进入场景
                LoginSceneMgr.Instance.IntroduceDeviceDone();
            }));
    }

}