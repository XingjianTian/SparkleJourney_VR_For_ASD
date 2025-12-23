using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeColorArea : MonoSingleton<ChangeColorArea>
{
    public Button btnConfirm;
    private List<Color> colorList = new List<Color>();
    private int index;



    // Start is called before the first frame update
    void Start()
    {
        colorList.Add(Color.black);
        colorList.Add(Color.blue);
        colorList.Add(Color.cyan);
        colorList.Add(Color.magenta);
        colorList.Add(Color.green);
        colorList.Add(Color.red);
        colorList.Add(Color.white);
        colorList.Add(Color.yellow);
        colorList.Add(Color.gray);
        btnConfirm.onClick.AddListener((() =>
        {
            Collider[] raycastColliders = Physics.OverlapSphere(transform.position,0.11f,1<<LayerMask.NameToLayer("ClassRoomObj"));
            for (int i = 0; i < raycastColliders.Length; i++)
            {
                raycastColliders[i].GetComponent<MeshRenderer>().material.color = colorList[index];
                index++;
                index %= colorList.Count;
            }
        }));


    }

    // Update is called once per frame
    void Update()
    {
    }
}
