using Fusion;
using UnityEngine;

[System.Serializable]
public struct UnitData : INetworkStruct
{
    public int UnitPrefabId;
    public Vector2 GridPosition;
    public int OwnerPlayerId;

    public UnitData(int prefabId, Vector2 gridPos, int ownerId)
    {
        UnitPrefabId = prefabId;
        GridPosition = gridPos;
        OwnerPlayerId = ownerId;
    }
}