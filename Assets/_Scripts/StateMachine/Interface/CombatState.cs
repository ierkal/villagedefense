using _Scripts.Main;
using _Scripts.OdinAttributes;
using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
    [LogTag("CombatState")]
    public class CombatState : BaseGameState
    {
        private readonly CombatManager _combatManager;

        public CombatState(CombatManager combatManager)
        {
            _combatManager = combatManager;
        }

        public override void Enter()
        {
            base.Enter();
            /*_combatManager.StartCombatPhase();*/
        }

        public override void Exit()
        {
            /*_combatManager.EndCombatPhase();*/
            base.Exit();
        }

        public override void Tick() { }
    }
}
