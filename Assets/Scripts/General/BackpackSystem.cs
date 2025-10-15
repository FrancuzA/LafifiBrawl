using System.Collections.Generic;
using UnityEngine;

public class BackpackSystem : MonoBehaviour
{
    private List<UnitData> placedUnits = new List<UnitData>();

    public void PlaceUnit(int prefabId, Vector2 gridPosition)
    {
        // Add unit to your backpack
        BasicSpawner basicSpawner = Dependencies.Instance.GetDependancy<BasicSpawner>();
        int playerId = basicSpawner._runner.LocalPlayer.PlayerId;
        placedUnits.Add(new UnitData(prefabId, gridPosition, playerId));
    }

    public List<UnitData> GetPlacedUnits()
    {
        return new List<UnitData>(placedUnits);
    }
}