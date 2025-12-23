using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class FindFileThen100M : EditorWindow
{
    [MenuItem("Custom/查找大于100M的文件")]
    private static void ShowWindow()
    {
        FindFileThen100M window = EditorWindow.GetWindow<FindFileThen100M>("查找大于100M的文件");
        window.minSize = new Vector2(100, 100);
        window.maxSize = new Vector2(500, 500);
        window.Show();
    }

    private void OnEnable()
    {
        filePathList.Clear();
        basePath = Application.dataPath;
    }

    private List<string> filePathList = new List<string>();
    private string basePath;

    private bool isFindButtonClicked;
    private void OnGUI()
    {
        basePath = EditorGUILayout.TextField("BasePath", basePath);
        if (GUILayout.Button("查找"))
        {
            filePathList.Clear();
            isFindButtonClicked = true;
            FindFile(basePath);
        }

        if (filePathList.Count > 0)
        {
            EditorGUILayout.Space(50);
            EditorGUILayout.LabelField("找到的文件");
            for (int i = 0; i < filePathList.Count; i++)
            {
                if (GUILayout.Button($"{filePathList[i]}"))
                {
                    string s = filePathList[i];
                    s = s.Substring(s.IndexOf("/Assets") + 1);
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(s);
                }
            }
        } else if (isFindButtonClicked)
        {
            EditorGUILayout.Space(50);
            EditorGUILayout.LabelField("没有找到的文件");
        }

    }

    private void FindFile(string basePath)
    {
        if (!Directory.Exists(basePath))
        {
            return;
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(basePath);

        FileInfo[] fileInfos = directoryInfo.GetFiles();
        for (int i = 0; i < fileInfos.Length; i++)
        {
            FileInfo info = fileInfos[i];
            if ((info.Length / 1024 / 1024) > 100)
            {
                filePathList.Add(basePath+"/"+info.Name);
            }
        }
        DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
        for (int i = 0; i < directoryInfos.Length; i++)
        {
            FindFile(basePath + "/"+ directoryInfos[i].Name);
        }
    }
}
