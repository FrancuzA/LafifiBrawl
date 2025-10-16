using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerUnit : NetworkBehaviour
{
    public UnitsStats Stats = null;
    
    [Header("Name and Image")]
    public string CharacterName;
    public SpriteRenderer spriteRenderer;
    [Header("Health")]
    public ushort MaxHealthPoints;
    [Header("Attack")]
    public float AttackDMG;
    public ushort AttackSpd;
    [Header("Ult")]
    public UltID Ult;
    public ushort UltCD;
    public ushort UltCost;
    

    public override void OnNetworkSpawn()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(SetStats());
    }

    private IEnumerator SetStats()
    {
        yield return Stats != null;
        CharacterName = Stats.CharacterName;
        spriteRenderer.sprite = Stats.CharacterImage;
        MaxHealthPoints = Stats.MaxHealthPoints;
        AttackDMG = Stats.AttackDMG;
        AttackSpd = Stats.AttackSpd;
        Ult = Stats.Ult;
        UltCD = Stats.UltCD;
        UltCost = Stats.UltCost;

        spriteRenderer.color = NetworkObject.IsOwner ? Color.green : Color.red;
    }
}
