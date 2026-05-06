using Fusion;
using UnityEngine;
public class MatchResult : NetworkBehaviour {
    [SerializeField] private GameObject WinnerUI;
    [SerializeField] private GameObject LoserUI;

    public override void Spawned() {
        GameManager.matchResult = this;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcHandleMatchResult(int deadID) {
        if(deadID == Runner.LocalPlayer.PlayerId) {
            LoserUI.SetActive(true);
        }
        else {
            WinnerUI.SetActive(true);
        }
    }
}