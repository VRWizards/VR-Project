using Destruction.Common;
using UnityEngine;

namespace Destruction.Tools
{
    public class DestroyMeOffscreen : MonoBehaviour
    {
        private bool isVisible = true;
        private float timer;
        private RangedFloat timeBeforeDestroy = new RangedFloat(5, 10);
        
        public RangedFloat TimeBeforeDestroy { set { timeBeforeDestroy = value; } }
        
        private void Update()
        {
            if (isVisible) return;

            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnBecameVisible()
        {
            isVisible = true;
        }

        private void OnBecameInvisible()
        {
            isVisible = false;

            timer = timeBeforeDestroy.Random();
        }
    }
}
