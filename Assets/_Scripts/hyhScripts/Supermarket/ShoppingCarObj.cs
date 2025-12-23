using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShoppingCarObj : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Product"))
        {
            ShoppingPanel.Instance.AddPanelData(other.gameObject);
            // TimerMgr.Instance.StartTimer(0.5f, (() =>
            // {
            //     other.transform.parent.parent = gameObject.transform;
            //     other.GetComponent<Rigidbody>().isKinematic = true;
            //     other.transform.parent.parent.GetComponent<XRGrabInteractable>().selectEntered.AddListener(arg0 =>
            //     {
            //         other.transform.parent.parent = null;
            //         other.GetComponent<Rigidbody>().isKinematic = false;
            //     });
            // }));
            // other.transform.parent.GetComponent<XRGrabInteractable>().selectExited.AddListener(HandleGoodEnterShopingCar);
            Destroy(other.transform.parent.gameObject);
        }
    }

    private void HandleGoodEnterShopingCar(SelectExitEventArgs args)
    {
        Destroy(args.interactableObject.transform.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Product"))
        {
            ShoppingPanel.Instance.RemovePanelData(other.gameObject);

        }
    }
}
