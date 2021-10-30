using System.Collections.Generic;

public class GameOverState : IState<GameStateMachine>
{
    private readonly GameStateMachine _stateMachine;
    
    public GameOverState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Tick()
    {
        
    }

    public void OnStateEnter()
    {
        EventManager.StartListening("OnRestartLevel", RestartLevel);
        _stateMachine.GameManager.GameUI.RestartButton.gameObject.SetActive(true);
    }

    public void OnStateExit()
    {
        EventManager.StopListening("OnRestartLevel", RestartLevel);
        _stateMachine.GameManager.PoolChasers();
        _stateMachine.GameManager.PoolTraps();
        _stateMachine.GameManager.PoolObstacles();
        _stateMachine.GameManager.GameUI.RestartButton.gameObject.SetActive(false);
    }
    private void RestartLevel(Dictionary<string, object> message)
    {
        _stateMachine.GameManager.Runner.ResetRunner();
        _stateMachine.SetState(new TutorialState(_stateMachine));
    }
}
