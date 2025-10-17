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
    public UnitsStats Stats;
    
    private SpriteRenderer spriteRenderer;
    [Header("Health")]
    public ushort MaxHealthPoints;
    [Header("Attack")]
    public float AttackDMG;
    public ushort AttackSpd;
    [Header("Ult")]
    public UltID Ult;
    public ushort UltCD;
    public ushort UltCost;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    [ClientRpc]
    public void SetColorClientRpc(ulong unitOwnerId)
    {
        spriteRenderer.color = unitOwnerId == NetworkManager.Singleton.LocalClientId ? Color.green : Color.red;
    }
}
