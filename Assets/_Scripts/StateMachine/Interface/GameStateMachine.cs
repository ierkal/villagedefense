using System.Collections.Generic;

namespace _Scripts.StateMachine.Interface
{
    public class GameStateMachine
    {
        private readonly Stack<IGameState> _stateStack = new();

        public IGameState CurrentState => _stateStack.Count > 0 ? _stateStack.Peek() : null;

        public void PushState(IGameState newState)
        {
            CurrentState?.Exit(); // pause current
            _stateStack.Push(newState);
            newState.Enter();
        }

        public void PopState()
        {
            if (_stateStack.Count == 0) return;

            _stateStack.Pop().Exit();
            CurrentState?.Enter(); // resume previous
        }

        public void SetState(IGameState newState)
        {
            while (_stateStack.Count > 0)
            {
                _stateStack.Pop().Exit();
            }

            _stateStack.Push(newState);
            newState.Enter();
        }

        public void Tick()
        {
            CurrentState?.Tick();
        }
    
        /*PushState()	Temporary overlays: pause menus, popups, targeting modes
        PopState()	Exiting temporary state, resume one underneath
        SetState()	Changing the entire flow (like going from MainMenu → GameOver)
    Tick()	Delegates per-frame updates to current top-of-stack state*/
    }
}