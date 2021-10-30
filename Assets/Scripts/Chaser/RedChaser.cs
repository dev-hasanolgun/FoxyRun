using System.Collections;
using UnityEngine;

public class RedChaser : Chaser, ILeap
{
    public float TargetVelocity = 10f;
    public float Distance = 1;
    public float RayRange = 2;

    private Vector3 _targetDir;
    private Vector3 _leftDir;
    private Vector3 _rightDir;
    private bool _isLeapFinished;
    private float _leapTimer;
    
    public override IEnumerator Run()
    {
        yield return null;
    }

    private Vector3 RaycastAvoidance() // Cast 3 ray right infront of the chaser distanced between them
    {
        var pos = transform.position;
        var deltaPosition = Vector3.zero;
        var runnerPos = Runner.transform.position;
        _targetDir = (runnerPos - pos).normalized;
        _leftDir = _targetDir + Vector3.forward * Distance;
        _rightDir = _targetDir + Vector3.back * Distance;
        
        var leftCast = !Physics.Raycast(pos+_leftDir+Vector3.up, Vector3.down, RayRange, 1 << 9);
        var rightCast = !Physics.Raycast(pos+_rightDir+Vector3.up, Vector3.down, RayRange, 1 << 9);
        var targetCast = !Physics.Raycast(pos+_targetDir+Vector3.up, Vector3.down, RayRange, 1 << 9);
        
        deltaPosition += TargetVelocity * _targetDir;
        if (targetCast)
        {
            if (leftCast && rightCast)
            {
                if (Physics.Raycast(pos+_leftDir+Vector3.up+Vector3.down*RayRange, Vector3.forward, RayRange, 1 << 9))
                {
                    deltaPosition -= TargetVelocity * _rightDir + Vector3.back;
                }
                else if (Physics.Raycast(pos+_rightDir+Vector3.up+Vector3.down*RayRange, Vector3.back, RayRange, 1 << 9))
                {
                    deltaPosition -= TargetVelocity * _leftDir + Vector3.forward;
                }
            }
            else if (leftCast)
            {
                deltaPosition -= TargetVelocity * _leftDir + Vector3.forward;
            }
            else if (rightCast)
            {
                deltaPosition -= TargetVelocity * _rightDir + Vector3.back;
            }
        }
        else
        {
            if (leftCast && rightCast)
            {
                deltaPosition += TargetVelocity * _targetDir;
            }
            else if (leftCast)
            {
                deltaPosition -= TargetVelocity * _leftDir + Vector3.forward;
            }
            else if (rightCast)
            {
                deltaPosition -= TargetVelocity * _rightDir + Vector3.back;
            }
            else
            {
                deltaPosition += TargetVelocity * _targetDir;
            }
        }

        return deltaPosition;
    }
    
    public void Leap() // Leap if possible
    {
        var pos = transform.position;
        if (pos.y > 0.5f && Physics.Raycast(transform.position+Vector3.up+Vector3.right, Vector3.down,50f, 1 << 9)) // Stop trying to leap if no falling down and not under anything
        {
            Animator.SetBool("IsJumping", false);
            Animator.SetBool("IsFalling", false);
            _leapTimer = 0; // Reset leap
            return;
        }
        
        _leapTimer += Time.deltaTime;
        if (_leapTimer < 0.15f) // Leap once while falling down
        {
            Animator.SetBool("IsJumping", true);
            transform.position = Vector3.MoveTowards(pos, pos+Vector3.up*0.1f, Time.deltaTime * 4f);
        }
        else
        {
            Animator.SetBool("IsFalling", true);
        }
    }
    
    private void Update()
    {
        Leap();
        
        if (IsChasing) // Run method, not in coroutine since not a waypoint base
        {
            var deltaPosition = RaycastAvoidance();
            var pos = transform.position;
            var rotation = Quaternion.LookRotation(deltaPosition,Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*2f);
            transform.position = Vector3.Lerp(pos, pos+deltaPosition, Time.deltaTime);
            KillChaser();
        }
    }
    private void OnDisable()
    {
        PoolManager.Instance.PoolObject("dumbChaser", this);
    }
}
