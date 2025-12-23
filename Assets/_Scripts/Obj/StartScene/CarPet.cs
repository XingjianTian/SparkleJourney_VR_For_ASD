using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPet : MonoBehaviour
{
    public List<GameObject> carPetList;

    public float targetDisptance;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject item in carPetList)
        {
            item.transform.position -= item.transform.up * 10f;
        }
    }

    IEnumerator Show()
    {
        foreach (var item in carPetList)
        {

        }

        yield return new WaitForSeconds(0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
