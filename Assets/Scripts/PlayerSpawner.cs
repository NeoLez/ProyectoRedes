using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] NetworkPrefabRef _playerPrefab;
    [SerializeField] private List<Transform> spawnPoints;
    private Player _player;
    private bool matchStarted;
    
    public void PlayerJoined(PlayerRef player)
    {
        if (!matchStarted && Runner.SessionInfo.PlayerCount >= spawnPoints.Count)
        {
            matchStarted = true;
            StartMatch();
        }
    }
    
    private void StartMatch()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        _player = Runner.Spawn(_playerPrefab, spawnPoints[Runner.LocalPlayer.PlayerId - 1].position, spawnPoints[Runner.LocalPlayer.PlayerId - 1].rotation).GetComponent<Player>();
        GameManager.player = _player;
    }
}