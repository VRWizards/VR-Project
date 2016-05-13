using System.Collections.Generic;
using UnityEngine;

namespace Destruction.Tools
{
    [AddComponentMenu("Destruction/Sticky Zone")]
    public class StickyZone : MonoBehaviour
    {
        private readonly Dictionary<Collider, Vector3> shardsStuckToMe = new Dictionary<Collider, Vector3>();

        #region Unity API
        private void Update()
        {
            if (shardsStuckToMe == null) return;

            foreach (var pair in shardsStuckToMe)
            {
                pair.Key.transform.position = transform.TransformPoint(pair.Value);
            }
        }

        private void OnDisable()
        {
            foreach (var pair in shardsStuckToMe)
            {
                UnstickObject(pair.Key);
            }
        }

        #endregion

        #region Aux
        private void StickObject(Collider shard)
        {
            if (shard == null) return;

            Destroy(shard.GetComponent<Rigidbody>());
        }

        private void UnstickObject(Collider shard)
        {
            if (shard == null) return;

            if (shard.attachedRigidbody == null)
            {
                shard.gameObject.AddComponent<Rigidbody>();
            }
        }

        private static bool BoxIntersect(Bounds a, Bounds b)
        {
            return b.min.x >= a.min.x && b.max.x <= a.max.x &&
                   b.min.y >= a.min.y && b.max.y <= a.max.y &&
                   b.min.z >= a.min.z && b.max.z <= a.max.z ||
                   a.max.x >= b.min.x && a.min.x <= b.max.x &&
                   (a.max.y >= b.min.y && a.min.y <= b.max.y &&
                   (a.max.z >= b.min.z && a.min.z <= b.max.z));
        }

        internal void DrawGizmos()
        {
            transform.rotation = Quaternion.identity;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            Gizmos.color = new Color(0, 1, 0, 1);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = Matrix4x4.identity;
        }

        #endregion

        internal void PerformStick(List<Collider> brokenPieces)
        {
            Bounds myBounds = new Bounds(transform.position, transform.lossyScale);

            foreach (Collider s in brokenPieces)
            {
                if (BoxIntersect(myBounds, s.GetComponent<Collider>().bounds))
                {
                    StickObject(s);

                    shardsStuckToMe.Add(s, transform.InverseTransformPoint(s.transform.position));
                }
            }
        }
    }
}
