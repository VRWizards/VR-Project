using System.Collections.Generic;
using Destruction.Tools;
using UnityEngine;

namespace Destruction.Common
{
    public abstract class BaseDestructable : MonoBehaviour, IDestructable, IDestructionAffectable, ISchedulable
    {
        public StickyZone[] stickyZones;

        public bool destroyWhenOffscreen = true;
        public RangedFloat offscreenTimer = new RangedFloat(10f, 20f);

        public bool useDestroyTimer = true;
        public RangedFloat destroyTime = new RangedFloat(10f, 20f);

        public bool useMyTag = true;
        public bool useMyLayer = true;

        public bool useScheduler;

        public List<Collider> finalizedBrokenPieces = new List<Collider>();

        public Vector3 LastImpactPoint { get; protected set; }
        protected bool Complete { get; set; }

        protected abstract void BeginFracture(Vector3 point, float size);

        private bool isDestructionScheduled;
        private Vector4 storedInfo;

        #region IDestructable
        void IDestructable.Destroy(Vector3 point, float force)
        {
            if (isDestructionScheduled && useScheduler) return;

            if(useScheduler)
            {
                isDestructionScheduled = true;
                storedInfo = new Vector4(point.x, point.y, point.z, force);
                Scheduler.ScheduleObject(this);
            }
            else
            {
                BeginFracture(point, force);
            }
        }

        #endregion

        #region ISchedulable
        public void BeginScheduledOperation()
        {
            if (!isDestructionScheduled) return;

            BeginFracture(storedInfo, storedInfo.w);
        }

        #endregion


        #region IDestructionAffectable
        public void OnPreDestruction(BaseDestructable objectToDestroy)
        {
        }

        public void OnPostDestruction(List<Collider> fragments)
        {
            if (stickyZones != null)
            {
                foreach (StickyZone sZone in stickyZones)
                {
                    sZone.PerformStick(finalizedBrokenPieces);
                }
            }

            foreach (var piece in fragments)
            {
                GameObject obj = piece.gameObject;
                if (useMyLayer)
                {
                    obj.layer = gameObject.layer;
                }

                if (useMyTag)
                {
                    obj.tag = gameObject.tag;
                }

                if(destroyWhenOffscreen)
                {
                    obj.AddComponent<DestroyMeOffscreen>().TimeBeforeDestroy = offscreenTimer;
                }

                if(useDestroyTimer)
                {
                    Destroy(obj, destroyTime.Random());
                }
            }
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            if (stickyZones == null) return;
            foreach (StickyZone sZone in stickyZones)
            {
                if (sZone == null) continue;

                sZone.DrawGizmos();
            }
        }
    }
}