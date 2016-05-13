using System.Collections.Generic;
using Destruction.Tools;
using UnityEngine;

namespace Destruction.Dynamic
{
    [AddComponentMenu("Destruction/Convex Fracture")]
    [RequireComponent(typeof(Trigger))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ConvexFracture : BaseFracture
    {
        private Vector4[] initPlanes;
        private Mesh mesh;

        private void Awake()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;

            HashSet<Vector4> meshPlanes = new HashSet<Vector4>();
            for(int i=0 ; i<mesh.triangles.Length ; i+=3)
            {
                Plane face = new Plane(GetVertex(i), GetVertex(i + 1), GetVertex(i + 2));
                Vector4 planeEq = new Vector4(face.normal.x, face.normal.y, face.normal.z, face.distance);

                if (!meshPlanes.Contains(planeEq))
                {
                    meshPlanes.Add(planeEq);
                }
            }

            initPlanes = new Vector4[meshPlanes.Count];
            meshPlanes.CopyTo(initPlanes);
        }

        private Vector3 GetVertex(int i)
        {
            return Vector3.Scale(transform.lossyScale, mesh.vertices[mesh.triangles[i]]);
        }

        /// <summary>
        /// Initializes the planes for fracture to the mesh planes generated from the convex mesh.
        /// </summary>
        /// <param name="point">Current calculation point.</param>
        protected override void CreateMeshPlanes(Vector3 point)
        {
            for (int index = 0; index < initPlanes.Length; index++)
            {
                Vector4 plane = initPlanes[index];
                plane.w += Vector3.Dot(plane, point);
                planes.Add(plane);
            }
        }

    }
}