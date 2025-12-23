using UnityEngine;
using UnityEngine.SceneManagement;

public enum DoorScene
{
    Begin,
    Scene1,
    Scene1_1,
    Scene1_2,
    Scene2,
    Scene3,
    Scene4
}
public class SceneLoadObj : MonoBehaviour
{
    public DoorScene scene;
    private string sceneName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NavUtility.Instance.HideAllNavPath();
            switch (scene)
            {
                case DoorScene.Scene1:
                    SceneManager.LoadSceneAsync("1_HomeScene");
                    BeginSceneMgr.currentMode = "Second";
                    break;
                case DoorScene.Scene1_1:
                    SceneManager.LoadSceneAsync("1_1_MusicScene");
                    break;
                case DoorScene.Scene1_2:
                    SceneManager.LoadSceneAsync("1_2_DrawScene");
                    break;
                case DoorScene.Scene2:
                    SceneManager.LoadSceneAsync("2_ClassroomScene");
                    BeginSceneMgr.currentMode = "Third";
                    break;
                case DoorScene.Scene3:
                    SceneManager.LoadSceneAsync("3_Scene");
                    BeginSceneMgr.currentMode = "Forth";
                    break;
                case DoorScene.Scene4:
                    SceneManager.LoadSceneAsync("4_OnlineScene");
                    break;
                case DoorScene.Begin:
                    SceneManager.LoadSceneAsync("0_BeginScene");
                    break;
            }
        }
    }


}
