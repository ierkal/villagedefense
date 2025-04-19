using _Scripts.AI.Enemies;
using _Scripts.Island;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Event
{
    public class ServicesInitializedEvent: GameEvent
    {
        
    }
    public class SceneLoadedEvent : GameEvent
    {
        public Scene Scene { get; }

        public SceneLoadedEvent(Scene scene)
        {
            Scene = scene;
        }
    }
    
    public class WaveStartedEvent : GameEvent
    {
        public int WaveNumber { get; }

        public WaveStartedEvent(int waveNumber)
        {
            WaveNumber = waveNumber;
        }
    }

    public class WaveEndedEvent : GameEvent
    {
        public int WaveNumber { get; }

        public WaveEndedEvent(int waveNumber)
        {
            WaveNumber = waveNumber;
        }
    }
    
    public class EnemyDiedEvent : GameEvent
    {
        public GameObject Enemy { get; }

        public EnemyDiedEvent(GameObject enemy)
        {
            Enemy = enemy;
        }
    }
    public class ShipArrivedEvent : GameEvent
    {
        public EnemyShip Ship { get; }
        public ShipArrivedEvent(EnemyShip ship) => Ship = ship;
    }

    public class TroopDisembarkedEvent : GameEvent
    {
        public GameObject Troop { get; }
        public HexTile LandingTile { get; }

        public TroopDisembarkedEvent(GameObject troop, HexTile landingTile)
        {
            Troop = troop;
            LandingTile = landingTile;
        }
    }

    public class TroopPackDeployedEvent : GameEvent
    {
        public EnemyShip Ship { get; }
        public TroopPackDeployedEvent(EnemyShip ship) => Ship = ship;
    }
}