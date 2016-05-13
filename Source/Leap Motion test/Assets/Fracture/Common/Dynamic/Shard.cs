//#define DYNAMIC
//#define OPTIMIZE
#define TANGENTS
//#define FIX

using Destruction.Utilities;
using UnityEngine;

namespace Destruction.Dynamic
{
    public class Shard : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        public Mesh Mesh
        {
            get { return meshFilter != null ? meshFilter.sharedMesh : GetComponent<MeshFilter>().sharedMesh; }
        }

        private void Awake()
        {
            if (GetComponent<Renderer>() == null) gameObject.AddComponent<MeshRenderer>();
            if (GetComponent<Rigidbody>() == null) gameObject.AddComponent<Rigidbody>();

            meshFilter = GetComponent<MeshFilter>() == null ? gameObject.AddComponent<MeshFilter>() : GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>() == null ? gameObject.AddComponent<MeshCollider>() : GetComponent<MeshCollider>();
            
            meshCollider.convex = true;

            meshFilter.sharedMesh = new Mesh();

#if DYNAMIC
            meshFilter.mesh.MarkDynamic();
#endif

            gameObject.SetActive(false);
        }

        internal void FinalizeShard()
        {
            transform.parent = null;
            gameObject.SetActive(true);
        }

        internal static Shard CreateShard(GameObject parent, Vector3[] newVertices, Vector3[] newNormals, int[] newTriangles, Vector2[] newUVs)
        {
            Shard shard = ShardPool.NextShard;

            if (shard == null)
            {
                Debug.LogError("Not enough shards left in pool.");
                return null;
            }

            shard.name = "Fractured shard";
            shard.Mesh.vertices = newVertices;
            shard.Mesh.normals = newNormals;
            shard.Mesh.uv = newUVs;
            shard.Mesh.triangles = newTriangles;

#if OPTIMIZE
            shard.Mesh.Optimize();
#endif

#if TANGENTS
            shard.Mesh.tangents = MeshUtility.RecalculateTangents(newVertices, newUVs, newNormals, newTriangles);
#endif

            shard.meshCollider.sharedMesh = shard.Mesh;

            shard.FinalizeShard();

            shard.GetComponent<Renderer>().material = parent.GetComponent<Renderer>().material;

            if (parent.GetComponent<Rigidbody>())
            {
                shard.GetComponent<Rigidbody>().mass = parent.GetComponent<Rigidbody>().mass;
                shard.GetComponent<Rigidbody>().velocity = parent.GetComponent<Rigidbody>().velocity;
            }
            
            return shard;
        }
    }
}