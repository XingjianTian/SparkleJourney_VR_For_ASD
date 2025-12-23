using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class PainttingObj : MonoBehaviour
{
    public bool Enable = true;
    public string baseMapName = "_BaseMap";
    private MeshRenderer m_meshRenderer;
    public MeshRenderer meshRenderer
    {
        get
        {
            if (m_meshRenderer == null)
            {
                m_meshRenderer = GetComponent<MeshRenderer>();
            }

            return m_meshRenderer;
        }
    }


    public RenderTexture painttingRT;

    public RenderTexture backupRT;

    private Material offlineRenderMat;
    // Start is called before the first frame update
    void Start()
    {
        if (!enabled)
            return;
        gameObject.layer = LayerMask.NameToLayer("DrawAble");
        gameObject.tag = "DrawAble";
        painttingRT = CreateRendererTexture();
        backupRT = CreateRendererTexture();
        offlineRenderMat = new Material(Shader.Find("Unlit/UnfoldUV"));
        Texture originTex = meshRenderer.material.GetTexture(baseMapName);
        if (originTex != null)
            Graphics.Blit(originTex,backupRT);

        offlineRenderMat.SetTexture("_PrevTex",backupRT);
        meshRenderer.material.SetTexture(baseMapName,painttingRT);

    }

    private void OnDisable()
    {
        if (painttingRT != null)
        {
            painttingRT.Release();
            backupRT.Release();
        }
    }

    private CommandBuffer commandBuffer;

    private Vector3 preMousePos = Vector3.zero;

    private Vector3 curMousePos = Vector3.zero;

    private bool isTouch = false;
    // Update is called once per frame
    void Update()
    {
        //测试用代码
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     LoadDrawedTexture(Application.dataPath+"/a.png");
        // }
        if (!enabled)
            return;
        commandBuffer = new CommandBuffer();
        commandBuffer.SetRenderTarget(painttingRT);
        commandBuffer.DrawRenderer(meshRenderer,offlineRenderMat);
        commandBuffer.Blit(painttingRT,backupRT);
        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Release();

        if (isTouch)
        {
            offlineRenderMat.SetInt("_EnablePaintting",1);
            offlineRenderMat.SetVector("_CurMousePos", new Vector4(curMousePos.x,curMousePos.y,curMousePos.z,0));
            if (preMousePos == Vector3.zero)
            {
                offlineRenderMat.SetVector("_PrevMousePos",new Vector4(curMousePos.x,curMousePos.y,curMousePos.z,0));
            }
            else
            {
                offlineRenderMat.SetVector("_PrevMousePos",new Vector4(preMousePos.x,preMousePos.y,preMousePos.z,0));
            }
            preMousePos = curMousePos;
        }
        else
        {
            offlineRenderMat.SetInt("_EnablePaintting",0);
            offlineRenderMat.SetVector("_CurMousePos",Vector4.zero);
            offlineRenderMat.SetVector("_PrevMousePos",Vector4.zero);
        }
        //测试用代码
        // if (Input.GetKeyDown(KeyCode.I))
        // {
        //     SavePainttingRenderTexture2Png(Application.dataPath, "a.png");
        // }
    }

    private RenderTexture CreateRendererTexture()
    {
        RenderTexture renderTexture = new RenderTexture(1024, 1024,32,DefaultFormat.LDR);
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.enableRandomWrite = true;
        return renderTexture;
    }

    public void TouchPointHandler(Vector3 point,Color burshColor,float brushSize = 1)
    {
        isTouch = true;
        curMousePos = point;
        offlineRenderMat.SetFloat("_BrushSize",brushSize);
        offlineRenderMat.SetColor("_MainColor",burshColor);
    }

    public void StopTouch()
    {
        isTouch = false;
        preMousePos = Vector3.zero;
    }

    public bool SavePainttingRenderTexture2Png(string filePath,string fileName)
    {
        if (!enabled)
        {
            Debug.LogWarning($"{gameObject.name}当前处于绘画完成贴图展示模式");
            return false;
        }
        if (!Directory.Exists(filePath))
        {
            Debug.LogError($"无法保存贴图，应为当前路径{filePath}不是一个有效的路径");
            return false;
        }
        RenderTexture preRT = RenderTexture.active;
        RenderTexture.active = painttingRT;
        Texture2D savedTex = new Texture2D(painttingRT.width, painttingRT.height, TextureFormat.ARGB32,false);
        savedTex.ReadPixels(new Rect(0,0,painttingRT.width,painttingRT.height),0,0);
        byte[] pngFile = savedTex.EncodeToPNG();
        using (FileStream fileStream = File.OpenWrite(Path.Combine(filePath,fileName)))
        {
            fileStream.Write(pngFile);
        }
        Texture2D.Destroy(savedTex);
        RenderTexture.active = preRT;
        return true;
    }

    public bool LoadDrawedTexture(string filePath)
    {
        if (enabled)
        {
            Debug.LogWarning($"{gameObject.name}当前处于绘画模式");
            return false;
        }
        if (!File.Exists(filePath))
        {
            Debug.LogError($"无法从{filePath}加载贴图");
            return false;
        }
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            Texture2D tex = new Texture2D(1024, 1024);
            byte[] pngFile = new byte[fileStream.Length];
            fileStream.Read(pngFile);
            tex.LoadImage(pngFile);

            meshRenderer.material.SetTexture(baseMapName, tex);
        }

        return true;
    }
}
