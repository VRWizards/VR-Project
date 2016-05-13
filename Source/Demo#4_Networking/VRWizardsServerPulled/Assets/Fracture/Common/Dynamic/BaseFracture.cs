using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Destruction.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Destruction.Dynamic
{
    public abstract class BaseFracture : BaseDestructable
    {
        #region Editor Fields
        [SerializeField] 
        protected bool immediate = true;
        [SerializeField]
        protected RangedInt shards = new RangedInt(3, 9);
        [SerializeField]
        protected int shardsPerFrame = 2;

        #endregion

        /// <summary> Contains the points used for the voronoi calculations, sorted in order of distance from the current calculation point. </summary>
        protected Vector3[] sortedVoronoiPoints;

        protected Vector3[] newVertices;
        protected Vector3[] newNormals;
        protected Vector2[] newUVs;
        protected int[] newTriangles;

        protected Dictionary<int, List<Vector3>> vert = new Dictionary<int, List<Vector3>>(50);
        protected readonly List<Vector4> planes = new List<Vector4>(100);

        private void Update()
        {
            if (!Complete) return;

            for(int i=0 ; i<finalizedBrokenPieces.Count ; i++)
            {
                finalizedBrokenPieces[i].transform.position = transform.position;
                finalizedBrokenPieces[i].transform.rotation = transform.rotation;
            }
        }

        /// <summary>
        /// Initializes the planes for fracturing.
        /// </summary>
        /// <param name="point">Current calculation point.</param>
        protected abstract void CreateMeshPlanes(Vector3 point);

        protected override void BeginFracture(Vector3 point, float size)
        {
            if (Complete) return;
            Complete = true;

            //Debug.Log("Destruction Triggered : " + name);

            LastImpactPoint = point;

            // Create an array of random points that pivot around the main fracture point.
            Vector3[] points = new Vector3[shards.Random()];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = Vector3.Scale(transform.lossyScale, transform.InverseTransformPoint(point)) + Random.insideUnitSphere * size;
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();

            InitializeDestruction();

            sortedVoronoiPoints = new Vector3[points.Length];
            points.CopyTo(sortedVoronoiPoints, 0);

            // Start the worker methods here.
            if (immediate)
            {
                ImmediateFracture(points);
            }
            else
            {
                StartCoroutine(SpreadFracture(points));
            }

            timer.Stop();
            //Debug.Log("Time taken to compute, " + timer.ElapsedMilliseconds + " shards : " + points.Length);
        }

        /// <summary>
        /// The entire fracture process over the course of a single frame.
        /// </summary>
        /// <param name="points">Voronoi points.</param>
        protected virtual void ImmediateFracture(Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                CalculateShard(points[i]);
            }

            FinalizeDestruction();
        }

        /// <summary>
        /// The entire fracture process, split over multiple frames.
        /// </summary>
        /// <param name="points">Voronoi points.</param>
        /// <returns>Coroutine.</returns>
        protected virtual IEnumerator SpreadFracture(Vector3[] points)
        {
            int currentShard = 0;
            int remainingShards = points.Length;

            while (remainingShards > 0)
            {
                for (int i = 0; i < Mathf.Min(shardsPerFrame, remainingShards); i++)
                {
                    --remainingShards;

                    CalculateShard(points[currentShard]);

                    ++currentShard;
                }

                yield return null;
            }

            FinalizeDestruction();
        }
        
        #region Aux Methods

        /// <summary>
        /// Calculates the planes and vertices that make up one complete shard.
        /// </summary>
        /// <param name="point">Current voronoi point.</param>
        private void CalculateShard(Vector3 point)
        {
            planes.Clear();

            int vertexCount = CalculateVerts(point);

            if (vertexCount <= 3) return;

            newVertices = new Vector3[vertexCount];
            newNormals = new Vector3[vertexCount];
            newUVs = new Vector2[vertexCount];
            newTriangles = new int[vertexCount * 3];

            // TODO : Optimize this dictionary out.
            int indexer = 0, tIndexer = 0;
            foreach (KeyValuePair<int, List<Vector3>> p in vert)
            {
                if (p.Value.Count < 3) continue;

                Vector3 normal = planes[p.Key];
                p.Value.SortAntiClockwiseOnPlane(normal);

                CreateNewVertex(p.Value[0] + point, normal, indexer);

                for (int v = 1; v < p.Value.Count - 1; v++)
                {
                    CreateNewVertex(p.Value[v] + point, normal, indexer + v);

                    newTriangles[tIndexer++] = indexer;
                    newTriangles[tIndexer++] = indexer + v;
                    newTriangles[tIndexer++] = indexer + v + 1;
                }

                CreateNewVertex(p.Value[p.Value.Count - 1] + point, normal, indexer + p.Value.Count - 1);

                indexer += p.Value.Count;

                p.Value.Clear();
            }

            CreateShardMesh(point);
        }

        private void CreateNewVertex(Vector3 vertex, Vector3 normal, int index)
        {
            newNormals[index] = normal;
            newVertices[index] = vertex;
        }

        /// <summary>
        /// Calculates the corner vertices of the vornoi convex hull.
        /// </summary>
        /// <param name="point">Current calculation point.</param>
        protected virtual int CalculateVerts(Vector3 point)
        {
            Array.Sort(sortedVoronoiPoints, (p1, p2) => (p1 - point).sqrMagnitude.CompareTo((p2 - point).sqrMagnitude));
            
            for (int j = 1; j < sortedVoronoiPoints.Length; j++)
            {
                Vector3 normal = sortedVoronoiPoints[j] - point;
                float nlength2 = normal.sqrMagnitude;

                Vector4 plane = normal.normalized;
                plane.w = -Mathf.Sqrt(nlength2) / 2.0f;
                planes.Add(plane);
            }

            // At this point, all planes created from the initial points, should form voronoi diagram
            CreateMeshPlanes(point);

            return GetVerticesInPlane();
        }

        /// <summary>
        /// Initializes this destruction object, called just before destruction has started.
        /// 
        /// Calculates the bounds for this box and disables collisions whil destruction is on-going.
        /// </summary>
        protected virtual void InitializeDestruction()
        {
            SendMessage("OnPreDestruction", this, SendMessageOptions.DontRequireReceiver);

            if (immediate)
            {
                GetComponent<Collider>().enabled = false;
                if (GetComponent<Rigidbody>() != null)
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }

        /// <summary>
        /// Finalizes this destruction object.  Called after destruction has finished.
        /// 
        /// Destroys this object and enables the rigidbodies of the newly created shards.
        /// </summary>
        protected void FinalizeDestruction()
        {
            foreach (var fracturedMeshPiece in finalizedBrokenPieces)
            {
                fracturedMeshPiece.GetComponent<Rigidbody>().isKinematic = false;
                if (!immediate && GetComponent<Rigidbody>() != null && !GetComponent<Rigidbody>().isKinematic)
                {
                    fracturedMeshPiece.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
                    fracturedMeshPiece.GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().angularVelocity;
                }
            }

            SendMessage("OnPostDestruction", finalizedBrokenPieces, SendMessageOptions.DontRequireReceiver);

            transform.DetachChildren();
            Destroy(gameObject);
        }

        /// <summary>
        /// Compares all the planes and calculates intersection points where 3 planes meet, these points will go on
        /// to be the corner points of our voronoi cells.
        /// </summary>
        protected virtual int GetVerticesInPlane()
        {
            const float errorEpsilon = 0.001f;
            int potentialVertexCount = 0;

            vert.Clear();

            // Calculate intersection between 3 planes.
            for (int i = 0; i < planes.Count; i++)
            {
                for (int j = i + 1; j < planes.Count; j++)
                {
                    Vector3 n1n2 = Vector3.Cross(planes[i], planes[j]);

                    if (n1n2.sqrMagnitude <= errorEpsilon) continue;

                    for (int k = j + 1; k < planes.Count; k++)
                    {
                        Vector3 n2n3 = Vector3.Cross(planes[j], planes[k]);
                        Vector3 n3n1 = Vector3.Cross(planes[k], planes[i]);

                        if (n2n3.sqrMagnitude <= errorEpsilon || n3n1.sqrMagnitude <= errorEpsilon) continue;

                        float dot = Vector3.Dot(planes[i], n2n3);

                        if (Mathf.Abs(dot) <= errorEpsilon) continue;

                        Vector3 potentialVertex = (n2n3 * planes[i].w + n3n1 * planes[j].w + n1n2 * planes[k].w) * (-1f / dot);

                        int l;
                        for (l = 0; l < planes.Count; l++)
                        {
                            Vector4 NP = planes[l];

                            if (Vector3.Dot(NP, potentialVertex) + NP.w > errorEpsilon) break;
                        }

                        if (l != planes.Count) continue;

                        // Increment by amount of new vertices per plane.
                        potentialVertexCount+=3;

                        AddVertex(potentialVertex, i);
                        AddVertex(potentialVertex, j);
                        AddVertex(potentialVertex, k);
                    }
                }
            }

            return potentialVertexCount;
        }

        protected virtual void AddVertex(Vector3 potentialVertex, int planeIndex)
        {
            if (!vert.ContainsKey(planeIndex))
            {
                vert.Add(planeIndex, new List<Vector3>(50));
            }

            vert[planeIndex].Add(potentialVertex);                
        }

        /// <summary>
        /// Creates a shard using the calculated vertices and triangles.
        /// </summary>
        /// <param name="point">Current calculation point.  Used to offset the created shard.</param>
        protected Shard CreateShardMesh(Vector3 point, float mass = -1)
        {
            Shard shard = Shard.CreateShard(gameObject, newVertices, newNormals, newTriangles, newUVs);

            if (shard == null) return null;

            shard.transform.position = transform.position;// +transform.TransformDirection(point);
            shard.transform.rotation = transform.rotation;

            shard.GetComponent<Renderer>().sharedMaterial = new Material(GetComponent<Renderer>().material);
            shard.GetComponent<Rigidbody>().isKinematic = true;

            if (GetComponent<Rigidbody>() != null)
            {
                Vector3 shardSize = shard.GetComponent<Renderer>().bounds.size;
                float shardVolume = shardSize.x * shardSize.y * shardSize.z;

                Vector3 parentSize = GetComponent<Renderer>().bounds.size;
                float parentVolume = parentSize.x * parentSize.y * parentSize.z;

                shard.GetComponent<Rigidbody>().mass = GetComponent<Rigidbody>().mass * (shardVolume / parentVolume);
            }

            if(!immediate)
            {
                if (gameObject.activeInHierarchy && GetComponent<Collider>().enabled && shard.gameObject.activeInHierarchy && shard.GetComponent<Collider>().enabled)
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), shard.GetComponent<Collider>());
                }
            }

            finalizedBrokenPieces.Add(shard.GetComponent<Collider>());

            return shard;
        }

        #endregion
    }
}