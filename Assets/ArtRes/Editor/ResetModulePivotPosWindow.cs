using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ResetModulePivotPosWindow : EditorWindow
{

    [MenuItem("Custom/ResetModulePivotPosWindow")]
    private static void ShowWindow()
    {
        ResetModulePivotPosWindow window = GetWindow<ResetModulePivotPosWindow>();
        window.minSize = new Vector2(100, 100);
        window.maxSize = new Vector2(100, 100);

    }
    private GameObject activeObj;


    private void OnEnable()
    {
        activeObj = Selection.activeGameObject;
        if (activeObj!= null)
        {
            flagGO = new GameObject(activeObj.name);
            flagGO.transform.parent = activeObj.transform.parent;
            // flagGO.transform.position = activeObj.transform.position;
            flagGO.transform.position = activeObj.transform.TransformPoint(activeObj.GetComponent<MeshFilter>().mesh.bounds.center);
            Selection.activeObject = flagGO;
        }

        isDone = false;
    }

    private void OnDisable()
    {
        if (!isDone && flagGO != null)
        {
            GameObject.DestroyImmediate(flagGO);
        }
    }

    private GameObject flagGO;
    private bool isDone;
    private void OnGUI()
    {
        if (activeObj == null)
        {
            EditorGUILayout.LabelField("请选择游戏物体");
            return;
        }

        if (GUILayout.Button("确定"))
        {
            isDone = true;
            activeObj.transform.parent = flagGO.transform;
            GetWindow<ResetModulePivotPosWindow>().Close();
        }
    }

}
