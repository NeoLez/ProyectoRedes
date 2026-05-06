using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using Behaviour = Fusion.Behaviour;

public class Player : NetworkBehaviour, IExplodable
{
    private NetworkRigidbody3D _rb;
    [SerializeField] private float speed;
    [SerializeField] private TsarBomba bombPrefab;

    [Networked] public int PlayerID { get; private set; }
    
    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        
        PlayerID = Runner.LocalPlayer.PlayerId;
        GameManager.input.Player.DropBomb.performed += DropBomb;
        
        _rb = GetComponent<NetworkRigidbody3D>();
    }

    private void DropBomb(InputAction.CallbackContext _)
    {
        Runner.Spawn(bombPrefab, transform.position, Quaternion.identity);
    }
    
    private Vector2 moveDirection = Vector2.zero;
    private void Update()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        moveDirection = GameManager.input.Player.MoveDir.ReadValue<Vector2>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        _rb.Rigidbody.linearVelocity = moveDirection.Swizzle_x0y() * speed;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcKill()
    {
        Debug.Log(gameObject.name + " has been killed");
        GameManager.matchResult.RpcHandleMatchResult(PlayerID);
    }

    public Vector2Int GetTilePosition()
    {
        return GameManager.gridMap.GetTilePosition(transform.position);
    }

    public void Explode() {
        RpcKill();
    }

    public Behaviour Behaviour() {
        return this;
    }
}