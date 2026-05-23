using UnityEditor;
using UnityEngine;
using ShiraOzi.Core;

namespace ShiraOzi.EditorTools
{
    public class SaveResetTool : EditorWindow
    {
        private GameState gameState;

        [MenuItem("ShiraOzi/Tools/Save Reset Tool")]
        public static void ShowWindow()
        {
            SaveResetTool window = GetWindow<SaveResetTool>("Save Reset Tool");
            window.minSize = new Vector2(300, 150);
            window.Show();
        }

        private void OnEnable()
        {
            FindGameState();
        }

        private void FindGameState()
        {
            string[] guids = AssetDatabase.FindAssets("t:GameState");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                gameState = AssetDatabase.LoadAssetAtPath<GameState>(path);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Save Data Reset Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (gameState == null)
            {
                EditorGUILayout.HelpBox("GameState asset not found in the project!", MessageType.Error);
                if (GUILayout.Button("Retry Find GameState"))
                {
                    FindGameState();
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Target Asset: {AssetDatabase.GetAssetPath(gameState)}", MessageType.Info);
                
                EditorGUILayout.Space(10);
                
                GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
                if (GUILayout.Button("RESET GAME STATE", GUILayout.Height(50)))
                {
                    if (EditorUtility.DisplayDialog("Reset Game State", 
                        "Are you sure you want to reset all game progress, inventory, and unlocked diary entries?\n\nThis will modify the GameState asset directly.", 
                        "Reset", "Cancel"))
                    {
                        ResetSaveData();
                    }
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.Space(5);
                if (GUILayout.Button("Select GameState Asset"))
                {
                    Selection.activeObject = gameState;
                }
            }

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Clear PlayerPrefs"))
            {
                if (EditorUtility.DisplayDialog("Clear PlayerPrefs", "Are you sure you want to clear all PlayerPrefs data?", "Clear", "Cancel"))
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    Debug.Log("PlayerPrefs cleared.");
                }
            }
        }

        private void ResetSaveData()
        {
            if (gameState != null)
            {
                Undo.RecordObject(gameState, "Reset Game State");
                gameState.ResetState();
                EditorUtility.SetDirty(gameState);
                AssetDatabase.SaveAssets();
                Debug.Log($"<color=green>Game State has been reset successfully.</color> Target: {AssetDatabase.GetAssetPath(gameState)}", gameState);
            }
        }
    }
}
