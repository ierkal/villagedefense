using _Scripts.Input;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.StateMachine.Interface;
using _Scripts.Utility;

[LogTag("BuildingState")]
public class BuildingState : BaseGameState
{
    private readonly BuildManager _buildManager;

    public BuildingState(BuildManager buildManager)
    {
        _buildManager = buildManager;
    }

    public override void Enter()
    {
        base.Enter();
        

    }


    public override void Exit()
    {
        base.Exit();
    }

    public override void Tick() { }
}