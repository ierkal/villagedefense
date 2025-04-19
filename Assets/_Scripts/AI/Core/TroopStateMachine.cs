using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.AI.Core
{
    [LogTag("TroopStateMachine")]
    public class TroopStateMachine : MonoBehaviour
    {
        private IUnitState _currentState;

        public IUnitState CurrentState => _currentState;

        public void SetState(IUnitState newState)
        {
            if (_currentState != null)
            {
                Log.Info(this, $"Exiting {_currentState.GetType().Name}", "gray");
                _currentState.Exit();
            }

            _currentState = newState;

            if (_currentState != null)
            {
                Log.Info(this, $"Entering {_currentState.GetType().Name}", "white");
                _currentState.Enter();
            }
        }

        private void Update()
        {
            _currentState?.Tick();
        }
    }
}