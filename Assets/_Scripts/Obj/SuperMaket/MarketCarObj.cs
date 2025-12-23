using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteralGood
{
    public string name;
    public float price;
    public int num = 1;

    public InteralGood(string name,float price)
    {
        this.name = name;
        this.price = price;
    }

    public void AddNum()
    {
        this.num += 1;
    }

    public void ReduceNum()
    {
        this.num -= 1;
    }
}

public class MarketCarObj : MonoSingleton<MarketCarObj>
{
    public Dictionary<string, InteralGood> goodDic = new Dictionary<string, InteralGood>();


    private void Start()
    {
        StartCoroutine(UpdateUIPanel());
    }




    IEnumerator UpdateUIPanel()
    {
        WaitForSeconds wait = new WaitForSeconds(0.15f);
        while (true)
        {
            foreach (var good in goodDic.Values)
            {
                GoodPanel.Instance.UpdateGoodItem(good);
            }

            yield return wait;
        }
    }


    public void AddGood(string name,float price)
    {
        if (!goodDic.ContainsKey(name))
        {
            goodDic.Add(name,new InteralGood(name,price));
        }
        else
        {
            goodDic[name].AddNum();
        }
    }

    public void AddGood(GoodObj goodObj)
    {
        if (!goodDic.ContainsKey(goodObj.name))
        {
            goodDic.Add(goodObj.name,new InteralGood(goodObj.name,goodObj.price));
        }
        else
        {
            goodDic[goodObj.name].AddNum();
        }
    }

    public void RemoveGood(string name)
    {
        if (goodDic.ContainsKey(name))
        {
            goodDic[name].ReduceNum();
        }
    }
    public void RemoveGood(GoodObj goodObj)
    {
        if (goodDic.ContainsKey(goodObj.name))
        {
            goodDic[goodObj.name].ReduceNum();
        }
    }



    public float ClaculateTotalPrice()
    {
        float res = 0;
        foreach (var goodItem in goodDic.Values)
        {
            res += goodItem.price * goodItem.num;
        }

        return res;
    }

    private void OnTriggerEnter(Collider other)
    {
        GoodObj goodObj = other.GetComponent<GoodObj>();
        if (goodObj != null)
        {
            other.transform.parent = transform.parent;
            AddGood(goodObj);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GoodObj goodObj = other.GetComponent<GoodObj>();
        if (goodObj != null)
        {
            other.transform.parent = null;
            RemoveGood(goodObj);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GameObject"))
        {
            other.transform.parent = transform.parent;
        }
    }
}
