using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMusicMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MusicMgr.Instance.effCore.PlaySound("Music/Eff/Test2");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            MusicMgr.Instance.effCore.Clear();
        }
    }
}
