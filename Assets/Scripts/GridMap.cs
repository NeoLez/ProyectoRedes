using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GridMap : NetworkBehaviour
{
    [Serializable]
    public class PosState
    {
        public Vector2Int position;
        public TileState state;
    } 
    [SerializeField] public List<PosState> _posStates = new() ;
    [SerializeField] GameObject occupiedPrefab;
    [SerializeField] BreakableObstacle breakablePrefab;
        
    private Dictionary<Vector2Int, TileState> TileStates = new();

    public override void Spawned() {
        GameManager.gridMap = this;
        PopulateMapDictionary();
    }

    private void PopulateMapDictionary()
    {
        foreach (var posState in _posStates)
        {
            if (posState.state == TileState.Free) continue;
                
            TileStates[posState.position] = posState.state;
            if(posState.state == TileState.Occupied)
                Instantiate(occupiedPrefab, new Vector3(posState.position.x, 0, posState.position.y), Quaternion.identity, transform);
            else if (HasStateAuthority) {
                var o = Runner.Spawn(breakablePrefab, new Vector3(posState.position.x, 0, posState.position.y), Quaternion.identity);
                o.transform.SetParent(transform);
            }
        }
    }

    public TileState GetTileState(Vector2Int pos)
    {
        return TileStates.TryGetValue(pos, out TileState state) ? state : TileState.Free;
    }

    public Vector2Int GetTilePosition(Vector2 pos)
    {
        return new Vector2Int((int)Math.Round(pos.x), (int)Math.Round(pos.y));
    }
        
    public Vector2Int GetTilePosition(Vector3 pos)
    {
        return new Vector2Int((int)Math.Round(pos.x), (int)Math.Round(pos.z));
    }

    public void ToggleTileEditor(Vector2Int pos) {
        var posState = _posStates.Find(p => p.position == pos);
        if (posState == null) {
            _posStates.Add(new PosState { position = pos, state = TileState.Occupied });
        } else if (posState.state == TileState.Occupied) {
            posState.state = TileState.Breakable;
        }
        else {
            _posStates.RemoveAll(p => p.position == pos);
        }
    }
}

public enum TileState
{ 
    Free,
    Occupied,
    Breakable,
}