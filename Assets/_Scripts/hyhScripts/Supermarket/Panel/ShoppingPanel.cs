
using UnityEngine;
using UnityEngine.UI;

public class ShoppingPanel : SupermarketPanelMgr<ShoppingPanel>
{
    public GameObject content;
    private Text[] TextList;
    public Text currentPrice;
    
    void Start()
    {
        content.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        TextList = content.GetComponentsInChildren<Text>();
        HideDis();
    }

    public void AddPanelData(GameObject productObj)
    {
        ProductInformation product = productObj.GetComponent<ProductInformation>();
        for (int i = 0; i < TextList.Length; i++)
        {
            string[] PanelProductInfomation = TextList[i].text.Split("\t\t\t\t");
            if (PanelProductInfomation[0] == product.Name)
            {
                int CurrentProductNumber = int.Parse(PanelProductInfomation[1]) + 1;
                int CurrentProductPrice = int.Parse(PanelProductInfomation[2]) + product.Price;
                
                TextList[i].text = product.Name + "\t\t\t\t" + CurrentProductNumber + "\t\t\t\t" + CurrentProductPrice;
                break;
            }
            if (TextList[i].text == "")
            {
                TextList[i].text = product.Name + "\t\t\t\t1\t\t\t\t" + product.Price;
                content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,content.GetComponent<RectTransform>().sizeDelta.y + 19.2f);
                break;
            } 
        }

        Judge();
    }

    public void RemovePanelData(GameObject productObj)
    {
        ProductInformation product = productObj.GetComponent<ProductInformation>();
        for (int i = 0; i < TextList.Length; i++)
        {
            string[] PanelProductInfomation = TextList[i].text.Split("\t\t\t\t");
            if (PanelProductInfomation[0] == product.Name)
            {
                int CurrentProductNumber = int.Parse(PanelProductInfomation[1]) - 1;
                int CurrentProductPrice = int.Parse(PanelProductInfomation[2]) - product.Price;

                if (CurrentProductNumber == 0)
                {
                    TextList[i].text = "";
                    content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,content.GetComponent<RectTransform>().sizeDelta.y - 19.2f);
                }
                else TextList[i].text = product.Name + "\t\t\t\t" + CurrentProductNumber + "\t\t\t\t" + CurrentProductPrice;
                break;
            }
        }

        Judge();
    }

    private void Judge()
    {
        //计算当前总价
        int currentPriceInt = 0;
        foreach (Text textobj in TextList)
        {
            if (textobj.text == "")
            {
                currentPrice.text = currentPriceInt.ToString();
                return;
            }
            currentPriceInt += int.Parse(textobj.text.Split("\t\t\t\t")[2]);
        }
    }
}
