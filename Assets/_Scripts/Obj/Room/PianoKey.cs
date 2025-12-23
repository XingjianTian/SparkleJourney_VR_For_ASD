using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PianoKey : MonoBehaviour
{
    // public string effPath = "Music/PianoNotes/";

    private XRSimpleInteractable m_interactable;

    public XRSimpleInteractable Interactable
    {
        get
        {
            if (m_interactable == null)
            {
                m_interactable = gameObject.GetComponent<XRSimpleInteractable>();
            }

            return m_interactable;
        }
    }



    private void Start()
    {
        Interactable.hoverEntered.AddListener(v =>
        {
            MusicMgr.Instance.effCore.PlaySound("Music/PianoNotes/" + gameObject.name);
        });

        Interactable.selectExited.AddListener((v =>
        {
        }));
    }
}
