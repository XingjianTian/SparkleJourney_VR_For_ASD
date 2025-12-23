using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

public enum PuzzleMode
{
    Horizontal,
    Vertical
}

public enum PushDirection
{
    Left,
    Right,
    Forward,
    Back
}

public class SocketItem
{
    public static Dictionary<Vector3, SocketItem> SocketItemDic = new Dictionary<Vector3, SocketItem>();
    public Vector3 Trans;

    public XRSocketInteractor Interactor;
    public SocketPerDir controller;
    public SocketItem(SocketPerDir controller, Transform Trans, XRSocketInteractor interactor)
    {
        if (SocketItemDic.ContainsKey(Trans.position))
        {
            SocketItemDic[Trans.position].Interactor.selectEntered.AddListener(data =>
            {
                OnSelectEnter(data, Trans);
            });
        }
        else
        {
            SocketItemDic.Add(Trans.position, this);
            interactor.selectEntered.AddListener((data =>
            {
                OnSelectEnter(data, Trans);
            }));
        }

        this.controller = controller;
        this.Trans = Trans.position;
        this.Interactor = interactor;
    }

    private void OnSelectEnter(SelectEnterEventArgs selectEnterEventArgs, Transform Trans)
    {

        PuzzleItem targetPuzzleItem = selectEnterEventArgs.interactableObject
            .transform.GetComponent<PuzzleItemObj>().Item;
        TimerMgr.Instance.StartTimer(0.5f, (() =>
        {
            selectEnterEventArgs.interactableObject
                .transform.GetComponent<XRGrabInteractable>().enabled = false;
        }));
        selectEnterEventArgs.interactableObject
            .transform.GetComponent<Rigidbody>().isKinematic = true;
        switch (controller.controllerDir)
        {
            case PushDirection.Left:
                this.controller.controller.left = targetPuzzleItem;
                targetPuzzleItem.right = this.controller.controller;
                break;
            case PushDirection.Right:
                this.controller.controller.right = targetPuzzleItem;
                targetPuzzleItem.left = this.controller.controller;
                break;
            case PushDirection.Forward:
                this.controller.controller.forward = targetPuzzleItem;
                targetPuzzleItem.back = this.controller.controller;
                break;
            case PushDirection.Back:
                this.controller.controller.back = targetPuzzleItem;
                targetPuzzleItem.forward = this.controller.controller;
                break;
        }
        this.controller.Destroy();
        if (SocketItemDic.ContainsKey(Trans.position))
        {
            SocketItemDic.Remove(Trans.position);
        }

        if (selectEnterEventArgs.interactableObject
            .transform.rotation != Quaternion.identity)
        {
            targetPuzzleItem.mode = PuzzleMode.Vertical;
        }
        else
        {
            targetPuzzleItem.mode = PuzzleMode.Horizontal;
        }

        TimerMgr.Instance.StartTimer(1, (() =>
        {
            targetPuzzleItem.InitSocketInteractor();
        }));
    }
}

public class SocketPerDir
{
    public PuzzleItem controller;
    public PushDirection controllerDir;
    public SocketItem socket1;
    public SocketItem socket2;
    public SocketItem socket3;

    public static GameObject puzzleSocketPrefab;
    public SocketPerDir(PuzzleItem controller, PushDirection controllerDir, Transform originTransform, PuzzleMode mode, int counter)
    {
        if (puzzleSocketPrefab == null)
        {
            puzzleSocketPrefab = Resources.Load<GameObject>("Prefabs/hyh/HomeGame/PuzzleScoketInteractor");
        }
        this.controller = controller;
        this.controllerDir = controllerDir;
        BoxCollider boxCollider = originTransform.gameObject.GetComponent<BoxCollider>();
        Vector3 size = boxCollider.bounds.size;
        float x = size.x;
        float y = size.y;
        float z = size.z;
        Debug.Log($"X:{x}   Y:{y}    Z:{z}");
        //初始化交互器
        if (mode == PuzzleMode.Horizontal)
        {
            if (this.controllerDir == PushDirection.Left && this.controller.left == null)
            {
                GameObject go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Left_1";
                go.transform.position = originTransform.position - originTransform.right * x;
                XRSocketInteractor interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Left_2";
                go.transform.position = originTransform.position -
                                        originTransform.right * x / 2f + originTransform.up * x / 2f;
                go.transform.rotation *= Quaternion.AngleAxis(90, originTransform.forward);
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Left_3";
                go.transform.position = originTransform.position -
                                        originTransform.right * x / 2f - originTransform.up * x / 2f;
                go.transform.rotation *= Quaternion.AngleAxis(90, originTransform.forward);
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
            }

            if (this.controllerDir == PushDirection.Right && this.controller.right == null)
            {
                GameObject go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Right_1";
                go.transform.position = originTransform.position + originTransform.right * x;
                XRSocketInteractor interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Right_2";
                go.transform.position = originTransform.position +
                                        originTransform.right * x / 2f +
                                         originTransform.up * x / 2f;
                go.transform.rotation *= Quaternion.AngleAxis(90, originTransform.forward);
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Right_3";
                go.transform.position = originTransform.position +
                                        originTransform.right * x / 2f - originTransform.up * x / 2f;
                go.transform.rotation *= Quaternion.AngleAxis(90, originTransform.forward);
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
            }

            if (this.controllerDir == PushDirection.Forward && this.controller.forward == null)
            {
                GameObject go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Forward_1";
                go.transform.position = originTransform.position + originTransform.forward * x;
                go.transform.rotation = originTransform.rotation;
                XRSocketInteractor interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);

                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Forward_2";
                go.transform.position = originTransform.position + originTransform.forward * x / 2f + originTransform.up *
                    (x / 2f);
                go.transform.rotation *= Quaternion.AngleAxis(90, originTransform.right);
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);

                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Forward_3";
                go.transform.position = originTransform.position + originTransform.forward * x / 2f - originTransform.up *
                    (x / 2f );
                go.transform.rotation *= Quaternion.AngleAxis(90, originTransform.right);
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
            }
            if (this.controllerDir == PushDirection.Back && this.controller.back == null)
            {
                GameObject go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Back_1";
                go.transform.position = originTransform.position - originTransform.forward * x;
                go.transform.rotation = originTransform.rotation;
                XRSocketInteractor interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);

                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Back_2";
                go.transform.position = originTransform.position - originTransform.forward * x / 2f - originTransform.up *
                    (x / 2f );
                go.transform.rotation *= Quaternion.AngleAxis(90, originTransform.right);
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);

                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Back_3";
                go.transform.position = originTransform.position - originTransform.forward * x / 2f + originTransform.up *
                    (x / 2f);
                go.transform.rotation *= Quaternion.AngleAxis(90, originTransform.right);
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
            }
        }
        else    
        {
            if (this.controllerDir == PushDirection.Left && this.controller.left == null)
            {
                GameObject go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Top_1";
                go.transform.position = originTransform.position + Vector3.up * y;
                go.transform.localRotation = originTransform.rotation;
                XRSocketInteractor interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Top_2";
                go.transform.position = originTransform.position +
                                        Vector3.up * y / 2f +
                                        originTransform.up * y / 2f;
                go.transform.localRotation = Quaternion.identity;
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Top_3";
                go.transform.position = originTransform.position +
                                        Vector3.up * y / 2f -
                                        originTransform.up * y / 2f;
                go.transform.localRotation = Quaternion.identity;
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
            }
            if (this.controllerDir == PushDirection.Right && this.controller.right == null)
            {
                GameObject go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Bottom_1";
                go.transform.position = originTransform.position - Vector3.up * y;
                go.transform.localRotation = originTransform.rotation;
                XRSocketInteractor interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Bottom_2";
                go.transform.position = originTransform.position -
                                        Vector3.up * y / 2f +
                                        originTransform.up * y / 2f;
                go.transform.localRotation = Quaternion.identity;
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Bottom_3";
                go.transform.position = originTransform.position -
                                        Vector3.up * y / 2f -
                                        originTransform.up * y / 2f;
                go.transform.localRotation = Quaternion.identity;
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
            }
            if (this.controllerDir == PushDirection.Forward && this.controller.forward == null)
            {
                GameObject go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Front_1";
                go.transform.position = originTransform.position + originTransform.forward * y;
                go.transform.localRotation = originTransform.rotation;
                XRSocketInteractor interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);

                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Front_2";
                go.transform.position = originTransform.position + originTransform.forward * y / 2f + originTransform.up * y / 2;
                go.transform.rotation = Quaternion.AngleAxis(90, Vector3.up) * originTransform.rotation;
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);

                go = GameObject.Instantiate(puzzleSocketPrefab);
                go.name = $"Socket_{counter}_Front_3";
                go.transform.position = originTransform.position + originTransform.forward * y / 2f - originTransform.up * y / 2;
                go.transform.rotation = Quaternion.AngleAxis(90, Vector3.up) * originTransform.rotation;
                interactor = go.GetComponent<XRSocketInteractor>();
                new SocketItem(this,
                    go.transform, interactor);
            }

        }
    }


    public void Destroy()
    {
        // GameObject.Destroy(socket1.Interactor.gameObject);
        // GameObject.Destroy(socket2.Interactor.gameObject);
        // GameObject.Destroy(socket3.Interactor.gameObject);
    }
}

public class PuzzleItem
{
    private static int socketCounter = 0;  // 添加静态计数器
    private GameObject currentGO;
    public PuzzleMode mode;

    private PuzzleItem m_left;
    private PuzzleItem m_right;
    private PuzzleItem m_forward;
    private PuzzleItem m_back;

    public PuzzleItem GetPuzzleItem(PushDirection direction)
    {
        switch (direction)
        {
            case PushDirection.Left:
                return left;
            case PushDirection.Right:
                return right;
            case PushDirection.Forward:
                return forward;
            case PushDirection.Back:
                return back;
        }

        return null;
    }
    public PuzzleItem left
    {
        get
        {
            return m_left;
        }
        set
        {
            m_left = value;

        }
    }

    public PuzzleItem right
    {
        get
        {
            return m_right;
        }
        set
        {
            m_right = value;

        }
    }

    public PuzzleItem forward
    {
        get
        {
            return m_forward;
        }
        set
        {
            m_forward = value;
        }
    }

    public PuzzleItem back
    {
        get
        {
            return m_back;
        }
        set
        {
            m_back = value;
        }
    }


    private Transform originTransform;
    public PuzzleItem(Transform originTransform, PuzzleMode mode)
    {
        this.originTransform = originTransform;
        this.mode = mode;
    }

    //初始化socket交互器
    public void InitSocketInteractor()
    {
        socketCounter++;  // 每次调用时增加计数器
        new SocketPerDir(this, PushDirection.Left, originTransform, mode, socketCounter);
        new SocketPerDir(this, PushDirection.Right, originTransform, mode, socketCounter);
        new SocketPerDir(this, PushDirection.Forward, originTransform, mode, socketCounter);
        new SocketPerDir(this, PushDirection.Back, originTransform, mode, socketCounter);
    }
}
