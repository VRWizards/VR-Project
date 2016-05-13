using Destruction.Common;
using UnityEditor;
using UnityEngine;

namespace Destruction.Tools
{
    internal class Options : IDestructionTool
    {
        private SerializedObject obj;
        private SerializedProperty useMyTag;
        private SerializedProperty useMyLayer;

        #region IDestructionTool
        public string Name
        {
            get { return "Options"; }
        }

        public void OnEnable(Object[] targets)
        {
            obj = new SerializedObject(targets);
            useMyTag = obj.FindProperty("useMyTag");
            useMyLayer = obj.FindProperty("useMyLayer");
        }

        public void OnSceneGUI(BaseDestructable target)
        {
        }

        public void OnInspectorGUI(BaseDestructable[] targets)
        {
            obj.Update();
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(useMyTag, new GUIContent("Use Tag Of This Object"));
            EditorGUILayout.PropertyField(useMyLayer, new GUIContent("Use Layer Of This Object"));

            EditorGUILayout.Separator();

            LinkObjectsOnDestroy linkObjects = targets[0].GetComponent<LinkObjectsOnDestroy>();
            bool linkedToggle = EditorGUILayout.Toggle(new GUIContent("Link Objects On Destroy"), linkObjects != null);
            if (linkedToggle && linkObjects == null)
            {
                foreach (var baseDestructable in targets)
                {
                    baseDestructable.gameObject.AddComponent<LinkObjectsOnDestroy>();
                }
            }
            else if (!linkedToggle && linkObjects != null)
            {
                Object.DestroyImmediate(linkObjects);
            }
            obj.ApplyModifiedProperties();
        }

        #endregion
    }
}