using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Djembe : MonoBehaviour
{
    private XRSimpleInteractable _xrSimpleInteractable;

    public List<AudioClip> DjembeEffList = new List<AudioClip>();

    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _xrSimpleInteractable.hoverEntered.AddListener((v =>
        {
            // if (totalTime > 0.5f)
            {
                totalTime = 0;
                MusicMgr.Instance.effCore.PlaySound(DjembeEffList[Random.Range(0, DjembeEffList.Count)]);
            }
        }));
    }

    private float totalTime;
    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
    }
}
