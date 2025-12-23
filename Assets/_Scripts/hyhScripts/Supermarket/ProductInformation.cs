using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ProductInformation : MonoBehaviour
{
    public string Name;
    public int Price;
    
    private XRGrabInteractable interactable;
    private Rigidbody rigidbody;
    private Transform currentObj;
    private void Start()
    {
        currentObj = transform;
        FindComponents();
    }

    private void FindComponents()
    {
        if (currentObj == null)
        {
            return;
        }
        interactable = currentObj.GetComponent<XRGrabInteractable>();
        rigidbody = currentObj.GetComponent<Rigidbody>();
        if (interactable == null || rigidbody == null)
        {
            currentObj = transform.parent;
            FindComponents();
            return;
        }

        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        interactable.selectEntered.AddListener((obj) =>
        {
            rigidbody.constraints = RigidbodyConstraints.None;
        });
    }
}
