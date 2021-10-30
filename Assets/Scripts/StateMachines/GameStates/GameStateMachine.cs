public class GameStateMachine : BaseStateMachine<GameStateMachine>
{
    public readonly GameManager GameManager;

    public GameStateMachine(GameManager gameManager)
    {
        GameManager = gameManager;
    }
}