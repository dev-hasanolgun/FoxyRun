using UnityEngine;

public class TutorialState : IState<GameStateMachine>
{
    private readonly GameStateMachine _stateMachine;
    
    public TutorialState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public void Tick()
    {
        _stateMachine.GameManager.CameraController.FocusRunner();
        if (Input.GetKeyDown(KeyCode.A))
        {
            _stateMachine.SetState(new GameState(_stateMachine));
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _stateMachine.SetState(new GameState(_stateMachine));
            }
        }
    }

    public void OnStateEnter()
    {
        _stateMachine.GameManager.GameUI.TutorialUI.gameObject.SetActive(true);
        _stateMachine.GameManager.InitializeLevel();
        _stateMachine.GameManager.GameUI.CurrentLevelText.text = "Level " + (_stateMachine.GameManager.CurrentLevel + 1).ToString("0");
    }

    public void OnStateExit()
    {
        _stateMachine.GameManager.GameUI.TutorialUI.gameObject.SetActive(false);
    }
}
