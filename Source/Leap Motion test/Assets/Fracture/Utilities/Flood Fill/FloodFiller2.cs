//#define DEBUG_FLOODFILL

using UnityEngine;

namespace Destruction.Utilities.FloodFiller
{
    public class FloodFiller2 : BaseFloodFiller<int2>
    {
        protected readonly bool[,] visitedSet;
        protected override float maxIter { get { return visitedSet.GetUpperBound(0) * visitedSet.GetUpperBound(1); } }

        public FloodFiller2(FloodFillCancelled OnCancelledCallback, FloodFillFinished finishedCallback, Vector3 startingPoint, Vector3 bottomLeft, Vector3 bounds, float errorRadius)
            : this(OnCancelledCallback, null, finishedCallback, startingPoint, bottomLeft, bounds, errorRadius)
        {
        }

        public FloodFiller2(FloodFillCancelled OnCancelledCallback, IsStepSatisfied SatisfiedContraint, FloodFillFinished finishedCallback, Vector3 startingPoint, Vector3 bottomLeft, Vector3 bounds, float errorRadius)
            : base(OnCancelledCallback, SatisfiedContraint, bottomLeft, errorRadius)
        {
            OnFinishedCallback += finishedCallback;

            int2 start = Point2XY(startingPoint);
            visitedSet = new bool[(int)(Mathf.Abs(bounds.x) / MoveDistance) + 1, (int)(Mathf.Abs(bounds.y) / MoveDistance) + 1];

            openSet.Enqueue(start);

            WorkerThread();
        }

        #region Aux
        private Vector3 XY2Point(int x, int y)
        {
            return new Vector3(x * MoveDistance, y * MoveDistance, 0) + bottomLeft;
        }

        private int2 Point2XY(Vector3 point)
        {
            Vector3 relativePoint = point - bottomLeft;
            relativePoint /= MoveDistance;

            return new int2((int)relativePoint.x, (int)relativePoint.y);
        }

        #endregion

        protected override void FloodFill(int2 coord)
        {
            int X = coord.x;
            int Y = coord.y;

            if (X < 0 || X >= visitedSet.GetUpperBound(0) || Y < 0 || Y >= visitedSet.GetUpperBound(1)) return;
            if (visitedSet[X, Y]) return;

            Vector3 point = XY2Point(X, Y);
            visitedSet[X, Y] = true;

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
                    if (i == 0 && j == 0) continue;

                    openSet.Enqueue(new int2(X + i, Y + j));
                }
            }
        }
    }
}
