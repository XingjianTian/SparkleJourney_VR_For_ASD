using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CloneArea : MonoBehaviour
{
    public Button btnConfirm;

    // Start is called before the first frame update
    void Start()
    {
        btnConfirm.onClick.AddListener((() =>
        {
            Collider[] raycastColliders = Physics.OverlapSphere(transform.position,0.11f,1<<LayerMask.NameToLayer("ClassRoomObj"));
            for (int i = 0; i < raycastColliders.Length; i++)
            {
                Instantiate(raycastColliders[i].gameObject, raycastColliders[i].gameObject.transform.position + Vector3.up * 0.4f,
                    transform.rotation);
            }
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
