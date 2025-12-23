using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCardPanel : MonoBehaviour
{
    public Slider sliderHub;

    public RawImage paleteeImage;
    private Image sliderBackgroundImg;

    // Start is called before the first frame update
    void Start()
    {
        sliderBackgroundImg = sliderHub.transform.Find("Background").GetComponent<Image>();
        Vector2 texSize = (sliderHub.transform as RectTransform).sizeDelta;
        Texture2D sliderBG = new Texture2D((int)texSize.x, (int)texSize.y,TextureFormat.RGB24,true);
        Vector2 paleteeTexSize = (paleteeImage.transform as RectTransform).sizeDelta;
        paleteeTex = new Texture2D((int)paleteeTexSize.x,(int)paleteeTexSize.y, TextureFormat.RGB24, true);
        for (int i = 0; i < sliderBG.width; i++)
        {
            for (int j = 0; j < sliderBG.height; j++)
            {
                Color pixelColor = Color.HSVToRGB((float)i / (float)(sliderBG.width - 1), 1, 1);
                sliderBG.SetPixel(i,j,pixelColor);
            }
        }
        sliderBG.Apply();
        sliderBackgroundImg.sprite = Sprite.Create(sliderBG,new Rect(0,0,sliderBG.width,sliderBG.height),new Vector2(0.5f, 0.5f), 100);
        UpdatePalette(sliderHub.value);
        sliderHub.onValueChanged.AddListener(v =>
        {
            UpdatePalette(v);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Texture2D paleteeTex;
    private void UpdatePalette(float v)
    {
        Color targetColor = Color.HSVToRGB(v, 1, 1);
        Color curPixelColor;
        for (int i = 0; i < paleteeTex.width; i++)
        {
            curPixelColor = Color.white + (targetColor - Color.white) * (float)i / (paleteeTex.width - 1);
            paleteeTex.SetPixel(i,paleteeTex.height - 1,curPixelColor);
        }
        paleteeTex.Apply();
        for (int i = 0; i < paleteeTex.width; i++)
        {
            Color yLineTargetColor = paleteeTex.GetPixel(i, paleteeTex.height - 1);
            for (int j = 0; j < paleteeTex.height - 1; j++)
            {
                curPixelColor = Color.black + (yLineTargetColor - Color.black) * (float)j /
                    (paleteeTex.height - 1);
                paleteeTex.SetPixel(i,j,curPixelColor);
            }
        }

        paleteeTex.Apply();
        paleteeImage.texture = paleteeTex;
    }
}
