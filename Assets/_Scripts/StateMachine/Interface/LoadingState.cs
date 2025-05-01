using _Scripts.AI.Player;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.StateMachine.Interface;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.StateMachine.States
{
    [LogTag("LoadingState")]
    public class LoadingState : BaseGameState
    {
        private bool _servicesReady = false;

        public override void Enter()
        {
            base.Enter();
            Debug.Log("[LoadingState] Entered. Waiting for services...");
        }

        public override void Tick()
        {
            if (_servicesReady)
                return;

            bool allReady =
                ServiceLocator.Instance.TryGet<HexagonManager>(out var hexManager) &&
                ServiceLocator.Instance.TryGet<TroopSpawner>(out var troopSpawner);

            
            if (allReady)
            {
                _servicesReady = true;
                StateMachine.SetState(new IslandState());
            }
        }

        public override void Exit()
        {
            var hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            var troopSpawner = ServiceLocator.Instance.Get<TroopSpawner>();
            
            // ✅ Find the spawn location (center tile or any logic you define)
            var spawnTile = hexManager.GetClosestHexTile(Vector3.zero);
            if (spawnTile != null)
            {
                troopSpawner.SpawnStartingUnit(spawnTile.transform.position);
                Debug.Log("[LoadingState] Starting unit spawned.");
            }
            else
            {
                Debug.LogWarning("[LoadingState] No valid spawn tile found!");
            }

            base.Exit();
        }
    }
}
