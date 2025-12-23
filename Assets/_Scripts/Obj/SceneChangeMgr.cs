using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneChangeMgr : MonoBehaviour
{
    public InputActionReference inputActionReference;
    // Start is called before the first frame update
    void Start()
    {
        inputActionReference.action.performed += (context =>
        {
            SceneManager.LoadSceneAsync("0_BeginScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
