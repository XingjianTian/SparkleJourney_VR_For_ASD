using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeSkyBox : MonoBehaviour
{
    public InputActionReference changeSkyBoxButton;

    public Material[] skyboxArr;

    private int curSkyBoxIndex = 0;
    private void OnEnable()
    {
        changeSkyBoxButton.action.performed += OnChangeSkyBoxButtonDown;
    }

    private void OnDisable()
    {
        changeSkyBoxButton.action.performed -= OnChangeSkyBoxButtonDown;
    }

    private void OnChangeSkyBoxButtonDown(InputAction.CallbackContext context)
    {
        if (FreeSpaceMgr.isOpenVideo)
            return;
        curSkyBoxIndex++;
        curSkyBoxIndex %= skyboxArr.Length;
        RenderSettings.skybox = skyboxArr[curSkyBoxIndex];
    }

    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = skyboxArr[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
