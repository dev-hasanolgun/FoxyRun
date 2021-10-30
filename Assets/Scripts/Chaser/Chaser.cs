using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chaser : MonoBehaviour, IPoolable
{
    public Runner Runner;
    public Collider Collider;
    public Animator Animator;
    public RagdollController RagdollController;
    public Vector3[] Waypoints;
    public float CappedSpeed;
    public bool IsChasing;

    private bool _isCatched;
    private float _timer;
    private float _slideTimer;
    
    public virtual IEnumerator Run()
    {
        var targetIndex = 1; // Skip first point which is spawn point
        var currentWaypoint = Waypoints[targetIndex];
        while (true)
        {
            if (!IsChasing) yield break;
            
            var runnerPos = Runner.transform.position;
            var pos = transform.position;
            var targetPos = new Vector3(currentWaypoint.x,pos.y,currentWaypoint.z);
            var dis = Vector3.Distance(pos, targetPos);
            
            if (dis < 0.1f)
            {
                targetIndex++;
                targetIndex = Mathf.Clamp(targetIndex, 0, Waypoints.Length);
                if (targetIndex == Waypoints.Length)
                {
                    yield break;
                }
                currentWaypoint = Waypoints[targetIndex];
            }
            var distance = Mathf.Abs(runnerPos.x - pos.x);
            var speed = Mathf.Clamp(Runner.Speed + distance - 1f, 0, CappedSpeed); // Speed related to distance of the runner
            var rotation = Quaternion.LookRotation(currentWaypoint - pos,Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);
            if (!FallStuck() && transform.position.y < 2f) transform.position = Vector3.MoveTowards(pos, targetPos,Time.deltaTime * speed);
            KillChaser();
            yield return null;
        }
    }
    public void ResetChaser()
    {
        RagdollController.TurnOffRagdolls();
        Animator.enabled = true;
        Collider.isTrigger = false;
        IsChasing = false;
        _isCatched = false;
    }
    public void KillChaser()
    {
        if (transform.position.y < -2f)
        {
            PoolManager.Instance.PoolObject("respawnChaser", this);
            EventManager.TriggerEvent("OnChaserDeath", new Dictionary<string, object>{{"chaser", this}});
        }
    }
    private bool FallStuck() // Fall object down when stuck
    {
        if (transform.position.y < 0.3f)
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
    private void StartChasing(Dictionary<string, object> message)
    {
        if (gameObject.activeInHierarchy && !IsChasing)
        {
            IsChasing = true;
            Animator.SetBool("IsRunning", true);
            StartCoroutine(Run());
        }
    }

    private void Update()
    {
        if (_isCatched)
        {
            Animator.SetBool("IsSliding", true);
            _slideTimer += Time.deltaTime;
            if (_slideTimer < 0.6f)
            {
                transform.position = Vector3.MoveTowards(transform.position, Runner.transform.position+Vector3.up+Vector3.right, Time.deltaTime*8f);
            }
            else if (_slideTimer > 1f)
            {
                Animator.enabled = false;
                Collider.isTrigger = true;
                RagdollController.TurnOnRagdolls();
                _isCatched = false;
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            _isCatched = true;
            IsChasing = false;
        }

        if (other.collider.CompareTag("Exit"))
        {
            IsChasing = false;
            Animator.enabled = false;
            Collider.isTrigger = true;
            RagdollController.TurnOnRagdolls();
        }
    }
    private void OnTriggerEnter(Collider other) // To detect runner after enabling isTrigger on runner
    {
        if (other.CompareTag("Player"))
        {
            _isCatched = true;
            IsChasing = false;
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening("OnChaseStart", StartChasing);
    }
    private void OnDisable()
    {
        EventManager.StopListening("OnChaseStart", StartChasing);
    }
}
