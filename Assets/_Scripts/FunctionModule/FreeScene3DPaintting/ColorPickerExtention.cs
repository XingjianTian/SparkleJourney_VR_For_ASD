using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ColorItem
{
    public Button btn;
    public Color color;
}


public class ColorPickerExtention : MonoBehaviour
{
    public List<ColorItem> ColorItemList = new List<ColorItem>();

    private ColorPicker _colorPicker;
    // Start is called before the first frame update
    void Start()
    {
        _colorPicker = GetComponent<ColorPicker>();
        foreach (ColorItem colorItem in ColorItemList)
        {
            colorItem.btn.transition = Selectable.Transition.None;
            colorItem.color = colorItem.btn.GetComponent<Image>().color;
            colorItem.btn.onClick.AddListener((() =>
            {
                _colorPicker.color = colorItem.color;
            }));
        }

        _colorPicker.color = ColorItemList.Count > 0 ? ColorItemList[0].color : Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
