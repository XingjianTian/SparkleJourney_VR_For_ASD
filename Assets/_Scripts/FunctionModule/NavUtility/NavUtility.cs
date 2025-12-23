using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavUtility: AutoMonoSingleton<NavUtility>
{
    private float groundTestHeight = 10;
    private float lineDrawOffset = 0.03f;
    private float quality = 5;
    private void Awake()
    {

    }


    // private void InitLineRenderer(LineRenderer lineRenderer)
    // {
    //     //init linerenderer
    //     lineRenderer = gameObject.AddComponent<LineRenderer>();
    //     lineRenderer.textureMode = LineTextureMode.Tile;
    //     lineRenderer.material = Resources.Load<Material>("Materials/Nav/ShowNavPath");
    //     lineRenderer.endWidth = 1;
    //     lineRenderer.startWidth = 1;
    // }


    private Dictionary<Transform, LineRenderer> cache = new Dictionary<Transform, LineRenderer>();
    private Dictionary<Coroutine, Transform> coroutineMapping = new Dictionary<Coroutine, Transform>();

    public Coroutine ShowNavPath(Transform start, Vector3 end)
    {
        if (cache.ContainsKey(start))
        {
            Debug.LogError($"一个游戏物体无法重复开启寻路标志");
            return null;
        }

        LineRenderer lineRenderer = CachePool.Instance.Pop<LineRenderer>("CachePool/LineRenderer");
        lineRenderer.enabled = true;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.material = Resources.Load<Material>("Materials/Nav/ShowNavPath");
        lineRenderer.endWidth = 1;
        lineRenderer.startWidth = 1;
        // InitLineRenderer(lineRenderer);
        Coroutine coroutine = StartCoroutine(DrawGroundGuide(start,end,lineRenderer));
        coroutineMapping.Add(coroutine, start);
        cache.Add(start,lineRenderer);
        return coroutine;
    }
    
    
    private NavMeshPath navMeshPath;
    IEnumerator DrawGroundGuide(Transform start, Vector3 end,LineRenderer lineRenderer)
    {
        navMeshPath = new NavMeshPath();
        WaitForSeconds wait = new WaitForSeconds(0.06f);
        while (true)
        {
            if (NavMesh.CalculatePath(start.position, end, NavMesh.AllAreas, navMeshPath))
            {
                List<Vector3> newCorners = new List<Vector3>();
                Vector3[] corners = navMeshPath.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Vector3 currentPoint = corners[i] + Vector3.up * groundTestHeight;
                    Vector3 nextPoint = corners[i + 1] + Vector3.up * groundTestHeight;
                    for (int j = 0; j < quality; j++)
                    {
                        Vector3 lerpPoint = Vector3.Lerp(currentPoint, nextPoint, 1 / quality * j);
                        if (Physics.Raycast(lerpPoint,Vector3.down,out RaycastHit hit,Mathf.Infinity,1 << LayerMask.NameToLayer("Ground")))
                        {
                            newCorners.Add(hit.point + new Vector3(0,lineDrawOffset,0));
                        }
                    }

                }
                newCorners.Add(end);
                lineRenderer.positionCount = newCorners.Count;
                //设置linerenderer的点
                for (int j = 0; j < newCorners.Count; j++)
                {
                    lineRenderer.SetPosition(j,newCorners[j]);
                }
            }
            yield return wait;
        }
    }

    public void HideNavPath(Coroutine id)
    {
        if (coroutineMapping.ContainsKey(id))
        {
            LineRenderer lineRenderer = cache[coroutineMapping[id]];
            lineRenderer.enabled = false;
            CachePool.Instance.Push(lineRenderer);
            StopCoroutine(id);
            cache.Remove(coroutineMapping[id]);
            coroutineMapping.Remove(id);
        }

    }

    public void HideAllNavPath()
    {
        List<Coroutine> idList = coroutineMapping.Keys.ToList();
        foreach (var id in idList)
        {
            HideNavPath(id);
        }
    }

    /// <summary>
    /// 设置从多高检测地面上的点
    /// </summary>
    /// <param name="data"></param>
    
    public void SetGroundTestHeight(float data)
    {
        groundTestHeight = data;
    }

    /// <summary>
    /// 设置箭头的精准程度
    /// </summary>
    /// <param name="data"></param>
    public void SetLineQuality(float data)
    {
        quality = data;
    }
    
    /// <summary>
    /// 设置画的箭头离地高度
    /// </summary>
    /// <param name="data"></param>
    public void SetDrawLineOffset(float data)
    {
        lineDrawOffset = data;
    }

}
