using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FreeSpaceMgr : MonoBehaviour
{
    public GameObject plane;

    public InputActionReference btnOpenVideoStreet;

    public static bool isOpenVideo;



    void Awake()
    {
        // PXR_Manager.EnableVideoSeeThrough = true;
        // PXR_MixedReality.EnableVideoSeeThroughEffect(true);
    }

    private void OnEnable()
    {
        btnOpenVideoStreet.action.performed += ToggleVideoStreet;
    }

    private void OnDisable()
    {
        btnOpenVideoStreet.action.performed -= ToggleVideoStreet;
    }

    private void ToggleVideoStreet(InputAction.CallbackContext context)
    {
        if (isOpenVideo)
        {
            isOpenVideo = false;
            Camera.main.clearFlags = CameraClearFlags.Skybox;
            plane.SetActive(true);
            PXR_Manager.EnableVideoSeeThrough = true;
            PXR_MixedReality.EnableVideoSeeThroughEffect(false);
        }
        else
        {
            isOpenVideo = true;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = new Color(0, 0, 0, 0);
            plane.SetActive(false);
            PXR_Manager.EnableVideoSeeThrough = false;
            PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
