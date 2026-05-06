using System;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [Serializable]
    public class PosState
    {
        public Vector2Int position;
        public TileState state;
    } 
    [SerializeField] public List<PosState> _posStates = new() ;
    [SerializeField] GameObject occupiedPrefab;
        
    private Dictionary<Vector2Int, TileState> TileStates = new();

    private void Awake()
    {
        PopulateMapDictionary();
        GameManager.gridMap = this;
    }

    private void PopulateMapDictionary()
    {
        foreach (var posState in _posStates)
        {
            if (posState.state == TileState.Free) continue;
                
            TileStates[posState.position] = posState.state;
            Instantiate(occupiedPrefab, new Vector3(posState.position.x, 0, posState.position.y), Quaternion.identity, transform);
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

    public void ToggleTile(Vector2Int pos)
    {
        if (TileStates.ContainsKey(pos))
        {
            TileStates.Remove(pos);
            _posStates.RemoveAll(p => p.position == pos);
        }
        else
        {
            TileStates[pos] = TileState.Occupied;
            _posStates.Add(new PosState { position = pos, state = TileState.Occupied });
        }
    }
}

public enum TileState
{ 
    Free,
    Occupied,
}