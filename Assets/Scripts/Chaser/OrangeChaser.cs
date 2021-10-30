using UnityEngine;

public class OrangeChaser : Chaser ,ILeap
{
    private bool _isLeapFinished;
    private float _leapTimer;

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
            transform.position = Vector3.MoveTowards(pos, pos+Vector3.up*2f, Time.deltaTime * 4f);
        }
        else
        {
            Animator.SetBool("IsFalling", true);
        }
    }

    private void Update()
    {
        Leap();
    }
    private void OnDisable()
    {
        PoolManager.Instance.PoolObject("dumberChaser", this);
    }
}