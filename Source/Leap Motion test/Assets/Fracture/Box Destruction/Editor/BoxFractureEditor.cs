using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Destruction.Dynamic.BoxFracture)), CanEditMultipleObjects]
public class BoxFractureEditor : BaseDestructableEditor
{
    private SerializedProperty immediate;
    private SerializedProperty shards;
    private SerializedProperty shardsPerFrame;

    protected override void OnEnable()
    {
        base.OnEnable();

        immediate = serializedObject.FindProperty("immediate");
        shards = serializedObject.FindProperty("shards");
        shardsPerFrame = serializedObject.FindProperty("shardsPerFrame");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        using(new Vertical("box"))
        {
            GUILayout.Label("Uses vornoi partitioning to fracture a box into smaller, convex shards.", EditorStyles.miniBoldLabel);

            GUILayout.Space(3f);

            EditorGUILayout.PropertyField(immediate, new GUIContent("Fracture Immediately", "Having this enabled will make this object fracture over the course of a single frame."));
            
            if (!immediate.boolValue)
            {
                GUILayout.Space(3f);
                EditorGUILayout.PropertyField(shardsPerFrame, new GUIContent("Shards To Compute Per Frame", "If you aren't using the immediate option, you can choose how many shards you wish to compute per frame."));
            }

            GUILayout.Space(5f);

            EditorGUILayout.PropertyField(shards, new GUIContent("Amount of Shards", "Min/Max range, a random number inbetween these values will determine the amount of shards this object will fracture into."), true);

            GUILayout.Space(3f);
        }

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();
    }
}
