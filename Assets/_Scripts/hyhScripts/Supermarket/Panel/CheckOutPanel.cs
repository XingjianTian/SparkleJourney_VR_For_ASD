using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CheckOutPanel : SupermarketPanelMgr<CheckOutPanel>
{
    public Text currentBalance;
    public Button checkOutButton;

    void Start()
    {
        checkOutButton.onClick.AddListener(()=>
        {
            ShoppingPanel.Instance.Hide();
            Show();
            RobotObj.Instance.SpeechText("恭喜你完成了购物任务，现在让我们去公园里休闲一下吧");
            SupermaketMenuMgr.isDone = true;
        });
        HideDis();
    }

    public override void Show()
    {
        base.Show();
        //计算当前余额
        currentBalance.text = "余额:" + (100 - int.Parse(ShoppingPanel.Instance.currentPrice.text)).ToString();
        Invoke(nameof(Hide),3);
    }
}
