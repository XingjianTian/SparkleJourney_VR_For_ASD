using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorObj : MonoBehaviour
{
    private List<Material> doorRockMats;

    private TeleportColliderObj tele;
    // Start is called before the first frame update
    void Awake()
    {
        // doorRockMats = GetComponentsInChildren<MeshRenderer>().Select(x  => x.material).ToList();
        // print(doorRockMats.Count);
        // tele = GetComponentInChildren<TeleportColliderObj>();
        // LoginSceneMgr.RegistryTeleport(tele.targetId, this);
        // gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // public void SetDissolveNum(float v)
    // {
    //     v = Mathf.Clamp(v, 0, 1);
    //     for (int i = 0; i < doorRockMats.Count; i++)
    //     {
    //         doorRockMats[i].SetFloat("_DissolveThreshold", v);
    //     }
    // }
}
