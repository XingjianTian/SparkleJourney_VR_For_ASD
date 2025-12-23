using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(FixedPointMovement))]
public class FixedPointMovementEditor : Editor
{
    private SerializedProperty waypointsProperty;
    private int selectedWaypointIndex = -1;
    private const float sphereRadius = 0.2f;
    private const float handleSize = 0.5f;
    private const float selectionRadius = 0.3f; // 选择半径
    private bool isDragging = false;
    private Transform waypointsParent;

    private void OnEnable()
    {
        waypointsProperty = serializedObject.FindProperty("waypoints");
        FixedPointMovement walkBoy = (FixedPointMovement)target;
        // 在场景中查找已存在的父物体
        waypointsParent = GameObject.Find(walkBoy.name + "WayPoints")?.transform;
    }

    private void OnSceneGUI()
    {
        FixedPointMovement walkBoy = (FixedPointMovement)target;

        // 获取所有路径点
        serializedObject.Update();
        if (waypointsProperty == null || !waypointsProperty.isArray)
            return;

        // 处理鼠标事件
        HandleMouseEvents();

        // 绘制添加点按钮
        Handles.BeginGUI();
        SceneView sceneView = SceneView.currentDrawingSceneView;
        if (sceneView != null)
        {
            Rect addButtonRect = new Rect(10, sceneView.position.height - 50, 100, 20);
            if (GUI.Button(addButtonRect, "添加一个点"))
            {
                AddNewWaypoint(walkBoy);
            }

            // 添加删除按钮
            Rect deleteButtonRect = new Rect(120, sceneView.position.height - 50, 100, 20);
            GUI.enabled = selectedWaypointIndex != -1; // 只有在选中点时才启用删除按钮
            if (GUI.Button(deleteButtonRect, "删除选中点"))
            {
                DeleteSelectedWaypoint();
            }
            GUI.enabled = true; // 恢复GUI状态
        }
        Handles.EndGUI();

        // 绘制每个路径点及其坐标轴
        for (int i = 0; i < waypointsProperty.arraySize; i++)
        {
            SerializedProperty waypointProperty = waypointsProperty.GetArrayElementAtIndex(i);
            if (waypointProperty.objectReferenceValue == null)
                continue;

            Transform waypoint = (Transform)waypointProperty.objectReferenceValue;
            Vector3 position = waypoint.position;

            // 绘制小球
            Color sphereColor = (i == selectedWaypointIndex) ? Color.green : Color.yellow;
            Handles.color = sphereColor;
            Handles.SphereHandleCap(0, position, Quaternion.identity, sphereRadius, EventType.Repaint);

            // 如果被选中，绘制可拖动的坐标轴
            if (i == selectedWaypointIndex)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPosition = Handles.PositionHandle(position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(waypoint, "Move Waypoint");
                    waypoint.position = newPosition;
                    EditorUtility.SetDirty(waypoint);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            // 绘制路径点序号
            Handles.Label(position + Vector3.up * 0.5f, $"Point {i}");

            // 绘制连接线
            if (i < waypointsProperty.arraySize - 1)
            {
                SerializedProperty nextWaypointProperty = waypointsProperty.GetArrayElementAtIndex(i + 1);
                if (nextWaypointProperty.objectReferenceValue != null)
                {
                    Transform nextWaypoint = (Transform)nextWaypointProperty.objectReferenceValue;
                    Handles.color = Color.yellow;
                    Handles.DrawLine(position, nextWaypoint.position);
                }
            }
        }
    }

    private void AddNewWaypoint(FixedPointMovement walkBoy)
    {
        // 检查是否需要创建父物体
        if (waypointsParent == null)
        {
            // 再次检查场景中是否存在（以防在OnEnable之后被创建）
            waypointsParent = GameObject.Find(walkBoy.name + "WayPoints")?.transform;

            // 如果仍然不存在，才创建新的
            if (waypointsParent == null)
            {
                waypointsParent = new GameObject(walkBoy.name + "WayPoints").transform;
                waypointsParent.parent = null; // 设置为根物体
                Undo.RegisterCreatedObjectUndo(waypointsParent.gameObject, "Create Waypoints Parent");
            }
        }

        if (waypointsProperty.arraySize == 0)
        {
            // 如果没有点，在挂载脚本的物体位置创建一个
            CreateNewWaypoint(walkBoy.transform.position);
            return;
        }

        // 获取最后一个点
        SerializedProperty lastWaypointProperty = waypointsProperty.GetArrayElementAtIndex(waypointsProperty.arraySize - 1);
        if (lastWaypointProperty.objectReferenceValue == null)
            return;

        Transform lastWaypoint = (Transform)lastWaypointProperty.objectReferenceValue;
        Vector3 newPosition = lastWaypoint.position - lastWaypoint.forward;
        CreateNewWaypoint(newPosition);
    }

    private void CreateNewWaypoint(Vector3 position)
    {
        // 创建新的GameObject作为路径点
        GameObject newWaypoint = new GameObject($"Waypoint_{waypointsProperty.arraySize}");
        newWaypoint.transform.position = position;
        newWaypoint.transform.SetParent(waypointsParent);

        // 记录撤销操作
        Undo.RegisterCreatedObjectUndo(newWaypoint, "Create Waypoint");

        // 添加到waypoints列表
        waypointsProperty.arraySize++;
        SerializedProperty newWaypointProperty = waypointsProperty.GetArrayElementAtIndex(waypointsProperty.arraySize - 1);
        newWaypointProperty.objectReferenceValue = newWaypoint.transform;

        // 应用更改
        serializedObject.ApplyModifiedProperties();

        // 选中新创建的点
        selectedWaypointIndex = waypointsProperty.arraySize - 1;
    }

    private void HandleMouseEvents()
    {
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0 && !isDragging)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    float minDistance = float.MaxValue;
                    int closestIndex = -1;

                    for (int i = 0; i < waypointsProperty.arraySize; i++)
                    {
                        SerializedProperty waypointProperty = waypointsProperty.GetArrayElementAtIndex(i);
                        if (waypointProperty.objectReferenceValue == null)
                            continue;

                        Transform waypoint = (Transform)waypointProperty.objectReferenceValue;
                        Vector3 position = waypoint.position;

                        // 计算鼠标点击位置到路径点的屏幕空间距离
                        Vector2 mousePos = e.mousePosition;
                        Vector2 pointScreenPos = HandleUtility.WorldToGUIPoint(position);
                        float screenDistance = Vector2.Distance(mousePos, pointScreenPos);

                        // 使用屏幕空间距离来判断选择
                        if (screenDistance < minDistance && screenDistance < 20f) // 20像素的选择范围
                        {
                            minDistance = screenDistance;
                            closestIndex = i;
                        }
                    }

                    if (closestIndex != -1)
                    {
                        selectedWaypointIndex = closestIndex;
                        isDragging = true;
                        GUIUtility.hotControl = controlID;
                        e.Use();
                        Repaint();
                    }
                }
                break;

            case EventType.MouseUp:
                if (e.button == 0 && isDragging)
                {
                    isDragging = false;
                    GUIUtility.hotControl = 0;
                    e.Use();
                    Repaint();
                }
                break;

            case EventType.MouseDrag:
                if (isDragging)
                {
                    e.Use();
                }
                break;
        }
    }

    private void DeleteSelectedWaypoint()
    {
        if (selectedWaypointIndex == -1 || selectedWaypointIndex >= waypointsProperty.arraySize)
            return;

        // 获取要删除的点
        SerializedProperty waypointProperty = waypointsProperty.GetArrayElementAtIndex(selectedWaypointIndex);
        if (waypointProperty.objectReferenceValue == null)
            return;

        Transform waypoint = (Transform)waypointProperty.objectReferenceValue;

        // 记录撤销操作
        Undo.DestroyObjectImmediate(waypoint.gameObject);

        // 从数组中移除
        waypointsProperty.DeleteArrayElementAtIndex(selectedWaypointIndex);
        serializedObject.ApplyModifiedProperties();

        // 更新选中索引
        if (selectedWaypointIndex >= waypointsProperty.arraySize)
        {
            selectedWaypointIndex = waypointsProperty.arraySize - 1;
        }

        // 强制重绘
        Repaint();
    }
}