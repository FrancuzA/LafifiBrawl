using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawningManager : NetworkBehaviour
{
    public static UnitSpawningManager Instance { get; private set; }

    [SerializeField] private GameObject[] unitPrefabs; // Assign your unit prefabs in inspector
    [SerializeField] private Transform[] playerSpawnAreas; // Assign spawn areas for both players

    private List<NetworkObject> spawnedUnits = new List<NetworkObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SpawnAllUnits()
    {
        if (Runner == null) return;

        // Clear any existing units
        ClearAllUnits();

        // Spawn player 1 units
        for (int i = 0; i < GamePhaseManager.Instance.Player1UnitCount; i++)
        {
            var unitData = GamePhaseManager.Instance.Player1Units[i];
            SpawnUnit(unitData, 0);
        }

        // Spawn player 2 units  
        for (int i = 0; i < GamePhaseManager.Instance.Player2UnitCount; i++)
        {
            var unitData = GamePhaseManager.Instance.Player2Units[i];
            SpawnUnit(unitData, 1);
        }
    }

    private void SpawnUnit(UnitData unitData, int playerIndex)
    {
        if (unitData.UnitPrefabId < 0 || unitData.UnitPrefabId >= unitPrefabs.Length) return;

        Vector3 spawnPosition = CalculateBattlePosition(unitData.GridPosition, playerIndex);
        var unitObject = Runner.Spawn(unitPrefabs[unitData.UnitPrefabId], spawnPosition, Quaternion.identity);

        // Set up unit ownership
        var unit = unitObject.GetComponent<Unit>();
        if (unit != null)
        {
            unit.SetOwner(unitData.OwnerPlayerId);
        }

        spawnedUnits.Add(unitObject);
    }

    private Vector3 CalculateBattlePosition(Vector2 gridPosition, int playerIndex)
    {
        Vector3 basePosition = playerSpawnAreas[playerIndex].position;

        // Convert backpack grid position to battlefield position
        // Adjust these values based on your game's scale
        float xOffset = gridPosition.x * 2f; // Adjust spacing as needed
        float zOffset = gridPosition.y * 2f;

        if (playerIndex == 1) // Mirror for player 2
        {
            xOffset = -xOffset;
        }

        return basePosition + new Vector3(xOffset, 0, zOffset);
    }

    private void ClearAllUnits()
    {
        foreach (var unit in spawnedUnits)
        {
            if (unit != null)
                Runner.Despawn(unit);
        }
        spawnedUnits.Clear();
    }
}