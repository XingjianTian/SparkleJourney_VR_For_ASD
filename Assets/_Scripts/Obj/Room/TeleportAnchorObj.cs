using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(TeleportationAnchor))]
public class TeleportAnchorObj : MonoBehaviour
{
    private TeleportationAnchor m_teleportationAnchor;

    public TeleportationAnchor teleportationAnchor
    {
        get
        {
            if (m_teleportationAnchor == null)
            {
                m_teleportationAnchor = GetComponent<TeleportationAnchor>();
            }

            return m_teleportationAnchor;
        }
    }

//private  Start is called before the first frame update
    void Start()
    {
        teleportationAnchor.teleporting.AddListener((v =>
        {
            SceneManager.LoadSceneAsync("Free space");
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
