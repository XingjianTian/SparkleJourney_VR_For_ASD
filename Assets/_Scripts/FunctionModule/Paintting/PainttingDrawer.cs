using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum DrawerMode
{
    Mouse,
    Collision
}

public class PainttingDrawer : MonoBehaviour
{
    public bool enable = false;
    public DrawerMode mode = DrawerMode.Mouse;
    public Color currentColor = Color.red;

    public float brushSize = 1;

    [Header("MouseMode")]
    public Camera eventCamera;

    [Header("CollisionMode")]
    public Transform rayOriginPoint;

    public float raycastMaxDistance = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!enable)
        {
            return;
        }
        if (mode == DrawerMode.Mouse)
        {
            HandleMosueDrawer();
        } else if (mode == DrawerMode.Collision)
        {
            HandleCollisionDrawer();
        }
    }

    private PainttingObj cacheObj;
    private void HandleMosueDrawer()
    {

        if (Physics.Raycast(eventCamera.ScreenPointToRay(Mouse.current.position.ReadValue()),out RaycastHit hit,Mathf.Infinity,1 << LayerMask.NameToLayer("DrawAble")))
        {
                PainttingObj obj = hit.collider.GetComponent<PainttingObj>();
                cacheObj = obj;
                if (obj != null)
                {
                    obj.TouchPointHandler(hit.point,currentColor,brushSize);
                } else
                {
                    Debug.LogWarning($"当前对象{obj.gameObject.name}被标记为了DrawAble，但是没有挂载PainttingObj脚本");
                }
        } else
        {
            if (cacheObj != null)
            {
                cacheObj.StopTouch();
                cacheObj = null;
            }
        }
    }

    private void HandleCollisionDrawer()
    {
        if (rayOriginPoint != null)
        {
            Ray ray = new Ray(rayOriginPoint.position, rayOriginPoint.right);
            Debug.DrawLine(rayOriginPoint.position,rayOriginPoint.transform.right*raycastMaxDistance + rayOriginPoint.position,Color.red);
            if (Physics.Raycast(ray,out RaycastHit hit,raycastMaxDistance,1 << LayerMask.NameToLayer("DrawAble")))
            {
                print(1);
                PainttingObj obj = hit.collider.GetComponent<PainttingObj>();
                cacheObj = obj;
                if (obj != null)
                {
                    float dynimicBrushSize = brushSize - (Mathf.Max(0,Vector3.Distance(rayOriginPoint.position, hit.point)) / raycastMaxDistance);
                    // obj.TouchPointHandler(hit.point,currentColor,dynimicBrushSize);
                    obj.TouchPointHandler(hit.point,currentColor,brushSize);
                } else
                {
                    Debug.LogWarning($"当前对象{obj.gameObject.name}被标记为了DrawAble，但是没有挂载PainttingObj脚本");
                }
            }
            else
            {
                if (cacheObj != null)
                {
                    cacheObj.StopTouch();
                    cacheObj = null;
                }
            }
        }
        else
        {
            Debug.LogWarning($"当前drawer{gameObject.name},没有设置rayOrigin");
        }
    }



    // private void OnCollisionStay(Collision other)
    // {
    //     if (enable)
    //     {
    //         if (other.collider.CompareTag("DrawAble"))
    //         {
    //             PainttingObj obj = other.collider.GetComponent<PainttingObj>();
    //             if (obj != null && cacheObj != null &&cacheObj != obj)
    //             {
    //                 cacheObj.StopTouch();
    //             }
    //
    //             if (obj != null)
    //             {
    //                 cacheObj = obj;
    //                 print(other.contacts[0].point);
    //                 obj.TouchPointHandler(other.contacts[0].point,currentColor,brushSize);
    //             } else
    //             {
    //                 Debug.LogWarning($"当前对象{obj.gameObject.name}被标记为了DrawAble，但是没有挂载PainttingObj脚本");
    //             }
    //         }
    //     }
    // }
}
