using System.Globalization;
using Destruction.Common;
using UnityEditor;
using UnityEngine;

namespace Destruction.Tools
{
    internal class StickyZoneTool : IDestructionTool
    {
        private bool showScaleHandle;
        private GameObject activeObject;

        private Vector2 scrollPosition;

        #region IDestructionTool
        public string Name
        {
            get { return "Sticky Zones"; }
        }

        public void OnEnable(Object[] targets)
        {
        }

        public void OnSceneGUI(BaseDestructable target)
        {
            if (target == null) return;

            if (target.stickyZones == null)
            {
                target.stickyZones = new StickyZone[0];
            }

            for (int i = 0; i < target.stickyZones.Length; i++)
            {
                if (target.stickyZones[i] == null) continue;

                target.stickyZones[i].gameObject.name = "Sticky Zone - " + i;
                Handles.Label(target.stickyZones[i].transform.position, "   " + i.ToString(CultureInfo.InvariantCulture), EditorStyles.largeLabel);
            }
        
            if (activeObject == null) return;

            Event current = Event.current;
            switch (current.type)
            {
                case EventType.KeyDown:
                    if (current.keyCode == KeyCode.S)
                    {
                        showScaleHandle = true;
                    }
                    break;

                case EventType.KeyUp:
                    if (current.keyCode == KeyCode.S)
                    {
                        showScaleHandle = false;
                    }
                    break;
            }

            if (showScaleHandle)
            {
                activeObject.transform.localScale = Handles.ScaleHandle(activeObject.transform.localScale, activeObject.transform.position, Quaternion.identity, 1);
            }
            else
            {
                activeObject.transform.position = Handles.PositionHandle(activeObject.transform.position, Quaternion.identity);
            }
        }

        public void OnInspectorGUI(BaseDestructable[] targets)
        {
            if (targets == null || targets.Length == 0 || targets[0] == null) return;
            if (targets.Length > 1)
            {
                using (new GUIColor(new Color(1.0f, 0.3f, 0.3f)))
                {
                    GUILayout.Label("Cannot use this tool with more than one destructable object selected.", EditorStyles.miniBoldLabel);
                }
                return;
            }

            if (targets[0].stickyZones != null && targets[0].stickyZones.Length != 0)
            {
                using (HorizontalScroll scrollbar = new HorizontalScroll(scrollPosition, Mathf.Min(100, targets[0].stickyZones.Length * 20 + 10)))
                {
                    scrollPosition = scrollbar.Position;
                    using (new Vertical("box"))
                    {
                        foreach (StickyZone zone in targets[0].stickyZones)
                        {
                            using (new Horizontal("toolbarbutton"))
                            {
                                if (zone == null)
                                {
                                    ArrayUtility.Remove(ref targets[0].stickyZones, null);
                                    break;
                                }

                                EditorGUILayout.ObjectField(zone, typeof(StickyZone), false);

                                using (new GUIBackgroundColor(new Color(0.8f, 0.8f, 1.0f)))
                                {
                                    if (GUILayout.Button("Select", "toolbarbutton"))
                                    {
                                        activeObject = zone.gameObject;
                                    }
                                }

                                if (GUILayout.Button("Zero", "toolbarbutton"))
                                {
                                    zone.transform.position = targets[0].transform.position + Vector3.one;
                                    zone.transform.rotation = targets[0].transform.rotation;
                                    zone.transform.localScale = Vector3.one;
                                }

                                using (new GUIBackgroundColor(Color.red))
                                {
                                    if (GUILayout.Button("X", "toolbarbutton", GUILayout.Width(30)))
                                    {
                                        Object.DestroyImmediate(zone.gameObject);
                                        ArrayUtility.Remove(ref targets[0].stickyZones, zone);
                                        break;
                                    }
                                }
                            }
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
            }

            using (new Horizontal("toolbarbutton"))
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add New", "toolbarbutton"))
                {
                    var newZone = CreateStickyZone();
                    activeObject = newZone.gameObject;
                    ArrayUtility.Add(ref targets[0].stickyZones, newZone);
                }
            }
        }

        #endregion

        private StickyZone CreateStickyZone()
        {
            GameObject zone = new GameObject("Sticky Zone");

            if(SceneView.lastActiveSceneView != null)
            {
                Transform camera = SceneView.lastActiveSceneView.camera.transform;
                zone.transform.position = (camera.position + camera.forward * 2);
            }
            else
            {
                zone.transform.position = Vector3.one;
            }

            return zone.AddComponent<StickyZone>();
        }
    }
}