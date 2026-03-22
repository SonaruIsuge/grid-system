using UnityEngine;

namespace SNR_PathFinding
{
    public class Path
    {
        public readonly Vector3[] lookPoints;
        public readonly Line[] turnBoundaries;
        public readonly int finishLineIndex;
        public readonly int slowDownIndex;


        public Path(Vector3[] wayPoints, Vector3 startPos, float turnDistance, float stoppingDistance)
        {
            lookPoints = wayPoints;
            turnBoundaries = new Line[lookPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;

            var previousPoint = V3ToV2(startPos);
            for (var i = 0; i < lookPoints.Length; i++)
            {
                var currentPoint = V3ToV2(lookPoints[i]);
                var dirToCurrentPoint = (currentPoint - previousPoint).normalized;
                var turnBoundaryPoint = currentPoint - dirToCurrentPoint * turnDistance;
                turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDistance);
                
                previousPoint = turnBoundaryPoint;
            }

            float distanceFromEnd = 0;
            for (var i = finishLineIndex; i > 0; i--)
            {
                distanceFromEnd += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
                if (distanceFromEnd > stoppingDistance)
                {
                    slowDownIndex = i;
                    break;
                }
            }
        }


        public void DrawWithGizmos()
        {
            Gizmos.color = Color.white;
            foreach(var l in turnBoundaries)
            {
                l.DrawWithGizmos(.5f);
            }
        }


        private Vector2 V3ToV2(Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
    }
}