using System.Collections.Generic;
using UnityEngine;
using _Scripts.Main.Services;
using _Scripts.Island;
using _Scripts.Event;
using _Scripts.OdinAttributes;
using _Scripts.Utility;

namespace _Scripts.Combat
{
    [LogTag("TileThreatManager")]
    public class TileThreatManager : MonoBehaviour, IGameService
    {
        private HashSet<HexTile> _threatenedTiles = new();

        private void OnEnable()
        {
            EventBroker.Instance.AddEventListener<TroopDisembarkedEvent>(OnTroopDisembarked);
        }

        private void OnDisable()
        {
            EventBroker.Instance.RemoveEventListener<TroopDisembarkedEvent>(OnTroopDisembarked);
        }

        private void OnTroopDisembarked(TroopDisembarkedEvent e)
        {
            if (_threatenedTiles.Add(e.LandingTile))
            {
                var indicator = e.LandingTile.GetComponent<ThreatIndicator>();
                if (indicator != null)
                {
                    indicator.SetThreatActive(true);
                }

                Log.Info(this, $"Marked tile {e.LandingTile.GridPosition} as threatened.", "red");
            }
        }


        public void UnmarkTile(HexTile tile)
        {
            if (_threatenedTiles.Remove(tile))
            {
                var indicator = tile.GetComponent<ThreatIndicator>();
                if (indicator != null)
                {
                    indicator.SetThreatActive(false);
                }

                Log.Info(this, $"Unmarked tile {tile.GridPosition}.", "green");
            }
        }


        public bool IsTileThreatened(HexTile tile) => _threatenedTiles.Contains(tile);

        public List<HexTile> GetThreatenedTiles() => new(_threatenedTiles);
    }
}