using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Destruction
{
    [Serializable]
    public class RangedInt
    {
        [SerializeField] private int min = 1;
        [SerializeField] private int max = 10;

        public int Min
        {
            get { return min; }
        }

        public int Max
        {
            get { return max; }
        }

        public int Size
        {
            get { return max - min; }
        }

        public RangedInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public int Random()
        {
            return UnityEngine.Random.Range(min, max);
        }

        public int Clamp(int input)
        {
            return Mathf.Clamp(input, min, max);
        }
    }

    public static class DestructionHelper
    {
        public static float MaxComponent(this Vector3 v)
        {
            return Mathf.Max(v.x, Mathf.Max(v.y, v.z));
        }

        public static float MinComponent(this Vector3 v)
        {
            return Mathf.Min(v.x, Mathf.Min(v.y, v.z));
        }

        public static void Resize<T>(this List<T> list, int size)
        {
            int currentSize = list.Count;

            if (size < currentSize)
            {
                list.RemoveRange(size, currentSize - size);
            }
        }

        public static void Toggle(ref bool value, string name)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(name + " : ");
            value = GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();
        }

        public static T[] FindObjectsOfType<T>()
        {
            T[] objects = Object.FindObjectsOfType(typeof(T)) as T[];
            return objects;
        }

        public static T[] GetInterfaces<T>(this GameObject gameObject)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");

            MonoBehaviour[] mObjs = gameObject.GetComponents<MonoBehaviour>();

            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }

        public static List<Vector3> DedupCollectionWithRandom(List<Vector3> input)
        {
            HashSet<Vector3> passedValues = new HashSet<Vector3>();

            foreach (Vector3 item in input)
            {
                if (!passedValues.Contains(item))
                {
                    passedValues.Add(item + Random.insideUnitSphere*0.0001f);
                }
            }

            return new List<Vector3>(passedValues);
        }

        public static List<Vector3> DedupCollectionWithRandom(List<Vector3> input, IEqualityComparer<Vector3> comparer)
        {
            HashSet<Vector3> passedValues = new HashSet<Vector3>(comparer);

            foreach (Vector3 item in input)
            {
                if (!passedValues.Contains(item))
                {
                    passedValues.Add(item + Random.insideUnitSphere*0.0001f);
                }
            }

            return new List<Vector3>(passedValues);
        }

        public static List<T> DedupCollection<T>(IEnumerable<T> input)
        {
            HashSet<T> passedValues = new HashSet<T>();

            foreach (T item in input)
            {
                if (!passedValues.Contains(item))
                {
                    passedValues.Add(item);
                }
            }

            return new List<T>(passedValues);
        }

        public static void SortClockwiseOnPlane(this Vector3[] input, Vector3 planeNormal)
        {
            Vector3 centroid = Vector3.zero;
            for (int i = 0; i < input.Length; i++)
            {
                centroid += input[i];
            }
            centroid /= input.Length;

            Vector3 referenceVector = input[0] - centroid;

            Array.Sort(input, (p1, p2) => Comparer<float>.Default.Compare(SignedAngle(planeNormal, p1 - centroid, referenceVector), SignedAngle(planeNormal, p2 - centroid, referenceVector)));
        }


        public static void SortClockwiseOnPlane(this List<Vector3> input, Vector3 planeNormal)
        {
            Vector3 centroid = Vector3.zero;
            for (int i = 0; i < input.Count; i++)
            {
                centroid += input[i];
            }
            centroid /= input.Count;

            Vector3 referenceVector = input[0] - centroid;

            input.Sort((p1, p2) => Comparer<float>.Default.Compare(SignedAngle(planeNormal, p1 - centroid, referenceVector), SignedAngle(planeNormal, p2 - centroid, referenceVector)));
        }

        public static void SortAntiClockwiseOnPlane(this List<Vector3> input, Vector3 planeNormal)
        {
            Vector3 centroid = Vector3.zero;
            for (int i = 0; i < input.Count; i++)
            {
                centroid += input[i];
            }
            centroid /= input.Count;

            Vector3 referenceVector = input[0] - centroid;

            input.Sort((p1, p2) => -SignedAngle(planeNormal, p1 - centroid, referenceVector).CompareTo(SignedAngle(planeNormal, p2 - centroid, referenceVector)));
        }

        public static void SortAntiClockwiseOnPlane(this Vector3[] input, Vector3 planeNormal)
        {
            Vector3 centroid = Vector3.zero;
            for (int i = 0; i < input.Length; i++)
            {
                centroid += input[i];
            }
            centroid /= input.Length;

            Vector3 referenceVector = input[0] - centroid;

            Array.Sort(input, (p1, p2) => -SignedAngle(planeNormal, p1 - centroid, referenceVector).CompareTo(SignedAngle(planeNormal, p2 - centroid, referenceVector)));
        }

        public static float SignedAngle(Vector3 referencePlane, Vector3 a, Vector3 b)
        {
            Vector3 c = Vector3.Cross(a, b);
            float angle = Mathf.Atan2(c.sqrMagnitude, Vector3.Dot(a, b)) * Mathf.Rad2Deg;

            return (Vector3.Dot(c, referencePlane) < 0) ? -angle : angle;
        }
    }
}