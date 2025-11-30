using System;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Units Statistics", menuName = "Scriptable/Units Statistics")]
public class UnitsStats : ScriptableObject, INetworkSerializable
{
    [Header("Name and Image")]
    public string CharacterName;
    public LafifiImg lafifiImg;
    [Header("Health")]
    public ushort MaxHealthPoints;
    [Header("Attack")]
    public float AttackDMG;
    public ushort AttackSpd;
    [Header("Ult")]
    public UltID Ult;
    public ushort UltCD;
    public ushort UltCost;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CharacterName);
        serializer.SerializeValue(ref lafifiImg);
        serializer.SerializeValue(ref MaxHealthPoints);
        serializer.SerializeValue(ref AttackDMG);
        serializer.SerializeValue(ref AttackSpd);
        serializer.SerializeValue(ref Ult);
        serializer.SerializeValue(ref UltCD);
        serializer.SerializeValue(ref UltCost);
    }
}

public enum UltID
{
    Belly,
    Grzegorz,
    Kon,
    Lafifi,
    Angelika,
    Rat 
}

public enum LafifiImg
{
    Belly = 0,
    Grzegorz = 1,
    Kon = 2,
    Lafifi = 3,
    Angelika = 4,
    Rat = 5
}

public enum LafifiIndex
{
    Belly = 0,
    Grzegorz = 1,
    Kon = 2,
    Lafifi = 3,
    Angelika = 4,
    Rat = 5
}