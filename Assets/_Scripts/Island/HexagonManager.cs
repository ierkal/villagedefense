using System.Collections;
using System.Collections.Generic;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using Sirenix.OdinInspector;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Island
{
    [LogTag("HexagonManager")]
    public class HexagonManager : MonoBehaviour, IGameService
    {
        [Title("Hex Settings")]
        [Tooltip("Prefab + type metadata")]
        public List<HexPrefabData> HexPrefabs = new();

        [MinValue(0.1f)]
        [Tooltip("Radius of the hexagon. Affects spacing.")]
        public float HexRadius = 1f;

        [PropertyRange(-10f, 10f)]
        [Tooltip("Height of the tile above water level.")]
        public float TileYPosition = 0.4f;

        [Required, Tooltip("Parent transform for all generated hex tiles.")]
        public Transform TileParent;

        [Title("Island Generation")]
        [MinValue(1)]
        [Tooltip("Total number of hex tiles to generate on start.")]
        public int StartingTileCount = 10;

        
        [Title("Debug / Runtime")]
        [ShowInInspector, ReadOnly]
        private Dictionary<Vector2Int, GameObject> _placedTiles = new();
        public Dictionary<Vector2Int, GameObject> PlacedTiles => _placedTiles;

        private void Start()
        {
            StartCoroutine(StartInOrder());
        }

        private IEnumerator StartInOrder()
        {
            GenerateConnectedRandomIsland();
            yield return new WaitForSeconds(.2f);
            Log.Info(this, "A* Grid Re-scanned.");
        }
        private void GenerateConnectedRandomIsland()
        {
            _placedTiles.Clear();

            Vector2Int center = Vector2Int.zero;
            HexTile lastPlacedTile = PlaceHex(center);

            Queue<Vector2Int> frontier = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int> { center };

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

            // Exclude mountain if any neighbor is mountain
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
                candidates = new(HexPrefabs); // fallback: allow mountain anyway to avoid infinite loop
            }

            return candidates[Random.Range(0, candidates.Count)];
        }


        /*
        private HexPrefabData GetRandomHexPrefab()
        {
            if (HexPrefabs == null || HexPrefabs.Count == 0)
                return null;

            int index = Random.Range(0, HexPrefabs.Count);
            return HexPrefabs[index];
        }
        */

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

    }
    
}
