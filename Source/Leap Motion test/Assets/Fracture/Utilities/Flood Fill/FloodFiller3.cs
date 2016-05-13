//#define DEBUG_FLOODFILL

using UnityEngine;

namespace Destruction.Utilities.FloodFiller
{
    public class FloodFiller3 : BaseFloodFiller<int3>
    {
        protected readonly bool[,,] visitedSet;
        protected override float maxIter { get { return visitedSet.GetUpperBound(0) * visitedSet.GetUpperBound(1) * visitedSet.GetUpperBound(2); } }

        public FloodFiller3(FloodFillCancelled OnCancelledCallback, FloodFillFinished finishedCallback, Vector3 startingPoint, Vector3 bottomLeft, Vector3 bounds, float errorRadius)
            : this(OnCancelledCallback, null, finishedCallback, startingPoint, bottomLeft, bounds, errorRadius)
        {
        }

        public FloodFiller3(FloodFillCancelled OnCancelledCallback, IsStepSatisfied SatisfiedContraint, FloodFillFinished finishedCallback, Vector3 startingPoint, Vector3 bottomLeft, Vector3 bounds, float errorRadius)
            : base(OnCancelledCallback, SatisfiedContraint, bottomLeft, errorRadius)
        {
            OnFinishedCallback += finishedCallback;

            int3 start = Point2XYZ(startingPoint);
            visitedSet = new bool[(int)(Mathf.Abs(bounds.x) / MoveDistance) + 1, (int)(Mathf.Abs(bounds.y) / MoveDistance) + 1, (int)(Mathf.Abs(bounds.z) / MoveDistance) + 1];
            
            openSet.Enqueue(start);

            WorkerThread();
        }

        #region Aux
        private Vector3 XYZ2Point(int x, int y, int z)
        {
            return new Vector3(x * MoveDistance, y * MoveDistance, z * MoveDistance) + bottomLeft;
        }

        private int3 Point2XYZ(Vector3 point)
        {
            Vector3 relativePoint = point - bottomLeft;
            relativePoint /= MoveDistance;

            return new int3((int)relativePoint.x, (int)relativePoint.y, (int)relativePoint.z);
        }

        #endregion

        protected override void FloodFill(int3 coord)
        {
            int X = coord.x;
            int Y = coord.y;
            int Z = coord.z;

            if (X < 0 || X >= visitedSet.GetUpperBound(0) || Y < 0 || Y >= visitedSet.GetUpperBound(1) || Z < 0 || Z >= visitedSet.GetUpperBound(2)) return;
            if (visitedSet[X, Y, Z]) return;
            //Debug.Log(coord);

            Vector3 point = XYZ2Point(X, Y, Z);
            visitedSet[X, Y, Z] = true;

            currentIterationCounter++;

            Collider[] colliders = Physics.OverlapSphere(point, errorRadius);

#if DEBUG_FLOODFILL
                if (colliders.Length > 0)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = point;
                    sphere.transform.localScale = Vector3.one * MoveDistance;
                }
#endif

            collidersFound.AddRange(colliders);

            if (SatisfiedContraint(colliders)) return;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        if (i == 0 && j == 0 && k == 0) continue;

                        openSet.Enqueue(new int3(X + i, Y + j, Z + k));
                    }
                }
            }
        }
    }
}