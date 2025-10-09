using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class GamePhaseManager : NetworkBehaviour
{
    public static GamePhaseManager Instance { get; private set; }

    [Networked] public GamePhase CurrentPhase { get; set; }
    [Networked] public int PlayersReady { get; set; }

    // Store unit lists for both players
    [Networked, Capacity(20)] public NetworkArray<UnitData> Player1Units { get; }
    [Networked, Capacity(20)] public NetworkArray<UnitData> Player2Units { get; }

    [Networked] public int Player1UnitCount { get; set; }
    [Networked] public int Player2UnitCount { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public override void Spawned()
    {
        CurrentPhase = GamePhase.Buying;
        PlayersReady = 0;
    }

    // RPC to signal player is ready
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerReady(RpcInfo info = default)
    {
        PlayersReady++;

        if (PlayersReady == 2) // Both players ready
        {
            StartBattlePhase();
        }
    }

    private void StartBattlePhase()
    {
        CurrentPhase = GamePhase.Battle;
        RPC_DeployAllUnits();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DeployAllUnits()
    {
        // This will be called on all clients to spawn units
        UnitSpawningManager.Instance.SpawnAllUnits();
    }
}

public enum GamePhase
{
    Buying,
    Battle
}