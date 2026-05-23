using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class HierarchyExporter : EditorWindow
{
    [MenuItem("Tools/Export Hierarchy to Text")]
    public static void ExportHierarchyToText()
    {
        string path = EditorUtility.SaveFilePanel("Save Hierarchy as Text", "", "hierarchy.txt", "txt");
        if (string.IsNullOrEmpty(path)) return;

        StringBuilder sb = new StringBuilder();
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            ProcessGameObject(rootObject, 0, sb);
        }

        File.WriteAllText(path, sb.ToString());
        Debug.Log("Hierarchy exported to: " + path);
    }

    private static void ProcessGameObject(GameObject go, int depth, StringBuilder sb)
    {
        // Add indentation based on depth
        sb.Append(new string(' ', depth * 2));

        // Add GameObject name
        sb.AppendLine(go.name);

        // Process children
        foreach (Transform child in go.transform)
        {
            ProcessGameObject(child.gameObject, depth + 1, sb);
        }
    }

    [MenuItem("Tools/Print Hierarchy to Console")]
    public static void PrintHierarchyToConsole()
    {
        StringBuilder sb = new StringBuilder();
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            ProcessGameObject(rootObject, 0, sb);
        }

        Debug.Log("Current Hierarchy:\n" + sb.ToString());
    }
}