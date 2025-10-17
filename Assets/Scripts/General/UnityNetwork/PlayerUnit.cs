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
    
    [Header("Look")]
    private SpriteRenderer spriteRenderer;
    
    public Sprite[] sprites = new Sprite[3];
    [Header("Health")]
    public ushort MaxHealthPoints;
    private NetworkVariable<ushort> CurrentHealthPoints = new ();
    [Header("Attack")]
    public float AttackDMG;
    public ushort AttackSpd;
    [Header("Ult")]
    public UltID Ult;
    public ushort UltCD;
    public ushort UltCost;
    
    private PlayerUnit targetUnit;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (IsServer){
            WalkForwardServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void WalkForwardServerRpc(){
        if(OwnerClientId == 0)
        {
            if(targetUnit == null){
                transform.Translate(Vector3.right * (0.2f * Time.deltaTime));
                var collider2Ds = Physics2D.OverlapCircleAll(transform.position, 1f);
                foreach (var coli in collider2Ds)
                {
                    if (!coli.gameObject.CompareTag("Unit")) continue;
                    coli.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
                    if (enemyUnit.OwnerClientId == OwnerClientId) continue;
                    targetUnit = enemyUnit;
                    break;
                }
            }
            else
            {
                transform.Translate((targetUnit.transform.position - transform.position).normalized * (0.2f * Time.deltaTime));
            }
            
        } else {
            if(targetUnit == null){
                transform.Translate(Vector3.left * (0.2f * Time.deltaTime));
                var collider2Ds = Physics2D.OverlapCircleAll(transform.position, 1f);
                foreach (var coli in collider2Ds)
                {
                    if (!coli.gameObject.CompareTag("Unit")) continue;
                    coli.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
                    if (enemyUnit.OwnerClientId == OwnerClientId) continue;
                    targetUnit = enemyUnit;
                    break;
                }
            }
            else
            {
                transform.Translate((targetUnit.transform.position - transform.position).normalized * (0.2f * Time.deltaTime));
            }
        }
        
    }
        

    [ClientRpc]
    public void SetStatsClientRpc(string characterName, LafifiImg lafifiImg, ushort maxHealthPoints, 
        float attackDMG, ushort attackSpd, UltID ult, ushort ultCD, ushort ultCost) 
    {
        gameObject.name = characterName;
        spriteRenderer.sprite = sprites[(int)lafifiImg];
        MaxHealthPoints = maxHealthPoints;
        //CurrentHealthPoints.Value = maxHealthPoints;
        AttackDMG = attackDMG;
        AttackSpd = attackSpd;
        Ult = ult;
        UltCD = ultCD;
        UltCost = ultCost;
    }
    
    [ClientRpc]
    public void SetColorClientRpc(ulong unitOwnerId)
    {
        spriteRenderer.color = unitOwnerId == NetworkManager.Singleton.LocalClientId ? Color.green : Color.red;
    }
}
