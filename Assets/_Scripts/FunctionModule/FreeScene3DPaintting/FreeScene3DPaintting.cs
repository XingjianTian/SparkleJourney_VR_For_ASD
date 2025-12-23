using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FreeScene3DPaintting : MonoBehaviour
{
    public InputActionReference button;
    public ColorPicker colorPicker;

    public Slider sliderLineWidth;
    public Button btnBack;
    public Image imgPreview;
    private void OnEnable()
    {
        button.action.performed += OnPainttingStart;
        button.action.canceled += OnPainttingEnd;

        colorPicker.onColorChanged += OnColorChange;

    }




    private void OnDisable()
    {
        button.action.performed -= OnPainttingStart;
        button.action.performed -= OnPainttingEnd;

        colorPicker.onColorChanged -= OnColorChange;
    }

    private float lineWidth = 0.01f;
    private List<GameObject> lineObjList = new List<GameObject>();
    private void OnPainttingStart(InputAction.CallbackContext context)
    {
        isDraw = true;
        GameObject go = new GameObject();
        lineObjList.Add(go);
        lineRenderer = go.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = lineMat;
        lineRenderer.positionCount = 0;
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

    private LineRenderer lineRenderer;

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
        imgPreview.color = colorPicker.color;

        BrushPoint.localScale = lineWidth * Vector3.one;
        btnBack.onClick.AddListener((() =>
        {
            int count = lineObjList.Count;
            if (count > 0)
            {
                Destroy(lineObjList[count -1]);
                lineObjList.RemoveAt(count - 1);

            }
        }));
    }
    private void OnColorChange(Color color)
    {
        lineMat = new Material(Shader.Find("Unlit/FreePaintting"));
        lineMat.color = color;
        imgPreview.color = color;
        if (brushPointMat == null)
        {
            brushPointMat = BrushPoint.GetComponent<MeshRenderer>().material;
        }

        brushPointMat.color = color;

    }

    // Update is called once per frame
    void Update()
    {
        if (isDraw)
        {
            // if (Vector3.Distance(handTrans.position,prevPoint) )
            {
                prevPoint = BrushPoint.position;
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1,prevPoint);
            }
        }
    }
}
