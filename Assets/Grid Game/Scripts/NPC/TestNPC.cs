
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SNR_Event;
using SNR_PathFinding;
using UnityEngine;
using UtilSNR.Common;


public class TestNPC : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed; 
    [SerializeField] private float turnDistance;
    [SerializeField] private float updatePathThreshold;
    [SerializeField] private float stoppingDistance;
    
    private GameBoard gameBoard;
    
    // Path Finding
    private Vector3 target;
    private int currentPoint;
    private Path path;
    
    private bool pathFindRequested;
    private bool moveProcessing;

    private CancellationTokenSource cts;

    private void Awake()
    {
        gameBoard = GameManager.Instance.GameBoard;
    }

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
        
        pathFindRequested = false;
        moveProcessing = false;
        path = null;
        currentPoint = 0;
    }

    private void OnDisable()
    {
        cts.CancelAndDispose();
    }

    private void Update()
    {
        // If target move, update new path
        // if(moveProcessing) 
        //     UpdatePath();
        
        // If path exists, stop the process
        if (pathFindRequested)
        {
            if (moveProcessing)
                cts = cts.Refresh();
            
            PathRequestManager.Instance.RequestPath(new PathRequest(gameBoard.Grid, transform.position, target, PathFindCallback));
            pathFindRequested = false;
        }

        // If path exists, start move along the path
        // if (path != null && !moveProcessing)
        // {   
        //     Move(cts.Token).Forget();
        // }
    }

    public void RequestMoveToTarget(Vector3 newTarget)
    {
        target = newTarget;
        pathFindRequested = true;
    }


    private void PathFindCallback(Vector3[] wayPoints, bool pathFind)
    {
        currentPoint = 0;
        path = pathFind ? new Path(wayPoints, transform.position, turnDistance, stoppingDistance) : null;
        
        if (path != null && !moveProcessing)
        {   
            Move(cts.Token).Forget(); 
        }
        
        EventManager.RaiseEvent(new OnNpcFindPath
        {
            HasPath = pathFind,
            Npc = this,
            WayPoints =  wayPoints,
            Path = path
        });
    }


    private async UniTaskVoid Move(CancellationToken ctn)
    {
        currentPoint = 0;
        moveProcessing = true;

        transform.LookAt(PointWithPlayerHeight(path.lookPoints[0]));

        while (currentPoint <= path.finishLineIndex)
        {
            if (ctn.IsCancellationRequested)
            {
                ClearPath();
                return;
            }
            
            var pos2D = new Vector2(transform.position.x, transform.position.z);
            var speedPercent = 1.0f;
            if (currentPoint >= path.slowDownIndex && stoppingDistance > 0)
            {
                speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceToPoint(pos2D) / stoppingDistance);
                if (speedPercent <= 0.01f)
                    break;
            }

            var targetRotation = Quaternion.LookRotation(PointWithPlayerHeight(path.lookPoints[currentPoint]) - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            transform.Translate(Vector3.forward * (Time.deltaTime * moveSpeed * speedPercent), Space.Self);

            while (path.turnBoundaries[currentPoint].HasCrossLine(pos2D))
            {
                currentPoint++;
                if (currentPoint > path.finishLineIndex)
                    break;
            }

            await UniTask.Yield();
        }

        ClearPath();
        
        EventManager.RaiseEvent(new OnNpcReachTarget
        {
            Npc = this
        });
    }


    private Vector3 PointWithPlayerHeight(Vector3 point)
    {
        return new Vector3(point.x, transform.position.y, point.z);
    }


    private void UpdatePath()
    {
        if(path == null) 
            return;

        var sqrPathThreshold = updatePathThreshold * updatePathThreshold;
        if ((target - path.lookPoints[^1]).sqrMagnitude < sqrPathThreshold)
        {
            pathFindRequested = true;
        }
    }


    private void ClearPath()
    {
        moveProcessing = false;
        path = null;
    }
}
