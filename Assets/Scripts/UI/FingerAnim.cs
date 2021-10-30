using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerAnim : MonoBehaviour
{
    public float Distance;
    public float Speed;

    private Vector3 _leftPoint;
    private Vector3 _rightPoint;
    private bool _movementCheck;

    private void Start()
    {
        var pos = transform.localPosition;
        _leftPoint = pos + Vector3.left * Distance;
        _rightPoint = pos + Vector3.right * Distance;
    }

    void Update() // I know its cringe not to use dotween already
    {
        var pos = transform.localPosition;

        if (_movementCheck)
        {
            transform.localPosition = Vector3.Lerp(pos, _rightPoint, Time.deltaTime * Speed);
            if (Vector3.Distance(pos, _rightPoint) < 20f)
            {
                _movementCheck = false;
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(pos, _leftPoint, Time.deltaTime * Speed);
            if (Vector3.Distance(pos, _leftPoint) < 20f)
            {
                _movementCheck = true;
            }
        }
    }
}
