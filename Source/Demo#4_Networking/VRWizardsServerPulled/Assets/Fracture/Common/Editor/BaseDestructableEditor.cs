using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

using Destruction.Tools;
using Destruction.Common;

public class BaseDestructableEditor : Editor
{
    private enum Tools
    {
        StickyZone = 0,
        Joiner,
        Cleanup,
        Options
    }

    private Tools CurrentToolKey = Tools.StickyZone;

    private readonly Dictionary<Tools, IDestructionTool> tools = new Dictionary<Tools, IDestructionTool>
                                                                {
                                                                    { Tools.StickyZone, new StickyZoneTool() },
                                                                    { Tools.Joiner, new Joiner() },
                                                                    { Tools.Cleanup, new CleanupTool() },
                                                                    { Tools.Options, new Options() }
                                                                };
    private IDestructionTool CurrentSelectedTool
    {
        get { return tools[CurrentToolKey]; }
    }

    protected virtual void OnEnable()
    {
        CurrentSelectedTool.OnEnable(targets);
    }

    public override void OnInspectorGUI()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        GUILayout.Space(10);

        using (new Vertical("box"))
        {
            using (new Horizontal("TE Toolbar"))
            {
                foreach (KeyValuePair<Tools, IDestructionTool> tool in tools)
                {
                    using (new GUIBackgroundColor(tool.Key == CurrentToolKey ? Color.grey : Color.white))
                    {
                        if (GUILayout.Button(tool.Value.Name, "toolbarbutton"))
                        {
                            if (CurrentToolKey != tool.Key)
                            {
                                tools[tool.Key].OnEnable(targets);
                            }

                            CurrentToolKey = tool.Key;

                            break;
                        }
                    }
                }

                GUILayout.FlexibleSpace();
            }

            CurrentSelectedTool.OnInspectorGUI(targets.Cast<BaseDestructable>().ToArray());
        }
    }

    protected virtual void OnSceneGUI()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        CurrentSelectedTool.OnSceneGUI(target as BaseDestructable);
    }
}