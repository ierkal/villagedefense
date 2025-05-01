using System.Collections;
using System.Collections.Generic;
using _Scripts.AI.Player;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Island
{
    [LogTag("HexagonManager")]
    public class HexagonManager : MonoBehaviour, IGameService
    {
        [Title("Hex Settings")]
        public List<HexPrefabData> HexPrefabs = new();

        [MinValue(0.1f)]
        public float HexRadius = 1f;

        [PropertyRange(-10f, 10f)]
        public float TileYPosition = 0.4f;

        [Required]
        public Transform TileParent;

        [Title("Island Generation")]
        [MinValue(1)]
        public int StartingTileCount = 10;

        [ShowInInspector, ReadOnly]
        private Dictionary<Vector2Int, GameObject> _placedTiles = new();
        public Dictionary<Vector2Int, GameObject> PlacedTiles => _placedTiles;

        private void Start()
        {
            StartCoroutine(StartInOrder());
        }

        private IEnumerator StartInOrder()
        {
            var overlayUI = ServiceLocator.Instance.Get<OverlayUIManager>();
            bool islandReady = false;

            StartCoroutine(GenerateUntilReady(() => islandReady = true));

            yield return new WaitUntil(() => islandReady);

            StartCoroutine(PlayAllTilesSpawnAnimationsStaggered()); // ✨ Play VFX only ONCE, after successful generation
            overlayUI?.HideLoadingAnimation(); // now show to player
            /*
            SpawnStartingTroop();
            */

        }


        
        private IEnumerator GenerateUntilReady(System.Action onValidIsland)
        {
            do
            {
                GenerateConnectedRandomIsland();

                HexTile playerStartTile = GetClosestHexTile(Vector3.zero);
                if (!HasEnoughReachableTiles(playerStartTile)) 
                {
                    foreach (var go in _placedTiles.Values)
                        if (go != null) Destroy(go);
                    _placedTiles.Clear();
                    yield return null;
                }
                else
                {
                    ConnectNeighbors(); // must happen after final layout is locked
                    onValidIsland?.Invoke();
                    break;
                }
            } while (true);
        }
        /*private void SpawnStartingTroop()
        {
            var spawnTile = GetClosestHexTile(Vector3.zero); // Or smarter spawn later
            if (spawnTile != null)
            {
                ServiceLocator.Instance.Get<TroopSpawner>()?.SpawnStartingUnit(spawnTile.transform.position);
                Debug.Log("[HexagonManager] Spawned starting troop at " + spawnTile.GridPosition);
            }
            else
            {
                Debug.LogWarning("[HexagonManager] No spawn tile found to spawn troop!");
            }
        }*/

        private bool HasEnoughReachableTiles(HexTile startTile, int requiredCount = 6)
        {
            var reachable = ReachabilityChecker.GetReachableTiles(startTile);
            Log.Info(this, $"Reachability Check: {reachable.Count} tiles reachable.");
            return reachable.Count >= requiredCount;
        }

        private void GenerateConnectedRandomIsland()
        {
            _placedTiles.Clear();

            Vector2Int center = Vector2Int.zero;
            HexPrefabData buildablePrefab = HexPrefabs.Find(p => p.TileType == TileType.Grass || p.TileType == TileType.Forest);
            HexTile lastPlacedTile = PlaceHex(center, buildablePrefab);

            Queue<Vector2Int> frontier = new();
            HashSet<Vector2Int> visited = new() { center };

            foreach (var neighbor in GetNeighbors(center))
            {
                frontier.Enqueue(neighbor);
                visited.Add(neighbor);
            }

            while (_placedTiles.Count < StartingTileCount && frontier.Count > 0)
            {
                Vector2Int nextCoord = frontier.Dequeue();
                if (_placedTiles.ContainsKey(nextCoord)) continue;

                lastPlacedTile = PlaceHex(nextCoord);

                foreach (var neighbor in GetNeighbors(nextCoord))
                {
                    if (!visited.Contains(neighbor))
                    {
                        frontier.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            EnsureBuildableTileExists(lastPlacedTile);
            
            ConnectNeighbors();
        }

        private void EnsureBuildableTileExists(HexTile lastPlacedTile)
        {
            bool hasBuildable = false;

            foreach (var tileGO in _placedTiles.Values)
            {
                HexTile tile = tileGO.GetComponent<HexTile>();
                if (tile.IsBuildable)
                {
                    hasBuildable = true;
                    break;
                }
            }

            if (!hasBuildable && lastPlacedTile != null)
            {
                Vector2Int pos = lastPlacedTile.GridPosition;

                if (_placedTiles.TryGetValue(pos, out GameObject oldTile))
                {
                    Destroy(oldTile);
                    _placedTiles.Remove(pos);
                }

                HexPrefabData buildablePrefab = HexPrefabs.Find(p =>
                    p.TileType == TileType.Grass || p.TileType == TileType.Forest);

                if (buildablePrefab == null)
                {
                    Log.Warning(this, "No buildable prefab found in HexPrefabs!");
                    return;
                }

                PlaceHex(pos, buildablePrefab);
            }
        }

        public HexTile GetClosestHexTile(Vector3 worldPosition)
        {
            HexTile closestTile = null;
            float closestDistance = float.MaxValue;

            foreach (var tileGO in _placedTiles.Values)
            {
                if (tileGO == null) continue;

                float dist = Vector3.Distance(tileGO.transform.position, worldPosition);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTile = tileGO.GetComponent<HexTile>();
                }
            }

            return closestTile;
        }

        private HexTile PlaceHex(Vector2Int offsetCoord, HexPrefabData forcedPrefab = null)
        {
            if (_placedTiles.ContainsKey(offsetCoord)) return null;


            Vector3 worldPos = OffsetToWorld(offsetCoord);
            HexPrefabData prefabData = forcedPrefab ?? GetValidPrefab(offsetCoord);

            if (prefabData == null || prefabData.Prefab == null)
            {
                Log.Warning(this, "No valid prefab found for placement.");
                return null;
            }

            GameObject hex = Instantiate(prefabData.Prefab, worldPos, Quaternion.identity, TileParent);
            HexTile tile = hex.GetComponent<HexTile>();
            tile.GridPosition = offsetCoord;
            tile.TileType = prefabData.TileType;

            _placedTiles[offsetCoord] = hex;

            return tile;
        }

        private HexPrefabData GetValidPrefab(Vector2Int coord)
        {
            List<HexPrefabData> candidates = new(HexPrefabs);

            foreach (var neighbor in GetNeighbors(coord))
            {
                if (_placedTiles.TryGetValue(neighbor, out var neighborGO))
                {
                    HexTile neighborTile = neighborGO.GetComponent<HexTile>();
                    if (neighborTile.TileType == TileType.Mountain)
                    {
                        candidates.RemoveAll(p => p.TileType == TileType.Mountain);
                        break;
                    }
                }
            }

            if (candidates.Count == 0)
            {
                Log.Warning(this, "All prefabs were excluded. Allowing fallback to any.");
                candidates = new(HexPrefabs);
            }

            return candidates[Random.Range(0, candidates.Count)];
        }

        public Vector3 OffsetToWorld(Vector2Int offset)
        {
            float width = HexRadius * 2f;
            float height = Mathf.Sqrt(3f) * HexRadius;

            float x = width * 0.75f * offset.x;
            float z = height * (offset.y + (offset.x % 2 == 0 ? 0f : 0.5f));
            float y = TileYPosition;

            return new Vector3(x, y, z);
        }

        private Vector2Int[] GetNeighbors(Vector2Int coord)
        {
            bool even = coord.x % 2 == 0;

            return even
                ? new Vector2Int[]
                {
                    new(coord.x + 1, coord.y),
                    new(coord.x - 1, coord.y),
                    new(coord.x, coord.y + 1),
                    new(coord.x, coord.y - 1),
                    new(coord.x + 1, coord.y - 1),
                    new(coord.x - 1, coord.y - 1),
                }
                : new Vector2Int[]
                {
                    new(coord.x + 1, coord.y),
                    new(coord.x - 1, coord.y),
                    new(coord.x, coord.y + 1),
                    new(coord.x, coord.y - 1),
                    new(coord.x + 1, coord.y + 1),
                    new(coord.x - 1, coord.y + 1),
                };
        }
        private IEnumerator PlayAllTilesSpawnAnimationsStaggered()
        {
            foreach (var tileGO in _placedTiles.Values)
            {
                var animator = tileGO.GetComponent<HexSpawnAnimator>();
                if (animator != null)
                {
                    animator.PlaySpawnAnimation();
                    yield return new WaitForSeconds(0.03f); // Tiny delay between each
                }
            }
        }



        [Button("Show Expandable Tiles")]
        public List<Vector2Int> GetAvailableNeighborSpots()
        {
            HashSet<Vector2Int> candidates = new();

            foreach (var kvp in _placedTiles)
            {
                foreach (var neighbor in GetNeighbors(kvp.Key))
                {
                    if (!_placedTiles.ContainsKey(neighbor))
                        candidates.Add(neighbor);
                }
            }

            return new List<Vector2Int>(candidates);
        }

        public List<Vector3> GetEdgeSpawnPositions(float offsetDistance = 2f)
        {
            List<Vector3> spawnPositions = new();

            foreach (var coord in _placedTiles.Keys)
            {
                foreach (var neighbor in GetNeighbors(coord))
                {
                    if (!_placedTiles.ContainsKey(neighbor))
                    {
                        Vector3 dir = (OffsetToWorld(neighbor) - OffsetToWorld(coord)).normalized;
                        Vector3 spawnPos = OffsetToWorld(neighbor) + dir * offsetDistance;
                        spawnPositions.Add(spawnPos);
                    }
                }
            }

            return spawnPositions;
        }
        
        private void ConnectNeighbors()
        {
            foreach (var tileGO in _placedTiles.Values)
            {
                HexTile tile = tileGO.GetComponent<HexTile>();
                tile.Neighbors.Clear();

                foreach (var neighborCoord in GetNeighbors(tile.GridPosition))
                {
                    if (_placedTiles.TryGetValue(neighborCoord, out var neighborGO))
                    {
                        HexTile neighborTile = neighborGO.GetComponent<HexTile>();
                        tile.Neighbors.Add(neighborTile);
                    }
                }
            }

            Log.Info(this, "Neighbors connected.");
        }

    }
    
}
