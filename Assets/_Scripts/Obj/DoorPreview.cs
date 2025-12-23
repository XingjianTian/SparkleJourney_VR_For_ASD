using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorPreview : MonoBehaviour
{
    public Transform CameraTrans;
    private Material previewMat;
    // Start is called before the first frame update
    void Start()
    {
        previewMat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraTrans != null)
        {
            Vector3 viewDir = (CameraTrans.position - transform.position).normalized;
            Vector3 xViewDir = viewDir;
            xViewDir.y = 0;
            previewMat.SetFloat("_XOffset",-Vector3.Dot(xViewDir,transform.right) * 0.3f);

            Vector3 yViewdir = viewDir;
            yViewdir.x = 0;
            previewMat.SetFloat("_YOffset",-Vector3.Dot(yViewdir,transform.up) * 0.3f);

        }
    }
}
