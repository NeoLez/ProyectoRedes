using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Utils;

public class TsarBomba : NetworkBehaviour
{
    [SerializeField] private float fuseTime;
    [SerializeField] private float explosionPropagationDelay;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private TileExplosionVisuals tileExplosionVisualsPrefab;
    [SerializeField] private NetworkMecanimAnimator animator;   
    private bool _exploded;
    private TickTimer _explosionTimer;
    private float _explosionTime;
    
    private struct PendingExplosion
    {
        public Vector2Int position;
        public TickTimer timer;
    }

    private List<PendingExplosion> _pendingExplosions = new();
    private HashSet<int> _objectsHit = new();

    public override void Spawned() {
        if (HasStateAuthority) {
            _explosionTimer = TickTimer.CreateFromSeconds(Runner, fuseTime);
            _explosionTime = Time.time + fuseTime;
        }
    }

    public override void FixedUpdateNetwork() {
        animator.Animator.SetFloat("Time", Mathf.Min((_explosionTime - Time.time) / fuseTime, 1.0f));
        if (!_exploded) {
            if (_explosionTimer.Expired(Runner)) {
                animator.Animator.SetBool("Exploded", true);
                _exploded = true;
                Explode(); 
                RpcExplodeVisuals();
            }
            else return;
        }
        
        if (_pendingExplosions.Count == 0) {
            Runner.Despawn(Object);
        }
        
        for (int i = _pendingExplosions.Count - 1; i >= 0; i--) {
            var pendingExplosion = _pendingExplosions[i];
            if (pendingExplosion.timer.ExpiredOrNotRunning(Runner)) {
                _pendingExplosions.RemoveAt(i);
                RpcExplodeTile(pendingExplosion.position);
                var colliders = Physics.OverlapBox(pendingExplosion.position.ToVector2().Swizzle_x0y(), new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, hitLayer);
                foreach (var collider in colliders) {
                    var obj = collider.GetComponent<IExplodable>();
                    
                    if (_objectsHit.Contains(obj.Behaviour().GetInstanceID())) continue;
                    _objectsHit.Add(obj.Behaviour().GetInstanceID());
                    obj.Explode();
                }
            } 
            RpcSpawnTileVisuals(pendingExplosion.position);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSpawnTileVisuals(Vector2Int position) {
        
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcExplodeVisuals() {
        Debug.Log("ExplodeVisuals");
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcExplodeTile(Vector2Int position) {
        Instantiate(tileExplosionVisualsPrefab, position.ToVector2().Swizzle_x0y(), Quaternion.identity);
    }

    public void Explode() {
        Debug.Log("Explode");
        var map = GameManager.gridMap;
        var startingPosition = GameManager.gridMap.GetTilePosition(transform.position);
        var directions = new [] {Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};

        _pendingExplosions.Add(new PendingExplosion
        {
            position = startingPosition,
            timer = TickTimer.CreateFromSeconds(Runner, 0)
        });
        foreach (var direction in directions)
        {
            int i = 1;

            while (true)
            {
                var currentPosition = startingPosition + direction * i;

                if (map.GetTileState(currentPosition) == TileState.Occupied)
                    break;

                _pendingExplosions.Add(new PendingExplosion
                {
                    position = currentPosition,
                    timer = TickTimer.CreateFromSeconds(Runner, explosionPropagationDelay * i)
                });

                if (map.GetTileState(currentPosition) == TileState.Breakable)
                    break;

                i++;
            }
        }
    }
}