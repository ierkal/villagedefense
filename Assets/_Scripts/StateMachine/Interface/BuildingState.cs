using _Scripts.Island;
using _Scripts.OdinAttributes;
using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
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
            /*_buildManager.EnableBuildMode();*/
        }

        public override void Exit()
        {
            /*_buildManager.DisableBuildMode();*/
            base.Exit();
        }

        public override void Tick() { }
    }
}
