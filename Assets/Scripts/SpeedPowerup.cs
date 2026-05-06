using Fusion;
using UnityEngine;

namespace DefaultNamespace {
    public class SpeedPowerup : NetworkBehaviour {
        [SerializeField] public float powerupTime;
        [SerializeField] public float powerupSpeedPercentage;

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RpcConsume() {
            Runner.Despawn(Object);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out Player player)) {
                if (!player.HasStateAuthority) return;
                player.RpcGivePowerup(powerupTime, powerupSpeedPercentage);
                RpcConsume();
            }
        }
    }
}