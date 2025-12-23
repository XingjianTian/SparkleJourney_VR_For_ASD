using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ColorPicker2 : MonoBehaviour
{
    public InputActionReference inputKey;
    public Transform handTrans;
    public RectTransform colorPicker;
    private EventTrigger m_eventTrigger;

    private EventTrigger eventTrigger
    {
        get
        {
            if (m_eventTrigger == null)
            {
                m_eventTrigger = GetComponent<EventTrigger>();
            }

            return m_eventTrigger;
        }
    }

    private void OnEnable()
    {
        inputKey.action.performed += OnInputKeyDown;
        inputKey.action.canceled += OnInputKeyUp;
    }

    private void OnDisable()
    {
        inputKey.action.performed -= OnInputKeyDown;
        inputKey.action.canceled -= OnInputKeyUp;
    }

    private void OnInputKeyDown(InputAction.CallbackContext context)
    {
        if (Physics.Raycast(handTrans.position, handTrans.forward, out RaycastHit hit, Mathf.Infinity,
                1 << LayerMask.NameToLayer("ColorPicker")))
        {
            // isEnable = true;
            colorPicker.transform.position = hit.point;
        }
    }

    private void OnInputKeyUp(InputAction.CallbackContext context)
    {
        // isEnable = false;
    }

    private bool isEnable;
    // Start is called before the first frame update
    void Start()
    {
        // EventTrigger.Entry dragStartEntry = new EventTrigger.Entry();
        // dragStartEntry.eventID = EventTriggerType.BeginDrag;
        // dragStartEntry.callback.AddListener((v =>
        // {
        //
        // }));
    }

    // Update is called once per frame
    void Update()
    {
        // if (isEnable)
        // {
        //     if (Physics.Raycast(handTrans.position, handTrans.forward, out RaycastHit hit, Mathf.Infinity,
        //             1 << LayerMask.NameToLayer("ColorPicker"))) ;
        //     {
        //         isEnable = true;
        //         // colorPicker.transform.position = hit.point;
        //         Vector2 screenPoint = Camera.main.WorldToScreenPoint(hit.point);
        //         RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, screenPoint,
        //             Camera.main,out Vector2 resPoint);
        //
        //         colorPicker.anchoredPosition = ClampPickerPosition(resPoint);
        //         // ClampPickerPosition();
        //     }
        }
}

    // private Vector2 colorPaleteeSize = Vector2.zero;
    // private Vector2 ClampPickerPosition(Vector2 resPoint)
    // {
    //     if (colorPaleteeSize == Vector2.zero)
    //     {
    //         colorPaleteeSize = (transform.parent as RectTransform).rect.size / 2;
    //         print(colorPaleteeSize);
    //     }
    //
    //     float x = Mathf.Clamp(resPoint.x, -colorPaleteeSize.x, colorPaleteeSize.x);
    //     float y = Mathf.Clamp(resPoint.y, -colorPaleteeSize.y, colorPaleteeSize.y);
    //     return new Vector2(x, y);
    // }
    // }
