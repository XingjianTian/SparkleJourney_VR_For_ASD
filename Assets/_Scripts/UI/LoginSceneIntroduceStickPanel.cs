using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LoginSceneIntroduceStickPanel : MonoSingleton<LoginSceneIntroduceStickPanel>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Vector3 playerPos;
    // Update is called once per frame
    void Update()
    {
        playerPos = PlayerObj.Instance.transform.position;
        playerPos.y = transform.position.y;
        transform.DOLookAt(playerPos,2f);
    }
}
