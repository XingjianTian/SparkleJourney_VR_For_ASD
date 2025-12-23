using System;
using System.Collections;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class FreeScene3DPainttingOnline : MonoBehaviour
{
    public InputActionReference button;
    public ColorPicker colorPicker;

    public Slider sliderLineWidth;
    public Button btnBack;
    private void OnEnable()
    {

        button.action.performed += OnPainttingStart;
        button.action.canceled += OnPainttingEnd;

        colorPicker.onColorChanged += OnColorChange;
    }

    // private void InitRelation()
    // {
    //     // colorPicker = transform.Find("Left Controller").Find("Canvas (1)").Find("Color picker")
    //     //     .GetComponent<ColorPicker>();
    //     colorPicker = transform.GetComponentInChildren<ColorPicker>();
    //     // sliderLineWidth = transform.Find("Left Controller").Find("Canvas (1)").Find("SliderLineWidth")
    //     //     .GetComponent<Slider>();
    //     sliderLineWidth = transform.GetComponentInChildren<Slider>();
    //     btnBack = transform.GetComponentInChildren<Button>();
    //     // btnBack = transform.Find("Left Controller").Find("Canvas (1)").Find("BtnBack")
    //     //     .GetComponent<Button>();
    //     BrushPoint = transform.Find("Camera Offset").Find("Right Controller").Find("BrushPoint").transform;
    // }



    private void OnDisable()
    {
        button.action.performed -= OnPainttingStart;
        button.action.performed -= OnPainttingEnd;

        colorPicker.onColorChanged -= OnColorChange;
    }

    private float lineWidth = 0.01f;
    private List<PainttingLine> lineObjList = new List<PainttingLine>();
    private void OnPainttingStart(InputAction.CallbackContext context)
    {
        isDraw = true;
        GameObject go = new GameObject();
        painttingLine = go.AddComponent<PainttingLine>();
        painttingLine.Init(lineWidth, lineMat, false);
        lineObjList.Add(painttingLine);
        prevPoint = BrushPoint.position;
    }

    private void OnPainttingEnd(InputAction.CallbackContext context)
    {
        isDraw = false;
    }


    // public Transform handTrans;
    public Transform BrushPoint;
    private Material brushPointMat;

    public Material lineMat;
    private bool isDraw;

    private PainttingLine painttingLine;

    private Vector3 prevPoint;
    // Start is called before the first frame update
    void Start()
    {
        sliderLineWidth.value = lineWidth * 10;
        sliderLineWidth.onValueChanged.AddListener((v =>
        {
            lineWidth = Mathf.Clamp(v * 0.1f,0.001f,0.1f);
            BrushPoint.localScale = lineWidth * Vector3.one;
        }));
        lineMat.color = colorPicker.color;

        BrushPoint.localScale = lineWidth * Vector3.one;
        btnBack.onClick.AddListener((() =>
        {
            int count = lineObjList.Count;
            if (count > 0)
            {
                lineObjList[count-1].RemoveLine();
                Destroy(lineObjList[count -1].gameObject);
                lineObjList.RemoveAt(count - 1);

            }
        }));
        InitLine();
    }

    private void InitLine()
    {
        StartCoroutine(HandleDraw());

        //初始化所有已经存在的线段
        FantasyNetMgr.Instance.Call(new ClientGetAllLineRequest(),(IResponse =>
        {
            ClientGetAllLineResponse res = IResponse as ClientGetAllLineResponse;
            foreach (Line line in res.line_list)
            {
                GameObject go = new GameObject();
                PainttingLine painttingLine = go.AddComponent<PainttingLine>();
                Material mat = new Material(Shader.Find("Unlit/FreePaintting"));
                mat.color = new Color(line.color.x, line.color.y, line.color.z);
                painttingLine.Init(line.lineWidth, mat, true,line.line_guid);
                foreach (var pos in line.pos_List)
                {
                    painttingLine.InsertPoint(new Vector3(pos.x,pos.y,pos.z));
                }
                OhterDrawLineDic.Add(line.line_guid,painttingLine);
            }
        } ));
    }

    private void OnColorChange(Color color)
    {
        lineMat = new Material(Shader.Find("Unlit/FreePaintting"));
        lineMat.color = color;
        if (brushPointMat == null)
        {
            brushPointMat = BrushPoint.GetComponent<MeshRenderer>().material;
        }

        brushPointMat.color = color;

    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator HandleDraw()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.05f);
        while (true)
        {
            if (isDraw)
            {
                prevPoint = BrushPoint.position;
                painttingLine.InsertPoint(prevPoint);
            }

            yield return waitForSeconds;
        }
    }

    public static Dictionary<string, PainttingLine> OhterDrawLineDic = new Dictionary<string, PainttingLine>();
    public static Dictionary<string, PointPackageCache> PointPackageCacheDic = new Dictionary<string, PointPackageCache>();
}





//Message

//缓存由于受到网络延迟导致的数据包顺序不对问题
public class PointPackageCache
{
    public int maxPoint;

    public List<ClientAddLinePointMessage> MessagesList = new List<ClientAddLinePointMessage>();

    public void AddPackage(ClientAddLinePointMessage message)
    {
        if (message.pos_index > maxPoint)
        {
            maxPoint = message.pos_index;
        }
        MessagesList.Add(message);
    }


    public void Dispose()
    {
        MessagesList.Clear();
        maxPoint = 0;
    }
}


public class ClientCreateLineMessageHandler: Message<ClientCreateLineMessage>
{
    protected async override FTask Run(Session session, ClientCreateLineMessage message)
    {
        GameObject go = new GameObject();
        PainttingLine painttingLine = go.AddComponent<PainttingLine>();
        Material mat = new Material(Shader.Find("Unlit/FreePaintting"));
        mat.color = new Color(message.color.x, message.color.y, message.color.z);
        painttingLine.Init(message.lineWidth, mat, true,message.line_guid);
        //处理数据包顺序
        if (FreeScene3DPainttingOnline.PointPackageCacheDic.ContainsKey(message.line_guid))
        {
            foreach (var messagePackage in FreeScene3DPainttingOnline.PointPackageCacheDic[message.line_guid].MessagesList)
            {
                painttingLine.InsertPoint(messagePackage.pos_index,new Vector3(messagePackage.pos.x,messagePackage.pos.y,messagePackage.pos.z));
            }
        }
        FreeScene3DPainttingOnline.OhterDrawLineDic.Add(message.line_guid,painttingLine);
        await FTask.CompletedTask;
    }
}

public class ClientAddLinePointMessageHandler: Message<ClientAddLinePointMessage>
{
    protected async override FTask Run(Session session, ClientAddLinePointMessage message)
    {
        if (FreeScene3DPainttingOnline.OhterDrawLineDic.ContainsKey(message.line_guid))
        {
            FreeScene3DPainttingOnline.OhterDrawLineDic[message.line_guid].InsertPoint(message.pos_index,new Vector3(message.pos.x,message.pos.y,message.pos.z));
        }
        else
        {
            if (FreeScene3DPainttingOnline.PointPackageCacheDic.ContainsKey(message.line_guid))
            {
                FreeScene3DPainttingOnline.PointPackageCacheDic[message.line_guid].AddPackage(message);
            }
            else
            {
                PointPackageCache pointPackageCache = new PointPackageCache();
                pointPackageCache.AddPackage(message);
                FreeScene3DPainttingOnline.PointPackageCacheDic.Add(message.line_guid,pointPackageCache);
            }
        }
    }
}

public class ClientRemoveLineMessageHandler: Message<ClientRemoveLineMessage>
{
    protected async override FTask Run(Session session, ClientRemoveLineMessage message)
    {
        if (FreeScene3DPainttingOnline.OhterDrawLineDic.ContainsKey(message.line_guid))
        {

            if (FreeScene3DPainttingOnline.PointPackageCacheDic.ContainsKey(message.line_guid))
            {
                FreeScene3DPainttingOnline.PointPackageCacheDic.Remove(message.line_guid);
            }
            PainttingLine line = FreeScene3DPainttingOnline.OhterDrawLineDic[message.line_guid];
            GameObject.Destroy(line.gameObject);
            FreeScene3DPainttingOnline.OhterDrawLineDic.Remove(message.line_guid);
        }
    }
}