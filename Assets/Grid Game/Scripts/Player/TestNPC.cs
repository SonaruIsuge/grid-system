using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SNR_PathFinding;
using UnityEngine;


public class TestNPC : MonoBehaviour
{
    [SerializeField] private KeyCode findPathKey;
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed; 
    [SerializeField] private float turnDistance;
    [SerializeField] private float updatePathThreshold;
    [SerializeField] private float stoppingDistance;
    
    private GameBoard gameBoard;
    
    // Path Finding
    [SerializeField] private int currentPoint;
    private Path path;
    
    private bool pathFindRequested;
    private bool moveProcessing;

    private CancellationTokenSource cts;

    public event Action<TestNPC, Vector3[], Path, bool> OnPathCallback;

    private void Awake()
    {
        gameBoard = FindObjectOfType<GameBoard>();
    }


    private void Start()
    {
        cts = new CancellationTokenSource();
        
        pathFindRequested = false;
        moveProcessing = false;
        path = null;
        currentPoint = 0;
    }


    private void Update()
    {
        pathFindRequested = Input.GetKeyDown(findPathKey);
        
        // If target move, update new path
        if(moveProcessing) 
            UpdatePath();
        
        // If path exists, stop the process
        if (pathFindRequested)
        {
            if (moveProcessing)
                cts.Cancel();
            
            PathRequestManager.Instance.RequestPath(new PathRequest(gameBoard.Grid, transform.position, target.position, PathFindCallback));
        }

        // If path exists, start move along the path
        if (path != null && !moveProcessing)
        {
            if (cts.IsCancellationRequested)
            {
                cts.Dispose();
                cts = new CancellationTokenSource();
            }
            
            Move(cts.Token).Forget();
        }
    }


    private void PathFindCallback(Vector3[] wayPoints, bool pathFind)
    {
        currentPoint = 0;
        path = pathFind ? new Path(wayPoints, transform.position, turnDistance, stoppingDistance) : null;
        
        OnPathCallback?.Invoke(this, wayPoints, path, pathFind);
    }


    private async UniTask Move(CancellationToken ctn)
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

            await Task.Yield();
        }

        ClearPath();
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
        if ((target.position - path.lookPoints[^1]).sqrMagnitude > sqrPathThreshold)
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
