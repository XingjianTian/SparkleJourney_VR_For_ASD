using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fantasy;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PainttingLine : MonoBehaviour
{
    public string UnitID;
    private bool isremoteAlread;
    private LineRenderer _lineRenderer;
    public async Task<string> Init(float lineWidth, Material lineMat,bool isremoteAlread = false,string UnitID ="")
    {
        this.isremoteAlread = isremoteAlread;
        if (UnitID == "")
        {
            this.UnitID = Guid.NewGuid().ToString();
        }
        else
        {
            this.UnitID = UnitID;
        }
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        _lineRenderer.material = lineMat;
        _lineRenderer.positionCount = 0;


        Fantasy.Vector3 color = new Fantasy.Vector3();
        color.x = lineMat.color.r;
        color.y = lineMat.color.g;
        color.z = lineMat.color.b;
        if (!isremoteAlread)
        {
            //发送创建请求
            FantasyNetMgr.Instance.Send(new ClientCreateLineMessage()
            {
                socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID(),
                line_guid = this.UnitID,
                lineWidth = lineWidth,
                color = color
            });
        }
        return this.UnitID;
    }

    public async void InsertPoint(Vector3 pos)
    {
        _lineRenderer.positionCount += 1;
        _lineRenderer.SetPosition(_lineRenderer.positionCount-1,pos);
        if (!isremoteAlread)
        {
            //发送请求同步组件信息
            Fantasy.Vector3 pointPos = new Fantasy.Vector3();
            pointPos.x = pos.x;
            pointPos.y = pos.y;
            pointPos.z = pos.z;
            FantasyNetMgr.Instance.Send(new ClientAddLinePointMessage()
            {
                socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID(),
                line_guid = UnitID,
                pos_index = _lineRenderer.positionCount-1,
                pos = pointPos
            });
        }
    }
    public async void InsertPoint(int index ,Vector3 pos)
    {
        if (_lineRenderer.positionCount-1 < index)
        {
            _lineRenderer.positionCount = index + 1;
        }
        _lineRenderer.SetPosition(index,pos);
        if (!isremoteAlread)
        {
            //发送请求同步组件信息
            Fantasy.Vector3 pointPos = new Fantasy.Vector3();
            pointPos.x = pos.x;
            pointPos.y = pos.y;
            pointPos.z = pos.z;
            FantasyNetMgr.Instance.Send(new ClientAddLinePointMessage()
            {
                socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID(),
                line_guid = UnitID,
                pos_index = _lineRenderer.positionCount-1,
                pos = pointPos
            });
        }
    }

    public async void RemoveLine()
    {
        if (!isremoteAlread)
        {
            //发送销毁请求
            FantasyNetMgr.Instance.Send(new ClientRemoveLineMessage()
            {
                socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID(),
                line_guid = UnitID
            });
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
