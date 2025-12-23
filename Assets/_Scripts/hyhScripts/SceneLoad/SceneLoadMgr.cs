using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadMgr : MonoBehaviour
{
    private static SceneLoadMgr instance;
    public static SceneLoadMgr Instance => instance;
    public SceneData SceneData = new();
    void Awake()
    {
        SceneData.Scene1IsOpen = true;
        SceneData.Scene2IsOpen = true;
        SceneData.Scene3IsOpen = true;
        SceneData.Scene4IsOpen = true;
        
        SceneData.Scene1Back = true;
        SceneData.Scene2Back = true;
        SceneData.Scene3Back = true;
        SceneData.Scene4Back = true;
        instance = this;
    }

    public void MyLoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
