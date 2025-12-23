using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateWavFileFromString : EditorWindow
{
    private static GenerateWavFileFromString window;
    [MenuItem("Custom/生成音频")]
    private static void ShowWindow()
    {
        window = GetWindow<GenerateWavFileFromString>();
        window.titleContent = new GUIContent("生成音频面板");
    }

    private void OnEnable()
    {
        if (tone == "")
        {
            tone = "103";
        }

        if (defaultSavedPath == "")
        {
            defaultSavedPath = Application.dataPath+"/Resources/Music/EnvMusic/" + SceneManager.GetActiveScene().name +"/";
            if (!Directory.Exists(defaultSavedPath))
            {
                Directory.CreateDirectory(defaultSavedPath);
            }
        }

        fileName = GetDefaultFileName();
    }

    private string GetDefaultFileName()
    {
        int count = 1;
        foreach (var iFile in Directory.GetFiles(defaultSavedPath))
        {
            if (!iFile.EndsWith(".meta"))
            {
                count += 1;
            }
        }
        return count.ToString() + ".wav";
    }

    private static string defaultSavedPath = "";
    private string convertTxt = "";
    private string fileName = "";
    private static string tone = "";

    //temp
    private GUIStyle style;
    private async void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:");
        defaultSavedPath = EditorGUILayout.TextField(defaultSavedPath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("请输入文本");
        style = new GUIStyle();
        // style.fixedWidth = window.;
        style.fixedHeight = 300;
        style.fontSize = 18;
        style.normal.background = EditorGUIUtility.whiteTexture;
        convertTxt = EditorGUILayout.TextArea(convertTxt,style);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("音色");
        tone = EditorGUILayout.TextField(tone);
        EditorGUILayout.LabelField("请输入文件名");
        fileName = EditorGUILayout.TextField(fileName);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("转换"))
        {
            if (convertTxt != "")
            {
                byte[] data = await Text2SpeechHandle.Instance.Send(convertTxt,tone: tone);
                if (data != null)
                {
                    Debug.Log($"{fileName} --转换成功");
                }
                string path = defaultSavedPath + fileName;
                using (FileStream stream = File.OpenWrite(path))
                {
                    stream.Write(data);
                }
                fileName = GetDefaultFileName();
                AssetDatabase.Refresh();
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            }
        }

    }
}
