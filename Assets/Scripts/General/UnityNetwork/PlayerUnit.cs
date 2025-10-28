using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace General.UnityNetwork
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerUnit : NetworkBehaviour
    {
        public UnitsStats stats = null;
    
        [Header("Look")]
        private SpriteRenderer _spriteRenderer;
    
        public Sprite[] sprites = new Sprite[3];
        [Header("Health")]
        public ushort MaxHealthPoints;
        private NetworkVariable<float> CurrentHealthPoints = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        [Header("Attack")]
        public float AttackDMG;
        public ushort AttackSpd;
        [Header("Ult")]
        public UltID Ult;
        public ushort UltCD;
        public ushort UltCost;
    
        private PlayerUnit _targetUnit;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (IsServer){
                WalkForwardServerRpc();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(!other.gameObject.CompareTag("Unit")) return;
            other.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
            if(enemyUnit.OwnerClientId == OwnerClientId) return;
            Attack(enemyUnit);
        
        }

        private void Attack(PlayerUnit enemyUnit)
        {
            enemyUnit.TakeDamageClientRpc(AttackDMG);
        }

        [ServerRpc(RequireOwnership = false)]
        private void WalkForwardServerRpc(){
            if(OwnerClientId == 0)
            {
                if(!_targetUnit){
                    transform.Translate(Vector3.right * (0.2f * Time.deltaTime));
                    var collider2Ds = Physics2D.OverlapCircleAll(transform.position, 1f);
                    foreach (var coli in collider2Ds)
                    {
                        if (!coli.gameObject.CompareTag("Unit")) continue;
                        coli.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
                        if (enemyUnit.OwnerClientId == OwnerClientId) continue;
                        _targetUnit = enemyUnit;
                        break;
                    }
                }
                else
                {
                    transform.Translate((_targetUnit.transform.position - transform.position).normalized * (0.2f * Time.deltaTime));
                }
            
            } else {
                if(!_targetUnit){
                    transform.Translate(Vector3.left * (0.2f * Time.deltaTime));
                    var collider2Ds = Physics2D.OverlapCircleAll(transform.position, 1f);
                    foreach (var coli in collider2Ds)
                    {
                        if (!coli.gameObject.CompareTag("Unit")) continue;
                        coli.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
                        if (enemyUnit.OwnerClientId == OwnerClientId) continue;
                        _targetUnit = enemyUnit;
                        break;
                    }
                }
                else
                {
                    transform.Translate((_targetUnit.transform.position - transform.position).normalized * (0.2f * Time.deltaTime));
                }
            }
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void DespawnUnitServerRpc(){
            NetworkObject.Despawn();
        }
    
        [ClientRpc]
        public void TakeDamageClientRpc(float damage)
        {
            if(!IsOwner) return;
            CurrentHealthPoints.Value -= damage;
            if (CurrentHealthPoints.Value <= 0)
            {
                DespawnUnitServerRpc();
            }
        }
        

        [ClientRpc]
        public void SetStatsClientRpc(string characterName, LafifiImg lafifiImg, ushort maxHealthPoints, 
            float attackDmg, ushort attackSpd, UltID ult, ushort ultCd, ushort ultCost) 
        {
            gameObject.name = characterName;
            _spriteRenderer.sprite = sprites[(int)lafifiImg];
            MaxHealthPoints = maxHealthPoints;
            AttackDMG = attackDmg;
            AttackSpd = attackSpd;
            Ult = ult;
            UltCD = ultCd;
            UltCost = ultCost;
        }
    
        [ClientRpc]
        public void SetColorClientRpc(ulong unitOwnerId)
        {
            _spriteRenderer.color = unitOwnerId == NetworkManager.Singleton.LocalClientId ? Color.green : Color.red;
        }
    }
}
