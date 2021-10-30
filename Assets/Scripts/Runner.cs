using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour, IPoolable
{
    public CharacterController CharacterController;
    public Animator Animator;
    public Collider Collider;
    public float Speed;
    public Vector3 Target;

    private bool _isCaught;
    private bool _isEscaped;
    private float _timer;

    private void StartRunning(Dictionary<string,object> message)
    {
        StartCoroutine(Move());
    }
    private IEnumerator Move()
    {
        while (true)
        {
            if (_isCaught || _isEscaped || transform.position.y <= -20f)
            {
                Animator.SetBool("IsFalling", false);
                yield break;
            }
            var pos = transform.position;
            var targetPos = new Vector3(Target.x,pos.y,pos.z);
            
            if (!FallStuck())
            {
                Animator.SetBool("IsFalling", false);
                transform.position = Vector3.MoveTowards(pos, targetPos, Time.deltaTime * Speed);
            }
            else
            {
                Animator.SetBool("IsFalling", true);
            }
            yield return null;
        }
    }
    private bool FallStuck() //Fall object down when stuck
    {
        if (transform.position.y < 0.47f)
        {
            _timer += Time.deltaTime;
            if (_timer > 0.2f)
            {
                return true;
            }
        }
        else
        {
            _timer = 0f;
        }

        return false;
    }
    public void ResetRunner()
    {
        Animator.SetBool("IsCaught", false);
        Animator.SetBool("IsRunning", false);
        Animator.SetBool("IsEscaped", false);
        gameObject.tag = "Untagged";
        Collider.isTrigger = false;
        Collider.attachedRigidbody.isKinematic = false;
        _isCaught = false;
        _isEscaped = false;
        PoolManager.Instance.PoolObject("runner", this);
    }
    private void Caught()
    {
        Animator.SetBool("IsCaught", true);
        Collider.isTrigger = true;
        Collider.attachedRigidbody.isKinematic = true;
        _isCaught = true;
        EventManager.TriggerEvent("OnCaught", null);
        
    }
    private void Escaped(Dictionary<string,object> message)
    {
        Animator.SetBool("IsEscaped", true);
        Collider.isTrigger = true;
        Collider.attachedRigidbody.isKinematic = true;
        _isEscaped = true;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Chaser"))
        {
            Caught();
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("OnChaseStart",StartRunning);
        EventManager.StartListening("OnEscape",Escaped);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnChaseStart",StartRunning);
        EventManager.StopListening("OnEscape",Escaped);
    }
}