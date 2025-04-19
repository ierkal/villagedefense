using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
    public class UIOverlayState : IGameState
    {
        protected readonly Canvas UiCanvas;

        public UIOverlayState(Canvas uiCanvas)
        {
            UiCanvas = uiCanvas;
        }

        public virtual void Enter()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Exit()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Tick()
        {
            throw new System.NotImplementedException();
        }
    }

    public class PauseMenuState : UIOverlayState
    {
        public PauseMenuState(Canvas uiCanvas) : base(uiCanvas)
        {
        }
        public override void Enter()
        {
            base.Enter();
            Time.timeScale = 0f; // freeze gameplay
            Debug.Log("Pause Menu Opened");
        }

        public override void Exit()
        {
            Time.timeScale = 1f; // resume
            base.Exit();
            Debug.Log("Pause Menu Closed");
        }
    }
}