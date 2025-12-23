using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoodPanel : MonoSingleton<GoodPanel>
{
    private Dictionary<string, GameObject> goodItemDic = new Dictionary<string, GameObject>();
    public TextMeshProUGUI txtTotalMoney;

    public void UpdateGoodItem(InteralGood good)
    {
        GameObject goodItem;
        if (!goodItemDic.ContainsKey(good.name))
        {
            goodItem = Instantiate(Resources.Load<GameObject>("Prefabs/GoodItem"),transform);
            goodItemDic.Add(good.name, goodItem);
        }
        else
        {
            goodItem = goodItemDic[good.name];
        }

        if (good.num == 0)
        {
            goodItemDic.Remove(good.name);
            Destroy(goodItem);
            return;
        }
        goodItem.transform.Find("TxtName").GetComponent<TextMeshProUGUI>().text = good.name;
        goodItem.transform.Find("TxtPrice").GetComponent<TextMeshProUGUI>().text = good.price.ToString();
        goodItem.transform.Find("TxtCount").GetComponent<TextMeshProUGUI>().text = good.num.ToString();
        goodItem.transform.Find("TxtTotalMoney").GetComponent<TextMeshProUGUI>().text =
            (good.price * good.num).ToString();
        float total = 0;
        foreach (var g in goodItemDic.Values)
        {
            total += Convert.ToSingle(g.transform.Find("TxtTotalMoney").GetComponent<TextMeshProUGUI>().text);
        }

        txtTotalMoney.text = total.ToString();
    }

    private void Start()
    {
        // StartCoroutine(CalculateTotalMoney());
    }

    // IEnumerator CalculateTotalMoney()
    // {
    //     WaitForSeconds wait = new WaitForSeconds(0.15f);
    //
    //     while (true)
    //     {
    //         float total = 0;
    //         print(goodItemDic.Values.Count + "------");
    //         foreach (var goodItem in goodItemDic.Values)
    //         {
    //             total += Convert.ToSingle(goodItem.transform.Find("TxtTotalMoney").GetComponent<TextMeshProUGUI>().text
    //             );
    //         }
    //
    //         print(total);
    //         txtTotalMoney.text = total.ToString();
    //         yield return wait;
    //     }
    // }
}
