using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PassageController))]
public class PassageControllerEditor : Editor
{
    private void OnSceneGUI()
    {
        PassageController passageController = (PassageController)target;
        DrawPassageIndicator(passageController, GizmoType.Selected);
    }

    // 在Scene视图中始终绘制标志
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawPassageIndicator(PassageController passageController, GizmoType gizmoType)
    {
        if (passageController == null) return;

        // 获取通行状态
        bool isPassageAllowed = passageController.IsPassageAllowed();

        // 设置标志的位置（在控制器上方）
        Vector3 position = passageController.transform.position + Vector3.up * 2f;

        // 设置标志的旋转（面向摄像机）
        Quaternion rotation = Quaternion.LookRotation(SceneView.currentDrawingSceneView.camera.transform.forward);

        // 设置标志的缩放
        float scale = HandleUtility.GetHandleSize(position) * 0.5f;

        // 保存当前矩阵
        Handles.matrix = Matrix4x4.TRS(position, rotation, Vector3.one * scale);

        // 绘制通行状态标志
        if (isPassageAllowed)
        {
            // 绘制绿色通行标志
            Handles.color = new Color(0, 1, 0, 0.5f); // 半透明绿色
            Handles.DrawSolidDisc(Vector3.zero, Vector3.forward, 1f);
            Handles.color = Color.green;
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 1f);

            // 绘制对勾
            Handles.color = Color.white;
            Vector3[] checkMark = new Vector3[]
            {
                new Vector3(-0.5f, 0, 0),
                new Vector3(0, 0.5f, 0),
                new Vector3(0.5f, -0.5f, 0)
            };
            Handles.DrawAAPolyLine(3f, checkMark);
        }
        else
        {
            // 绘制红色禁止标志
            Handles.color = new Color(1, 0, 0, 0.5f); // 半透明红色
            Handles.DrawSolidDisc(Vector3.zero, Vector3.forward, 1f);
            Handles.color = Color.red;
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 1f);

            // 绘制X
            Handles.color = Color.white;
            Vector3[] xMark1 = new Vector3[]
            {
                new Vector3(-0.5f, 0.5f, 0),
                new Vector3(0.5f, -0.5f, 0)
            };
            Vector3[] xMark2 = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, 0.5f, 0)
            };
            Handles.DrawAAPolyLine(3f, xMark1);
            Handles.DrawAAPolyLine(3f, xMark2);
        }

        // 恢复矩阵
        Handles.matrix = Matrix4x4.identity;
    }
}