using Destruction.Tools;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Trigger))]
public class TriggerEditor : Editor
{
    private SerializedProperty impactForceThreshold;
    private SerializedProperty startingHealth;
    private SerializedProperty toughness;

    private SerializedProperty onlyTriggerWithTag;
    private SerializedProperty triggerTag;

    private SerializedProperty currentHealth;

    private bool showCollisionSettings;
    private bool showHealthSettings;

    #region Unity API
    private void OnEnable()
    {
        impactForceThreshold    = serializedObject.FindProperty("impactForceThreshold");
        startingHealth          = serializedObject.FindProperty("startingHealth");
        toughness               = serializedObject.FindProperty("toughness");

        onlyTriggerWithTag      = serializedObject.FindProperty("onlyTriggerWithTag");
        triggerTag              = serializedObject.FindProperty("triggerTag");

        currentHealth           = serializedObject.FindProperty("currentHealth");
    }

    public override void OnInspectorGUI()
    {
        Trigger triggerObject = target as Trigger;

        if (triggerObject == null)
        {
            using (new GUIColor(new Color(1.0f, 0.3f, 0.3f)))
            {
                GUILayout.Label("There is an error with the inspector, can't load target reference.", EditorStyles.miniBoldLabel);
            }
            return;
        }

        serializedObject.Update();

        TriggerTypes(triggerObject);

        CollisionSettings(triggerObject);

        HealthSettings(triggerObject);

        ShowInformation();

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    private void TriggerTypes(Trigger triggerObject)
    {
        using (new Vertical("box"))
        {
            GUILayout.Label("Initiates the fracture of any object linked to this trigger based on the trigger types set here.", EditorStyles.miniBoldLabel);

            GUILayout.Space(3f);

            triggerObject.triggerType = (Trigger.TriggerType) EditorGUILayout.EnumMaskField("Trigger Type", triggerObject.triggerType);

            foreach (var t in targets)
            {
                if (t == null) continue;

                (t as Trigger).triggerType = triggerObject.triggerType;
            }

            GUILayout.Space(3f);
        }
    }

    private void ShowInformation()
    {
        using (new Vertical("box"))
        {
            GUILayout.Space(3f);

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                currentHealth.floatValue = startingHealth.floatValue;
            }

            GUILayout.Label("Info : ", EditorStyles.miniBoldLabel);
            Rect r = EditorGUILayout.BeginVertical();
            EditorGUI.ProgressBar(r, currentHealth.floatValue,
                                  string.Format("Current Health : {0}%", (currentHealth.floatValue*100).ToString("f1")));
            GUILayout.Space(16);
            EditorGUILayout.EndVertical();
            GUILayout.Space(3f);
        }
    }

    private void HealthSettings(Trigger triggerObject)
    {
        using (new Vertical("box"))
        {
            GUILayout.Space(3f);
            showHealthSettings = EditorGUILayout.Foldout(showHealthSettings, "Health Settings");

            GUILayout.Space(5f);

            if (showHealthSettings)
            {
                if ((triggerObject.triggerType & Trigger.TriggerType.Explosion) == 0 &&
                    (triggerObject.triggerType & Trigger.TriggerType.Bullet) == 0)
                {
                    using (new GUIColor(new Color(1.0f, 0.3f, 0.3f)))
                    {
                        GUILayout.Label("To use the health settings, you must have enabled the bullet or explosion trigger type(s).", EditorStyles.miniBoldLabel);
                    }
                }
                else
                {
                    EditorGUILayout.Slider(startingHealth, 0f, 1f, new GUIContent("Initial Health", "This is the initial health of the object."));
                    EditorGUILayout.Slider(toughness, 0f, 1f, new GUIContent("Toughness", "This is how resistant an object is to taking damage."));
                }
            }
            GUILayout.Space(3f);
        }
    }

    private void CollisionSettings(Trigger triggerObject)
    {
        using (new Vertical("box"))
        {
            GUILayout.Space(3f);
            showCollisionSettings = EditorGUILayout.Foldout(showCollisionSettings, "Collision Settings");

            GUILayout.Space(5f);

            if (showCollisionSettings)
            {
                if ((triggerObject.triggerType & Trigger.TriggerType.Collision) != 0)
                {
                    EditorGUILayout.PropertyField(impactForceThreshold, new GUIContent("Impact Force Threshold", "This is the minimum force required from a collision to trigger any fracture components this object is linked to.  Setting this to 0 would effectively make any collision trigger the fracture."));
                
                    EditorGUILayout.Separator();

                    EditorGUILayout.PropertyField(onlyTriggerWithTag, new GUIContent("Only Trigger With Tag", "If this checkbox is ticked, this object will only be triggered if the tag of the colliding object is the same as below."));
                    if(onlyTriggerWithTag.boolValue)
                    {
                        triggerTag.stringValue = EditorGUILayout.TagField(new GUIContent("Trigger Tag", "The desired tag to use for masking collisions by tag."), triggerTag.stringValue);                        
                    }
                }
                else
                {
                    using (new GUIColor(new Color(1.0f, 0.3f, 0.3f)))
                    {
                        GUILayout.Label("To use the collision settings, you must have enabled the collision trigger type.", EditorStyles.miniBoldLabel);
                    }
                }
            }
            GUILayout.Space(3f);
        }
    }
}
