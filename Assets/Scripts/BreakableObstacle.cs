using Fusion;
using UnityEngine;
using Behaviour = Fusion.Behaviour;

public class BreakableObstacle : NetworkBehaviour, IExplodable{
    
    public void Explode() {
        RpcDestroy();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcDestroy() {
        Runner.Despawn(Object);
    }

    public Behaviour Behaviour() {
        return this;
    }
}