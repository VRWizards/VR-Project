#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace Destruction.Utilities.FloodFiller
{
    public abstract class BaseFloodFiller<T> where T : struct
    {
        public delegate bool IsStepSatisfied(Collider[] colliders);
        public delegate void FloodFillFinished(Collider[] collidersFound);
        public delegate void FloodFillCancelled();

        protected event FloodFillFinished OnFinishedCallback;
        protected event FloodFillCancelled OnCancelledCallback;
        protected IsStepSatisfied SatisfiedContraint;

        protected float errorRadius = 0.025f;
        protected const int maxIterationCounter = 2000000;

        protected readonly List<Collider> collidersFound = new List<Collider>();
        protected readonly Queue<T> openSet = new Queue<T>(256);
        protected readonly Vector3 bottomLeft;

        protected int currentIterationCounter;
        private bool shouldStop;

        protected float MoveDistance { get { return errorRadius * 2; } }

        protected abstract void FloodFill(T coord);

        protected BaseFloodFiller(FloodFillCancelled OnCancelledCallback, IsStepSatisfied SatisfiedContraint, Vector3 bottomLeft, float errorRadius)
        {
            this.errorRadius = errorRadius;
            this.bottomLeft = bottomLeft;
            this.SatisfiedContraint = SatisfiedContraint ?? DefaultSatisfiedContraint;
            this.OnCancelledCallback = OnCancelledCallback;
        }

        public void Cancel()
        {
            shouldStop = true;

            OnCancelledCallback();

        }

        private bool DefaultSatisfiedContraint(Collider[] colliders)
        {
            return colliders.Length > 0;
        }

        protected void WorkerThread()
        {
            while ((currentIterationCounter < maxIterationCounter) && (openSet.Count > 0) && !shouldStop)
            {
#if UNITY_EDITOR
                if(EditorUtility.DisplayCancelableProgressBar("Flood Filler - Fracture", "Processing...", currentIterationCounter / maxIter))
                {
                    Cancel();
                }
#endif  
                if (openSet.Count > 0)
                {
                    FloodFill(openSet.Dequeue());
                }
            }

#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("Flood Filler - Fracture", "Done...", 1);
            EditorUtility.ClearProgressBar();
#endif
            OnFinishedCallback(DestructionHelper.DedupCollection(collidersFound).ToArray());
        }

        protected abstract float maxIter { get; }
    }
}
