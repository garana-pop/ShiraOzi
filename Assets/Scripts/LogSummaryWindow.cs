using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LogSummaryWindow : EditorWindow
{
    private Vector2 scrollPos;
    private List<string> summarizedLogs = new List<string>();

    [MenuItem("Tools/Log Summary")]
    public static void ShowWindow()
    {
        GetWindow<LogSummaryWindow>("Log Summary");
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        summarizedLogs.Clear();
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (!string.IsNullOrEmpty(condition))
        {
            string firstLine = condition.Split('\n')[0];
            summarizedLogs.Add(firstLine);
            Repaint();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
            summarizedLogs.Clear();
        }
        if (GUILayout.Button("Copy All"))
        {
            EditorGUIUtility.systemCopyBuffer = string.Join("\n", summarizedLogs);
            Debug.Log("All summarized logs copied to clipboard.");
        }
        EditorGUILayout.EndHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (summarizedLogs.Count == 0)
        {
            EditorGUILayout.LabelField("No logs captured yet.");
        }
        else
        {
            foreach (var log in summarizedLogs)
            {
                EditorGUILayout.SelectableLabel(log, GUILayout.Height(16));
            }
        }

        EditorGUILayout.EndScrollView();
    }
}
