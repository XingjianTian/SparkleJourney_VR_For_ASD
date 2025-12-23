using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Game3Socket : MonoBehaviour
{
    public GameObject upSocket;
    //是否允许显示上方 socket
    public bool isAllow;
    void Start()
    {
        XRSocketInteractor socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener((obj) =>
        {
            if (upSocket != null && isAllow) upSocket.gameObject.SetActive(true);
        });
        socket.selectExited.AddListener((obj) =>
        {
            if (upSocket != null && isAllow) upSocket.gameObject.SetActive(false);
        });
    }

    public void UpdateVis()
    {
        if (upSocket == null) return;

        if (isAllow)
        {
            upSocket.gameObject.SetActive(false);
        }
    }
}
