using System;
using UnityEngine;
using System.Collections.Generic;

namespace Destruction.Utilities
{
    public class Face
    {
        public static Vector3[] points;

        public int i0, i1, i2;
        public float a, b, c, d;

        public Vector3 Centroid
        {
            get
            {
                Vector3 p0 = points[i0];
                Vector3 p1 = points[i1];
                Vector3 p2 = points[i2];

                return new Vector3(p0.x + p1.x + p2.x, p0.y + p1.y + p2.y, p0.z + p1.z + p2.z) / 3f;
            }
        }

        public Face(int i0, int i1, int i2)
        {
            this.i0 = i0;
            this.i1 = i1;
            this.i2 = i2;

            ComputePlane();
        }

        private void ComputePlane()
        {
            Vector3 v1 = points[i0];
            Vector3 v2 = points[i1];
            Vector3 v3 = points[i2];

            a = v1.y * (v2.z - v3.z) + v2.y * (v3.z - v1.z) + v3.y * (v1.z - v2.z);
            b = v1.z * (v2.x - v3.x) + v2.z * (v3.x - v1.x) + v3.z * (v1.x - v2.x);
            c = v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y);
            d = -(v1.x * (v2.y * v3.z - v3.y * v2.z) + v2.x * (v3.y * v1.z - v1.y * v3.z) + v3.x * (v1.y * v2.z - v2.y * v1.z));
        }

        public bool IsVisible(Vector3 p)
        {
            return (a * p.x + b * p.y + c * p.z + d) >= 0;
        }

        public void Flip()
        {
            int t = i0;
            i0 = i1;
            i1 = t;
            ComputePlane();
        }
    }

    public static class MeshUtility
    {
        private static List<Face> validFaces;
        private static List<Face> visibleFaces;
        private static List<Face> tmpFaces;

        public static Vector4[] RecalculateTangents(Vector3[] vertices, Vector2[] uv, Vector3[] normals, int[] triangles)
        {
            //variable definitions
            int triangleCount = triangles.Length;
            int vertexCount = vertices.Length;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];

            Vector4[] tangents = new Vector4[vertexCount];

            for (long a = 0; a < triangleCount; a += 3)
            {
                long i1 = triangles[a + 0];
                long i2 = triangles[a + 1];
                long i3 = triangles[a + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = uv[i1];
                Vector2 w2 = uv[i2];
                Vector2 w3 = uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float div = s1 * t2 - s2 * t1;
                float r = Math.Abs(div - 0.0f) < Mathf.Epsilon ? 0.0f : 1.0f / div;

                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (long a = 0; a < vertexCount; ++a)
            {
                Vector3 n = normals[a];
                Vector3 t = tan1[a];

                Vector3.OrthoNormalize(ref n, ref t);
                tangents[a].x = t.x;
                tangents[a].y = t.y;
                tangents[a].z = t.z;

                tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }

            return tangents;
        }

        public static void RecalculateTangents(this Mesh mesh)
        {
            //speed up math by referencing the mesh arrays
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = mesh.uv;
            Vector3[] normals = mesh.normals;

            mesh.tangents = RecalculateTangents(vertices, uv, normals, triangles);
        }

        /// <summary>
        /// Pass in a cloud of points and an array of triangle indices shall be returned.
        /// </summary>
        /// <param name="points">Points to create hull from.</param>
        /// <returns>Triangle index array of hull.</returns>
        public static int[] ComputeHull(Vector3[] points)
        {
            if (points.Length < 3) return null;

            //local copy of the point set
            Face.points = points;

            //calculates the first convex tetrahedron

            //creates a face with the first 3 vertices
            Face faceA = new Face(0, 1, 2);

            //this is the center of the tetrahedron, all face should point outwards:
            //they should not be visible to the centroid
            Vector3 v = Centroid(points, 3, faceA);

            if (faceA.IsVisible(v))
            {
                faceA.Flip();
            }

            Face face0 = new Face(3, faceA.i0, faceA.i1);
            if (face0.IsVisible(v))
            {
                face0.Flip();
            }

            Face face1 = new Face(3, faceA.i1, faceA.i2);
            if (face1.IsVisible(v))
            {
                face1.Flip();
            }

            Face face2 = new Face(3, faceA.i2, faceA.i0);
            if (face2.IsVisible(v))
            {
                face2.Flip();
            }

            //store the tetrahedron faces in the valid faces list
            validFaces = new List<Face> { faceA, face0, face1, face2 };

            visibleFaces = new List<Face>(points.Length);
            tmpFaces = new List<Face>(points.Length);

            //so as we have a convex tetrahedron, we can skip the first 4 points
            for (int i = 4; i < points.Length; i++)
            {
                //for each avaiable vertices
                v = points[i];

                //checks the point's visibility from all faces
                visibleFaces.Clear();
                for (int j = 0; j < validFaces.Count; j++)
                {
                    if (validFaces[j].IsVisible(v))
                    {
                        visibleFaces.Add(validFaces[j]);
                    }
                }

                //the vertex is not visible : it is inside the convex hull, keep on
                if (visibleFaces.Count == 0) continue;

                //the vertex is outside the convex hull
                //delete all visible faces from the valid List
                for (int index = 0; index < visibleFaces.Count; index++)
                {
                    validFaces.Remove(visibleFaces[index]);
                }

                //special case : only one face is visible
                //it's ok to create 3 faces directly for they won't enclose any other point
                if (visibleFaces.Count == 1)
                {
                    faceA = visibleFaces[0];
                    validFaces.Add(new Face(i, faceA.i0, faceA.i1));
                    validFaces.Add(new Face(i, faceA.i1, faceA.i2));
                    validFaces.Add(new Face(i, faceA.i2, faceA.i0));
                    continue;
                }

                if (visibleFaces.Count > 2000)
                {
                    Debug.LogWarning("Visible faces is too big, " + visibleFaces.Count + " cancelling operation.");
                    return new int[0];
                }

                //creates all possible new faces from the visibleFaces
                tmpFaces.Clear();
                for (int k = 0; k < visibleFaces.Count; k++)
                {
                    tmpFaces.Add(new Face(i, visibleFaces[k].i0, visibleFaces[k].i1));
                    tmpFaces.Add(new Face(i, visibleFaces[k].i1, visibleFaces[k].i2));
                    tmpFaces.Add(new Face(i, visibleFaces[k].i2, visibleFaces[k].i0));
                }

                if (tmpFaces.Count > 8000)
                {
                    Debug.LogWarning("Temp faces is too big, " + tmpFaces.Count + " cancelling operation.");
                    return new int[0];
                }

                //Face other;
                for (int l = 0; l < tmpFaces.Count; l++)
                {
                    Face face = tmpFaces[l];
                    //search if there is a point in front of the face : 
                    //this means the face doesn't belong to the convex hull

                    for (int index1 = 0; index1 < tmpFaces.Count; index1++)
                    {
                        if (face != tmpFaces[index1])
                        {
                            if (face.IsVisible(tmpFaces[index1].Centroid))
                            {
                                face = null;
                                break;
                            }
                        }
                    }

                    //the face has no point in front of it
                    if (face != null)
                    {
                        validFaces.Add(face);
                    }
                }
            }


            var result = new int[validFaces.Count * 3];
            int vertIndex = 0;

            for (int i = 0; i < validFaces.Count; i++)
            {
                result[vertIndex++] = validFaces[i].i0;
                result[vertIndex++] = validFaces[i].i1;
                result[vertIndex++] = validFaces[i].i2;
            }

            return result;
        }


        private static Vector3 Centroid(Vector3[] points, int index, Face face)
        {
            Vector3 p = points[index];
            p += points[face.i0];
            p += points[face.i1];
            p += points[face.i2];
            return p / 4;
        }
    }
}