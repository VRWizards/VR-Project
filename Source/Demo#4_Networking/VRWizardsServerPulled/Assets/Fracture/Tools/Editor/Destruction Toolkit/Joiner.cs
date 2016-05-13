using System;
using System.Linq;
using Destruction.Common;
using Destruction.Utilities.FloodFiller;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Destruction.Tools
{
    internal class Joiner : IDestructionTool
    {
        private enum Quality
        {
            Low,
            Medium,
            High
        }

        private Quality setting;

        private static Collider currentCollider;
        private bool cancellLastAction;

        private float GetErrorRadiusFromQuality()
        {
            switch(setting)
            {
                case Quality.Low:
                    return 0.2f;
                case Quality.Medium:
                    return 0.1f;
                case Quality.High:
                    return 0.025f;
            }

            return 1;
        }

        #region IDestructionTool
        public string Name
        {
            get { return "Joiner"; }
        }

        public void OnEnable(Object[] targets)
        {
        }

        public void OnSceneGUI(BaseDestructable target)
        {
        }

        public void OnInspectorGUI(BaseDestructable[] targets)
        {
            if (targets == null) return;

            GUILayout.Label("This tool can be used to create a structural integrity simulation of sorts.", EditorStyles.miniBoldLabel);
            GUILayout.Label("The joints used are a first pass implementation, \nand just work with binary connectivity rather than \nstrength of bonds.", EditorStyles.miniBoldLabel);

            GUILayout.Space(5);

            setting = (Quality)EditorGUILayout.EnumPopup("Quality Setting", setting);

            GUILayout.Space(5);

            if (GUILayout.Button("Select Adjacent Colliders", GUILayout.Width(300)))
            {
                foreach(BaseDestructable t in targets)
                {
                    if (cancellLastAction) break;
                    currentCollider = t.GetComponent<Collider>();
                    new FloodFiller3(Cancelled, SatisfiedContraint, SelectOnComplete, t.GetComponent<Collider>().bounds.center, t.GetComponent<Collider>().bounds.min, t.GetComponent<Collider>().bounds.size * 2, GetErrorRadiusFromQuality());
                }

                cancellLastAction = false;
            }

            if (GUILayout.Button("Connect Adjacent Colliders", GUILayout.Width(300)))
            {
                RemoveAllJoints(targets);
                foreach (BaseDestructable t in targets)
                {
                    if (cancellLastAction) break;

                    currentCollider = t.GetComponent<Collider>();
                    new FloodFiller3(Cancelled, SatisfiedContraint, OnComplete, t.GetComponent<Collider>().bounds.center, t.GetComponent<Collider>().bounds.min, t.GetComponent<Collider>().bounds.size * 2, GetErrorRadiusFromQuality());
                }

                cancellLastAction = false;
            }

            if (GUILayout.Button("Remove Joints", GUILayout.Width(300)))
            {
                RemoveAllJoints(targets);
            }
        }

        private static void RemoveAllJoints(BaseDestructable[] targets)
        {
            foreach (BaseDestructable t in targets)
            {
                if (t.GetComponent<RigidJoint>() != null)
                {
                    foreach (var joint in t.GetComponents<RigidJoint>())
                    {
                        Object.DestroyImmediate(joint);
                    }
                }
            }
        }

        #endregion

        private void Cancelled()
        {
            cancellLastAction = true;
        }

        private bool SatisfiedContraint(Collider[] colliders)
        {
            return colliders.All(c => c != currentCollider);
        }

        private void SelectOnComplete(Collider[] collidersFound)
        {
            Selection.objects = collidersFound
                .Where(c => c != currentCollider)
                .Select(c => c.gameObject)
                .Cast<Object>()
                .ToArray();
        }

        private void OnComplete(Collider[] collidersFound)
        {
            foreach(Collider c in collidersFound)
            {
                if (c == currentCollider) continue;
                //if (c.GetComponent<RigidJoint>().IsConnectedToJoint(currentCollider.attachedRigidbody)) continue;
                
                if (c.GetComponent<BaseDestructable>() == null)
                {
                    (currentCollider.GetComponent<RigidJoint>() ?? currentCollider.gameObject.AddComponent<RigidJoint>()).AttachToObject(null);
                }
                else
                {
                    (currentCollider.GetComponent<RigidJoint>() ?? currentCollider.gameObject.AddComponent<RigidJoint>()).AttachToObject(c.attachedRigidbody);
                }
            }
        }
    }
}
