using Destruction.Common;
using UnityEditor;
using UnityEngine;

namespace Destruction.Tools
{
    internal class CleanupTool : IDestructionTool
    {
        private SerializedObject obj;

        private SerializedProperty destroyTime;
        private SerializedProperty useDestroyTimer;

        private SerializedProperty destroyWhenOffscreen;
        private SerializedProperty offscreenTimer;

        #region IDestructionTool
        public string Name
        {
            get { return "Cleanup"; }
        }

        public void OnEnable(Object[] targets)
        {
            obj = new SerializedObject(targets);

            destroyTime = obj.FindProperty("destroyTime");
            useDestroyTimer = obj.FindProperty("useDestroyTimer");

            destroyWhenOffscreen = obj.FindProperty("destroyWhenOffscreen");
            offscreenTimer = obj.FindProperty("offscreenTimer");
        }

        public void OnSceneGUI(BaseDestructable target)
        {
        }

        public void OnInspectorGUI(BaseDestructable[] target)
        {
            if (target == null) return;

            obj.Update();

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(destroyWhenOffscreen, new GUIContent("Destroy When Offscreen"));
            if (destroyWhenOffscreen.boolValue)
            {
                EditorGUILayout.PropertyField(offscreenTimer, new GUIContent("Time To Live Offscreen"), true);
            }
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(useDestroyTimer, new GUIContent("Destroy After Time"));
            if (useDestroyTimer.boolValue)
            {
                EditorGUILayout.PropertyField(destroyTime, new GUIContent("Time To Live"), true);
            }
            EditorGUILayout.Separator();

            obj.ApplyModifiedProperties();
        }

        #endregion
    }
}