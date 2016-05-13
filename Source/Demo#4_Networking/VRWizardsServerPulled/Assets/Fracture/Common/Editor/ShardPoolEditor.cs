using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Destruction.Dynamic.ShardPool))]
public class ShardPoolEditor : Editor
{
    private SerializedProperty maxPoolSize;

    private void OnEnable()
    {
        maxPoolSize = serializedObject.FindProperty("maxPoolSize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.BeginVertical("box");
        {
            GUILayout.Label("Creates and holds all the shards that can be used by the destruction system.", EditorStyles.miniBoldLabel);
            GUILayout.Label("Status : " + ((target as Destruction.Dynamic.ShardPool).IsPopulated ? "Populated" : "Empty"), EditorStyles.boldLabel);

            GUILayout.Space(5f);

            EditorGUILayout.PropertyField(maxPoolSize, new GUIContent("Max Pool Size", "The amount of shards this pool can handle."));

            GUILayout.Space(3f);

            if (GUILayout.Button("Populate", GUILayout.Width(200)))
            {
                (target as Destruction.Dynamic.ShardPool).PopulatePool();
            }

            GUILayout.Space(3f);
        }
        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    [MenuItem("GameObject/Create Other/Create Shard Pool In Scene...")]
    private static void CreateShardPool()
    {
        Destruction.Dynamic.ShardPool.CheckPool();
    }
}
