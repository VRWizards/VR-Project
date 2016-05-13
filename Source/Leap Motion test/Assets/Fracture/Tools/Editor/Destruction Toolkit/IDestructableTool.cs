using Destruction.Common;
using UnityEngine;

namespace Destruction.Tools
{
    internal interface IDestructionTool
    {
        string Name { get; }
        void OnEnable(Object[] targets);
        void OnInspectorGUI(BaseDestructable[] targets);
        void OnSceneGUI(BaseDestructable target);
    }
}