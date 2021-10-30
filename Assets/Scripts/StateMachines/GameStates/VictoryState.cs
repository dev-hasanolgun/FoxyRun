using System.Collections.Generic;

public class VictoryState : IState<GameStateMachine>
{
    private readonly GameStateMachine _stateMachine;
    
    public VictoryState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public void Tick()
    {
        _stateMachine.GameManager.CameraController.VictoryCameraRotation();
    }

    public void OnStateEnter()
    {
        EventManager.StartListening("OnPlayLevel", PlayLevel);
        EventManager.StartListening("OnRestartLevel", RestartLevel);
        EventManager.TriggerEvent("OnEscape", null);
        _stateMachine.GameManager.GameUI.NextLevelButton.gameObject.SetActive(true);
    }

    public void OnStateExit()
    {
        EventManager.StopListening("OnPlayLevel", PlayLevel);
        EventManager.StopListening("OnRestartLevel", RestartLevel);
        EventManager.TriggerEvent("OnNextLevel", null);
        _stateMachine.GameManager.PoolChasers();
        _stateMachine.GameManager.PoolTraps();
        _stateMachine.GameManager.PoolObstacles();
        _stateMachine.GameManager.GameUI.NextLevelButton.gameObject.SetActive(false);
    }
    private void PlayLevel(Dictionary<string, object> message)
    {
        _stateMachine.GameManager.CurrentLevel++;
        _stateMachine.GameManager.Runner.ResetRunner();
        _stateMachine.SetState(new TutorialState(_stateMachine));
    }
    private void RestartLevel(Dictionary<string, object> message)
    {
        _stateMachine.GameManager.Runner.ResetRunner();
        _stateMachine.SetState(new TutorialState(_stateMachine));
    }
}
