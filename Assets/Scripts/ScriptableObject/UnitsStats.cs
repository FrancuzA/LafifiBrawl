using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Units Statistics", menuName = "Scriptable/Units Statistics")]
public class UnitsStats : ScriptableObject
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