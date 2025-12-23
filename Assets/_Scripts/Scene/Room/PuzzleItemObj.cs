using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzleItemObj : MonoBehaviour
{
    public PuzzleItem Item;

    public bool isInit;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool isCapturedBySocket = false;

    // Start is called before the first frame update
    void Start()
    {
        Item = new PuzzleItem(transform, PuzzleMode.Horizontal);
        if (isInit)
        {
            Item.InitSocketInteractor();
        }

        // 获取组件引用
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // 添加事件监听
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
        }

    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }






    // Update is called once per frame
    void Update()
    {

    }
}
