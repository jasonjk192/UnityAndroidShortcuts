using System.IO;
using UnityEditor;
using UnityEngine;

namespace WC.Shortcuts
{
    [CustomEditor(typeof(ShortcutManager))]
    public class ShortcutManagerEditor : Editor
    {
        private ShortcutManager manager = null;
        private string editorFolderPath = "Assets/Shortcut/Scripts/Editor/"; // Default location

        private void OnEnable()
        {
            manager = target as ShortcutManager;

            // Check the default location
            if (!File.Exists(Path.Combine(editorFolderPath, "ShortcutManagerEditor.cs")))
                editorFolderPath = GetEditorPath();
        }

        private void OnDisable()
        {
            manager = null;
        }

        public override void OnInspectorGUI()
        {
            bool isApplicationPlaying = Application.isPlaying;
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor Only", GUI.skin.label);

            if(!isApplicationPlaying)
            {
                manager.simulateColdStart = EditorGUILayout.Toggle("Simulate ColdStart", manager.simulateColdStart);
            }
            CreateSelectableIDList();

            if (isApplicationPlaying)
            {
                if (GUILayout.Button("Simulate Background"))
                {
                    manager.Simulate(manager.simulationShortcutID, ShortcutTriggerType.BACKGROUND);
                }
            }
            else
            {
                if (GUILayout.Button("Edit Simulation Shortcuts"))
                {
                    string targetScriptPath = Path.Combine(editorFolderPath, "ShortcutManagerEditorData.cs");
                    if (File.Exists(targetScriptPath))
                    {
                        Object obj = AssetDatabase.LoadAssetAtPath<Object>(targetScriptPath);
                        AssetDatabase.OpenAsset(obj);
                    }
                    else
                    {
                        Debug.LogWarning("ShortcutManagerEditorData could not found in the editor folder.");
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(manager);
            }
        }

        private void CreateSelectableIDList()
        {
            int selectedIndex = System.Array.IndexOf(ShortcutManagerEditorData.simulationShortcutIDs, manager.simulationShortcutID);
            if (selectedIndex == -1) selectedIndex = 0;
            selectedIndex = EditorGUILayout.Popup("Shortcut ID", selectedIndex, ShortcutManagerEditorData.simulationShortcutIDs);
            manager.simulationShortcutID = ShortcutManagerEditorData.simulationShortcutIDs[selectedIndex];
        }

        /// <summary>Using this script asset, we get the editor's path</summary>
        private string GetEditorPath()
        {
            var editorScript = MonoScript.FromScriptableObject(this);
            string scriptPath = AssetDatabase.GetAssetPath(editorScript);
            return Path.GetDirectoryName(scriptPath);
        }
    }

}

