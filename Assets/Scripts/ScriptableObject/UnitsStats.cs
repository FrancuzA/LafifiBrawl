using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Units Statistics", menuName = "Scriptable/Units Statistics")]
public class UnitsStats : ScriptableObject
{
    [Header("Name and Image")]
    public string CharacterName;
    public Sprite CharacterImage;
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
    Lafifi = 0,
    Kon = 1,
    Grzegorz = 2,
    Belly = 3,
    Angelika = 4,
    Rat = 5
}