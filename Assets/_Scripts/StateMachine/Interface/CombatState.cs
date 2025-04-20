using _Scripts.Main;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
    [LogTag("CombatState")]
    public class CombatState : BaseGameState
    {
        private readonly CombatManager _combatManager;
        private UnitSelectionClickHandler _unitClickHandler;
        
        public CombatState(CombatManager combatManager)
        {
            _combatManager = combatManager;
        }

        public override void Enter()
        {
            base.Enter();
            _unitClickHandler ??= new UnitSelectionClickHandler();
            ServiceLocator.Instance.Get<ClickRouter>()?.SetClickHandler(_unitClickHandler);
            /*_combatManager.StartCombatPhase();*/
        }

        public override void Exit()
        {
            /*_combatManager.EndCombatPhase();*/
            ServiceLocator.Instance.Get<ClickRouter>()?.ClearClickHandler();

            base.Exit();
        }

        public override void Tick() { }
    }
}
