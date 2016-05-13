using System;
using Destruction.Common;
using UnityEngine;

namespace Destruction.Tools
{
    [AddComponentMenu("Destruction/Collision Trigger")]
    public class Trigger : MonoBehaviour
    {
        [Flags]
        public enum TriggerType
        {
            Collision =     1,
            Bullet =        2,
            Explosion =     4
        }

        public TriggerType triggerType;

        /// <summary>
        /// This is the minimum force required from a collision to trigger any fracture components this object is linked to.  
        /// Setting this to 0 would effectively make any collision trigger the fracture.
        /// </summary>
        [SerializeField] private float impactForceThreshold = 10;

        [SerializeField] private bool onlyTriggerWithTag;
        [SerializeField] private string triggerTag;

        [SerializeField] private float startingHealth = 1.0f;
        [SerializeField] private float toughness = 0.5f;

        [SerializeField] private float currentHealth;
        private float CurrentHealth
        {
            get { return currentHealth; }
            set 
            { 
                currentHealth = Mathf.Clamp01(value);
                CheckHealth();
            }
        }

        private Vector4 lastImpactPoint;

        private bool hasBeenTriggered;

        private float InverseToughness          { get { return 1 - toughness; } }
        private float ImpactForceThresholdSqr   { get { return impactForceThreshold * impactForceThreshold; } }


        private void Awake()
        {
            CurrentHealth = startingHealth;
        }

        private void CheckHealth()
        {
            if (CurrentHealth <= 0)
            {
                TriggerDestruction(lastImpactPoint, lastImpactPoint.w);
            }
        }

        private bool ShouldCollisionTrigger(string tag)
        {
            return (!onlyTriggerWithTag) || (onlyTriggerWithTag && tag.Equals(triggerTag));
        }

        /// <summary>
        /// Finds all the destructable components linked to this object, and destroys them.
        /// </summary>
        /// <param name="triggerPosition">Impact position, pivot point for the destruction.</param>
        /// <param name="magnitude">Magnitude of the impact force.</param>
        public void TriggerDestruction(Vector3 triggerPosition, float magnitude)
        {
            if(hasBeenTriggered) return;
            hasBeenTriggered = true;

            foreach (IDestructable d in gameObject.GetInterfaces<IDestructable>())
            {
                d.Destroy(triggerPosition, magnitude);
            }
        }

        #region Triggers
        private void OnCollisionEnter(Collision c)
        {
            if ((triggerType & TriggerType.Collision) == 0) return;
            if (!ShouldCollisionTrigger(c.transform.tag)) return;

            float force = c.relativeVelocity.sqrMagnitude;

            if (force < ImpactForceThresholdSqr) return;

            lastImpactPoint = c.contacts[0].point;
            lastImpactPoint.w = Mathf.Clamp(Mathf.Sqrt(force - ImpactForceThresholdSqr), 0.1f, 1.5f);
            CurrentHealth = 0;
        }

        private void OnTriggerEnter(Collider c)
        {
            if (!GetComponent<Collider>().isTrigger) return;
            if ((triggerType & TriggerType.Collision) == 0) return;
            if (!ShouldCollisionTrigger(c.tag)) return;

            lastImpactPoint = transform.position;
            lastImpactPoint.w = 1.0f;
            CurrentHealth = 0;
        }

        private void OnBulletHit(Vector4 bulletInfo)
        {
            if ((triggerType & TriggerType.Bullet) == 0) return;

            lastImpactPoint = bulletInfo;
            lastImpactPoint.w = 0.33f;
            CurrentHealth -= bulletInfo.w * InverseToughness;
        }

        private void OnExplosionHit(Vector4 explosionInfo)
        {
            if ((triggerType & TriggerType.Explosion) == 0) return;

            lastImpactPoint = explosionInfo;
            lastImpactPoint.w = 0.5f;
            CurrentHealth -= explosionInfo.w * InverseToughness;
        }

        private void OnCriticalExplosionHit(Vector3 explosionInfo)
        {
            if ((triggerType & TriggerType.Explosion) == 0) return;

            lastImpactPoint = explosionInfo;
            lastImpactPoint.w = 0.1f;
            CurrentHealth = 0;
        }

        #endregion

    }
}
