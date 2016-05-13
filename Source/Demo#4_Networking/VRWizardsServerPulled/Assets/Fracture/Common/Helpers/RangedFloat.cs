using UnityEngine;

namespace Destruction.Common
{
    [System.Serializable]
    public class RangedFloat
    {
        [SerializeField]
        private float min = 1;
        [SerializeField]
        private float max = 10;

        public float Min
        {
            get { return min; }
        }

        public float Max
        {
            get { return max; }
        }


        public RangedFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float Random()
        {
            return UnityEngine.Random.Range(min, max);
        }

        public float Clamp(float input)
        {
            return Mathf.Clamp(input, min, max);
        }
    }
}