using Destruction.Tools;
using UnityEngine;

namespace Destruction.Dynamic
{
    [AddComponentMenu("Destruction/Box Fracture")]
    [RequireComponent(typeof(Trigger))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BoxFracture : BaseFracture
    {
        private Vector3 maxBounds;
        private Vector3 minBounds;

        private readonly static Vector4[] initPlanes = new Vector4[] { Vector3.right, Vector3.up, Vector3.forward, Vector3.left, Vector3.down, Vector3.back };
        
        /// <summary>
        /// Initializes the planes for fracture.
        /// </summary>
        /// <param name="point">Current calculation point.</param>
        protected override void CreateMeshPlanes(Vector3 point)
        {
            Vector3 relativeMaxBounds = point - maxBounds;
            Vector3 relativeMinBounds = minBounds - point;

            initPlanes[0].w = relativeMaxBounds.x;
            initPlanes[1].w = relativeMaxBounds.y;
            initPlanes[2].w = relativeMaxBounds.z;

            initPlanes[3].w = relativeMinBounds.x;
            initPlanes[4].w = relativeMinBounds.y;
            initPlanes[5].w = relativeMinBounds.z;

            planes.AddRange(initPlanes);
        }

        protected override void InitializeDestruction()
        {
            base.InitializeDestruction();

            maxBounds = transform.lossyScale * 0.5f;
            minBounds = -maxBounds;
        }
    }
}