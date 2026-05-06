using DefaultNamespace;
using Fusion;
using UnityEngine;
using Behaviour = Fusion.Behaviour;

public class BreakableObstacle : NetworkBehaviour, IExplodable{
    [SerializeField] private SpeedPowerup speedPowerup;
    public void Explode() {
        RpcDestroy();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcDestroy() {
        Runner.Spawn(speedPowerup, transform.position);
        Runner.Despawn(Object);
    }

    public Behaviour Behaviour() {
        return this;
    }
}