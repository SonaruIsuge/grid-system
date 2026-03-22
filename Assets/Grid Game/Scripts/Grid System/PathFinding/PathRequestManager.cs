using System;
using System.Collections.Generic;
using System.Threading;
using SonaruUtilities;
using UnityEngine;

namespace SNR_PathFinding
{
    public struct PathRequest
    {
        public Grid<PathFindableTile> Grid;
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Action<Vector3[], bool> Callback;

        public PathRequest(Grid<PathFindableTile> grid, Vector3 startPos, Vector3 endPos, Action<Vector3[], bool> callback)
        {
            Grid = grid;
            StartPos = startPos;
            EndPos = endPos;
            Callback = callback;
        }
    }


    public struct PathResult
    {
        public Vector3[] Path;
        public bool Success;
        public Action<Vector3[], bool> Callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
        {
            Path = path;
            Success = success;
            Callback = callback;
        }
    }
    
    
    public class PathRequestManager : TSingletonMonoBehaviour<PathRequestManager>
    {
        private Queue<PathResult> pathResults;
        private PathFinding pathFinding;
        

        protected override void Awake()
        {
            base.Awake();
            pathFinding = new PathFinding();
            pathResults = new Queue<PathResult>();
        }


        private void Update()
        {
            if (pathResults.Count > 0)
            {
                var itemInQueue = pathResults.Count;
                lock (pathResults)
                {
                    for (var i = 0; i < itemInQueue; i++)
                    {
                        var result = pathResults.Dequeue();
                        result.Callback(result.Path, result.Success);
                    }
                }
            }
        }


        public void RequestPath(PathRequest pathRequest)
        {
            ThreadStart threadStart = delegate
            {
                Instance.pathFinding.FindSimplifyPath(pathRequest, Instance.FinishedProcessingPath);
            };
            threadStart.Invoke();
        }


        public void FinishedProcessingPath(PathResult result)
        {
            lock (pathResults)
            {
                pathResults.Enqueue(result);    
            }
        }

    }
}