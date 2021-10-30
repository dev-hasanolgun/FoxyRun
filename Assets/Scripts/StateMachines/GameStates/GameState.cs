using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : IState<GameStateMachine>
{
    private readonly GameStateMachine _stateMachine;
    private Runner _runner;
    public GameState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public void Tick()
    {
        _stateMachine.GameManager.StartSpawning();
        _stateMachine.GameManager.CameraController.FocusRunner();
        _stateMachine.GameManager.RespawnChasers();
        
        _runner.CharacterController.MoveCharacter();

        var distance = Mathf.Abs(_runner.transform.position.x - _runner.Target.x);
        if (distance < 0.5f) // Victory if near finish point
        {
            _stateMachine.SetState(new VictoryState(_stateMachine));
        }
        else if (distance < 6.5f) // Escaped if passed exit door
        {
            _runner.tag = "Untagged";
        }
        
        if (_runner.transform.position.y < -20f) // Death on falling down
        {
            _runner.Collider.attachedRigidbody.isKinematic = true;
            _stateMachine.SetState(new GameOverState(_stateMachine));
        }
    }
    
    public void OnStateEnter()
    {
        _stateMachine.GameManager.IsChasing = true;
        
        _runner = _stateMachine.GameManager.Runner;
        _runner.Animator.SetBool("IsRunning", true);
        _runner.tag = "Player";
        
        EventManager.TriggerEvent("OnChaseStart", null);
        EventManager.StartListening("OnCaught", GotCaught);
        EventManager.StartListening("OnChaserDeath", ChaserDeath);
    }
    
    public void OnStateExit()
    {
        _stateMachine.GameManager.IsChasing = false;
        
        EventManager.StopListening("OnCaught", GotCaught);
        EventManager.StopListening("OnChaserDeath", ChaserDeath);
    }
    
    private void GotCaught(Dictionary<string, object> message)
    {
        _stateMachine.SetState(new GameOverState(_stateMachine));
    }
    private void ChaserDeath(Dictionary<string,object> message)
    {
        var chaser = (Chaser) message["chaser"];
        _stateMachine.GameManager.ActiveChasers.Remove(chaser);
        chaser.gameObject.SetActive(false);
    }
}