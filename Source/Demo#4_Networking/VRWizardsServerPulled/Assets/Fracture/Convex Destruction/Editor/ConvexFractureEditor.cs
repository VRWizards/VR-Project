using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Destruction.Dynamic.ConvexFracture))]
public class ConvexFractureEditor : BaseDestructableEditor
{
    private SerializedProperty immediate;
    private SerializedProperty shards;
    private SerializedProperty shardsPerFrame;
    private SerializedProperty useScheduler;

    protected override void OnEnable()
    {
        base.OnEnable();

        immediate = serializedObject.FindProperty("immediate");
        shards = serializedObject.FindProperty("shards");
        shardsPerFrame = serializedObject.FindProperty("shardsPerFrame");
        useScheduler = serializedObject.FindProperty("useScheduler");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        using (new Vertical("box"))
        {
            GUILayout.Label("Uses vornoi partitioning to fracture a convex object into smaller, convex shards.", EditorStyles.miniBoldLabel);

            GUILayout.Space(3f);

            EditorGUILayout.PropertyField(useScheduler, new GUIContent("Use Scheduler?", "Having this enabled will make this object fracture over the course of a single frame."));
            GUILayout.Space(1f);

            EditorGUILayout.PropertyField(immediate, new GUIContent("Fracture Immediately?", "Destroy this object from within the scheduler, this will ensure only a set amount of objects will be destroyed per frame."));

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
