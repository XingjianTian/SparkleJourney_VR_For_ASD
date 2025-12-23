using System;using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicMgr : BaseMgr<MusicMgr>
{
    private MusicMgr()
    {
        BkMusicCore = new BKMusicCore();
        effCore = new EffCore();
    }


    public BKMusicCore BkMusicCore;
    public EffCore effCore;

    public class BKMusicCore
    {
        private float BKMusicVolume = 0.5f;
        private bool muteBkMusic = false;
        private AudioSource bkSource;


        public void PlayBKMusic(string path)
        {
            if (bkSource == null)
            {
                GameObject bkGo = new GameObject("BKMusic");
                GameObject.DontDestroyOnLoad(bkGo);
                bkSource = bkGo.AddComponent<AudioSource>();
            }

            bkSource.clip = Resources.Load<AudioClip>(path);
            bkSource.volume = BKMusicVolume;
            bkSource.mute = muteBkMusic;
            bkSource.Play();
        }

        public void PauseBKMusic()
        {
            if (bkSource != null)
            {
                bkSource.Pause();
            }
        }

        public void SetMuteEffProperty(bool isMute)
        {
            muteBkMusic = isMute;
            if (bkSource != null)
            {
                bkSource.mute = muteBkMusic;
            }
        }

        public void SetBKMusicVolume(float v)
        {
            v = Mathf.Clamp(v, 0, 1);
            BKMusicVolume = v;
            if (bkSource != null)
            {
                bkSource.volume = v;
            }
        }

        public AudioSource GetInteralAudioSource()
        {
            return bkSource;
        }

    }

    public class EffCore
    {
        public EffCore()
        {
            MonoMgr.Instance.AddLateUpdateEvent(UpdateAudioSourceState);
        }

        private void UpdateAudioSourceState()
        {
            AudioSource audioSource;
            for (int i = 0; i < effASList.Count; i++)
            {
                audioSource = effASList[i];
                if (!audioSource.isPlaying && !freeEffAsQueue.Contains(audioSource))
                {
                    freeEffAsQueue.Enqueue(audioSource);
                }
            }
        }

        private float effVolume = 0.5f;
        private bool muteEff = false;

        private List<AudioSource> effASList = new List<AudioSource>();
        private Queue<AudioSource> freeEffAsQueue = new Queue<AudioSource>();

        public void Clear()
        {
            for (int i = 0; i < effASList.Count; i++)
            {
                CachePool.Instance.Push<AudioSource>(effASList[i]);
            }
            effASList.Clear();
            freeEffAsQueue.Clear();
            MonoMgr.Instance.RemoveLateUpdateEvent(UpdateAudioSourceState);
        }

        public void PlaySound(string path)
        {
            AudioSource audioSource;
            if (freeEffAsQueue.Count > 0)
            {
                audioSource = freeEffAsQueue.Dequeue();
                audioSource.clip = Resources.Load<AudioClip>(path);
                audioSource.Play();
            }
            else
            {
                audioSource = CachePool.Instance.Pop<AudioSource>("CachePool/MusicCacheItem");
                audioSource.mute = muteEff;
                audioSource.volume = effVolume;
                audioSource.loop = false;
                if (!effASList.Contains(audioSource))
                    effASList.Add(audioSource);
            }
            audioSource.clip = Resources.Load<AudioClip>(path);
            audioSource.Play();
        }
        public void PlaySound(AudioClip clip)
        {
            AudioSource audioSource;
            if (freeEffAsQueue.Count > 0)
            {
                audioSource = freeEffAsQueue.Dequeue();
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                audioSource = CachePool.Instance.Pop<AudioSource>("CachePool/MusicCacheItem");
                audioSource.mute = muteEff;
                audioSource.volume = effVolume;
                audioSource.loop = false;
                if (!effASList.Contains(audioSource))
                    effASList.Add(audioSource);
            }

            audioSource.clip = clip;
            audioSource.Play();
        }

        public void SetEffVoluem(float v)
        {
            v = Mathf.Clamp(v, 0, 1);
            effVolume = v;
            for (int i = 0; i < effASList.Count; i++)
            {
                effASList[i].volume = effVolume;
            }
        }

        public void SetMuteEffProperty(bool isMute)
        {
            if (muteEff != isMute)
            {
                for (int i = 0; i < effASList.Count; i++)
                {
                    effASList[i].mute = isMute;
                }
            }
            muteEff = isMute;

        }
    }
}


