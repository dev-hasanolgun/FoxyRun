using System;
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform Runner;
    public float TargetVelocity = 10f;
    public int NumberOfRays = 17;
    public float Angle = 90;
    public float Distance = 1;
    public float GroundAngle = 15;
    public float RayRange = 2;

    private Vector3 _targetDir;
    private Vector3 _leftDir;
    private Vector3 _rightDir;

    private IEnumerator Testing()
    {
        while (true)
        {
            var pos = transform.position;
                
            _targetDir = (Runner.transform.position - pos).normalized;
            _leftDir = _targetDir + Vector3.left * Distance;
            _rightDir = _targetDir + Vector3.right * Distance;
                
            var deltaPosition = Vector3.zero;
            
            var leftCast = !Physics.Raycast(pos+_leftDir+Vector3.up, Vector3.down, RayRange);
            var rightCast = !Physics.Raycast(pos+_rightDir+Vector3.up, Vector3.down, RayRange);
            var targetCast = !Physics.Raycast(pos+_targetDir+Vector3.up, Vector3.down, RayRange);
            
            deltaPosition += TargetVelocity * _targetDir;
            
            if (targetCast)
            {
                if (leftCast && rightCast)
                {
                    if (Physics.Raycast(pos+_leftDir+Vector3.up+Vector3.down*RayRange, Vector3.left, RayRange))
                    {
                        deltaPosition -= TargetVelocity * _rightDir + Vector3.right;
                    }
                    else if (Physics.Raycast(pos+_rightDir+Vector3.up+Vector3.down*RayRange, Vector3.right, RayRange))
                    {
                        deltaPosition -= TargetVelocity * _leftDir + Vector3.left;
                    }
                }
                else if (leftCast)
                {
                    deltaPosition -= TargetVelocity * _leftDir + Vector3.left;
                }
                else if (rightCast)
                {
                    deltaPosition -= TargetVelocity * _rightDir + Vector3.right;
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
                    deltaPosition -= TargetVelocity * _leftDir + Vector3.left;
                }
                else if (rightCast)
                {
                    deltaPosition -= TargetVelocity * _rightDir + Vector3.right;
                }
                else
                {
                    deltaPosition += TargetVelocity * _targetDir;
                }
            }
            var rotation = Quaternion.LookRotation(-deltaPosition,Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*2f);
            transform.position = Vector3.Lerp(pos, pos + deltaPosition, Time.deltaTime);
            yield return null;
        }
    }

    private void Start()
    {
        StartCoroutine(Testing());
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        _targetDir = (Runner.transform.position - pos).normalized;
        var targetDirCast = (Runner.transform.position - pos).normalized;
        var leftDirCast = _targetDir + Vector3.left * Distance;
        var rightDirCast = _targetDir + Vector3.right * Distance;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pos+leftDirCast+Vector3.up, Vector3.down * RayRange);
        Gizmos.DrawRay(pos+rightDirCast+Vector3.up, Vector3.down * RayRange);
        Gizmos.DrawRay(pos+targetDirCast+Vector3.up, Vector3.down * RayRange);
        Gizmos.DrawRay(pos+leftDirCast+Vector3.up+Vector3.down * RayRange, Vector3.left * RayRange);
        Gizmos.DrawRay(pos+rightDirCast+Vector3.up+Vector3.down * RayRange, Vector3.right * RayRange);
        
    }
    // var pos = transform.position;
    //     
    // _targetDirCast = (Runner.transform.position - pos).normalized + Vector3.down*GroundAngle;
    // _leftDirCast = _targetDir + Vector3.left * Distance;
    // _rightDirCast = _targetDir + Vector3.right * Distance;
    //     
    // _targetDir = (Runner.transform.position - pos).normalized;
    // _leftDir = _targetDir + Vector3.left * Distance;
    // _rightDir = _targetDir + Vector3.right * Distance;
    //     
    // var deltaPosition = Vector3.zero;
    //
    // var leftCast = !Physics.SphereCast(pos+_leftDirCast+Vector3.up,0.1f, Vector3.down, out var hit1, RayRange);
    // var rightCast = !Physics.SphereCast(pos+_rightDirCast+Vector3.up,0.1f, Vector3.down, out var hit2, RayRange);
    // var targetCast = !Physics.SphereCast(pos+_targetDirCast+Vector3.up,0.1f, Vector3.down, out var hit3, RayRange);
    //     if (targetCast)
    // {
    //     deltaPosition += TargetVelocity * _targetDir;
    //     if (leftCast)
    //     {
    //         deltaPosition -= TargetVelocity * _leftDir + Vector3.left;
    //     }
    //     else if (rightCast)
    //     {
    //         deltaPosition -= TargetVelocity * _rightDir + Vector3.right;
    //     }
    // }
    // else
    // {
    //     if (leftCast && rightCast)
    //     {
    //         deltaPosition += TargetVelocity * _targetDir;
    //     }
    //     else if (leftCast)
    //     {
    //         deltaPosition -= TargetVelocity * _leftDir + Vector3.left;
    //     }
    //     else if (rightCast)
    //     {
    //         deltaPosition -= TargetVelocity * _rightDir + Vector3.right;
    //     }
    //     else
    //     {
    //         deltaPosition += TargetVelocity * _targetDir;
    //     }
    // }
    ////////////////////////////////////////////////////////////
    // var pos = transform.position;
    // var deltaPosition = Vector3.zero;
    // _targetDir = (Runner.transform.position - pos).normalized;
    // if (Physics.Raycast(pos+_targetDir+Vector3.up,Vector3.down,RayRange))
    // {
    //     deltaPosition += 1f / (NumberOfRays / 2 - 2) * TargetVelocity * _targetDir;
    // }
    // else
    // {
    //     deltaPosition -= 1f / (NumberOfRays / 2 - 2) * TargetVelocity * _targetDir;
    // }
    //     
    // for (int i = 1; i < NumberOfRays/2; i++)
    // {
    //     if (Physics.Raycast(pos+_targetDir+Vector3.left*(i * Distance)+Vector3.up,Vector3.down,RayRange))
    //     {
    //         deltaPosition += 1f / (NumberOfRays / 2 - 2) * TargetVelocity * (_targetDir + Vector3.left * Distance);
    //     }
    //     else
    //     {
    //         deltaPosition -= 1f / (NumberOfRays / 2 - 2) * TargetVelocity * (_targetDir + Vector3.left * Distance);
    //     }
    //     if (Physics.Raycast(pos+_targetDir+Vector3.right*(i * Distance)+Vector3.up,Vector3.down,RayRange))
    //     {
    //         deltaPosition += 1f / (NumberOfRays / 2 - 2) * TargetVelocity * (_targetDir + Vector3.right * Distance);
    //     }
    //     else
    //     {
    //         deltaPosition -= 1f / (NumberOfRays / 2 - 2) * TargetVelocity * (_targetDir + Vector3.right * Distance);
    //     }
    // }
}
