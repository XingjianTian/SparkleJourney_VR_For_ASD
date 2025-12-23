using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

class Timer: CachePoolOBJ<Timer>
{
    public string uid;
    //计时器记录的当前过去的时间
    public float totalTime;
    public float targetTime;
    public float loopJobGapTime = -1;
    //计时器记录的当前过去的时间
    public float loopTotalTime;
    public UnityAction loopJob;

    public UnityAction doneJob;

    private TimerCore core;
    //当前计时器是否被初始化
    public bool isInited;

    public void Init(TimerCore core,float targetTime,UnityAction doneJob,float loopJobGapTime = -1,UnityAction loopJob = null)
    {
        this.uid = Guid.NewGuid().ToString();
        this.totalTime = 0;
        this.loopTotalTime = 0;
        this.core = core;
        this.targetTime = targetTime;
        this.doneJob = doneJob;
        this.loopJobGapTime = loopJobGapTime;
        this.loopJob = loopJob;

        isInited = true;
    }


    public override void Release()
    {
        base.Release();
        isInited = false;
    }
}


class TimerCore
{
    public Dictionary<string, Timer> timers = new Dictionary<string, Timer>();


    //开启计时器
    public void StartComputedTimer()
    {
        Thread thread = new Thread(StartWork);

        thread.Start();
        MonoMgr.Instance.AddUpdateEvent((() =>
        {
            if (workPipline.Count > 0)
            {
                workPipline.Dequeue()?.Invoke();
            }
        }));
        MonoMgr.Instance.AddApplicationQuitEvent((() =>
        {
            Debug.Log("TimerMgr线程被关闭了");
            thread.Abort();
        }));
    }


    public void StartTimer(Timer t)
    {
        if (t.isInited)
        {
            timers.Add(t.uid,t);
        }
        else
        {
            Debug.LogWarning($"当前计时器【{t.uid}】没有被初始化，是个损坏的计时器");
        }
    }

    public bool RemoveTimer(string uid)
    {
        if (timers.ContainsKey(uid))
        {
            timers.Remove(uid);
            return true;
        }

        return false;
    }

    private Queue<UnityAction> workPipline = new Queue<UnityAction>();

    void StartWork()
    {
        DateTime PreTime = DateTime.Now;
        while (true)
        {
            List<string> keys = timers.Keys.ToList();
            DateTime curTime = DateTime.Now;
            float passedTime = (curTime - PreTime).Milliseconds / 1000f;
            // Debug.LogWarning(Time.deltaTime);
            PreTime = curTime;
            Timer t;
            for (int i = 0; i < keys.Count; i++)
            {
                t = timers[keys[i]];
                t.totalTime += passedTime;
                t.loopTotalTime += passedTime;

                if (t.totalTime >= t.targetTime)
                {
                    RemoveTimer(t.uid);
                    workPipline.Enqueue(t.doneJob);
                    workPipline.Enqueue((() => {t.Release();}));
                } else if (t.loopJobGapTime != -1 && t.loopTotalTime >= t.loopJobGapTime)
                {
                    workPipline.Enqueue(t.loopJob);
                }
            }
            Thread.Sleep(100);
        }
    }

}

public class TimerMgr : BaseMgr<TimerMgr>
{
    private TimerCore core;
    private TimerMgr()
    {
        core = new TimerCore();
        core.StartComputedTimer();
    }


    public string StartTimer(float targetTime,UnityAction doneJob,float loopJobGapTime = -1,UnityAction loopJob = null)
    {
        Timer timer = CachePool.Instance.Pop<Timer>();
        timer.Init(core,targetTime,doneJob,loopJobGapTime,loopJob);
        core.StartTimer(timer);
        return timer.uid;
    }

    public bool StopTimer(string uid)
    {
        return core.RemoveTimer(uid);
    }

}