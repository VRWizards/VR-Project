using System.Collections.Generic;
using Destruction.Common;
using UnityEngine;

namespace Destruction.Tools
{
    public class LinkObjectsOnDestroy : MonoBehaviour, IDestructionAffectable
    {
        public GameObject[] objectsToDestroy;
        public MonoBehaviour[] componentsToDisable;
        public MonoBehaviour[] componentsToEnable;

        void IDestructionAffectable.OnPreDestruction(BaseDestructable objectToDestroy)
        {
        }

        public void OnPostDestruction(List<Collider> fragments)
        {
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj == null) continue;

                Destroy(obj);
            }

            foreach (MonoBehaviour component in componentsToDisable)
            {
                if (component == null) continue;

                component.enabled = false;
            }

            foreach (MonoBehaviour component in componentsToEnable)
            {
                if (component == null) continue;

                component.enabled = true;
            }
        }
    }
}