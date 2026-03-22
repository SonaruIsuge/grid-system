using UnityEngine;

namespace SNR_PathFinding
{
    /// <summary>
    /// y = gradient * x + yIntercept
    /// </summary>
    public struct Line
    {
        private const float infinityLineGradient = 1e5f;

        private float gradient;
        private float yIntercept;

        private Vector2 pointOnLine1;
        private Vector2 pointOnLine2;

        private float gradientPerpendicular;

        private bool approachSide;
        

        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
        {
            var dx = pointOnLine.x - pointPerpendicularToLine.x;
            var dy = pointOnLine.y - pointPerpendicularToLine.y;

            gradientPerpendicular = dx == 0 ? infinityLineGradient : dy / dx;
            gradient = gradientPerpendicular == 0 ? infinityLineGradient : -1 / gradientPerpendicular;

            yIntercept = pointOnLine.y - gradient * pointOnLine.x;

            pointOnLine1 = pointOnLine;
            pointOnLine2 = pointOnLine + new Vector2(1, gradient);

            approachSide = false;
            approachSide = PointInLeftSideOfLine(pointPerpendicularToLine);
        }


        public bool HasCrossLine(Vector2 point)
        {
            return PointInLeftSideOfLine(point) != approachSide;
        }


        public float DistanceToPoint(Vector2 point)
        {
            var yInterceptPerpendicular = point.y - gradientPerpendicular * point.x;
            var intersectX = (yInterceptPerpendicular - yIntercept) / (gradient - gradientPerpendicular);
            var intersectY = gradient * intersectX + yIntercept;

            return Vector2.Distance(point, new Vector2(intersectX, intersectY));
        }


        public void DrawWithGizmos(float length)
        {
            var lineDir = new Vector3(1, 0, gradient).normalized;
            var lineCenter = new Vector3(pointOnLine1.x, 0, pointOnLine1.y);
            Gizmos.DrawLine(lineCenter - lineDir * length / 2f, lineCenter + lineDir * length / 2f);
        }
        
        
        private bool PointInLeftSideOfLine(Vector2 point)
        {
            return (point.x - pointOnLine1.x) * (pointOnLine2.y - pointOnLine1.y) >
                   (point.y - pointOnLine1.y) * (pointOnLine2.x - pointOnLine1.x);
        }
    }
}