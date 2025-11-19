using System;
using System.Collections;
using JetBrains.Annotations;
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
        private bool canAttack;
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
    
        [CanBeNull] private PlayerUnit _targetUnit;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if(!IsServer) return;
            CurrentHealthPoints.OnValueChanged += HealthChanged;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            StartCoroutine(AttackCooldownCoroutine());
        }

        private void HealthChanged(float previousValue, float newValue)
        {
            if (newValue <= 0)
            {
                DespawnUnitServerRpc();
            }
        }

        private IEnumerator AttackCooldownCoroutine()
        {
            WaitForSeconds wait = new WaitForSeconds(AttackSpd);
            canAttack = true;
            while (true)
            {
                if (canAttack) continue;
                Debug.Log($"{gameObject.name}:{NetworkObjectId} is waiting {AttackSpd} seconds to attack again.");
                yield return wait;
                canAttack = true;
            }
        }

        private void Update()
        {
            if (IsServer){
                WalkForwardServerRpc();
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if(!other.gameObject.CompareTag("Unit")) return;
            other.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
            if(enemyUnit.OwnerClientId == OwnerClientId) return;
            if(_targetUnit == null || _targetUnit != enemyUnit)
                _targetUnit = enemyUnit;
            if (!canAttack) return;
            AttackEnemyUnitServerRpc();
        
        }
        private void MoveOrDie(Vector3 dir)
        {
            if (_targetUnit)
            {
                transform.Translate((_targetUnit.transform.position - transform.position).normalized * (0.2f * Time.deltaTime));
            }
            else
            {
                transform.Translate(dir * (0.2f * Time.deltaTime));
                var collider2Ds = Physics2D.OverlapCircleAll(transform.position, 10f);
                foreach (var coli in collider2Ds)
                {
                    if (!coli.gameObject.CompareTag("Unit")) continue;
                    coli.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
                    if (enemyUnit.OwnerClientId == OwnerClientId) continue;
                    _targetUnit = enemyUnit;
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void AttackEnemyUnitServerRpc()
        {
            if(!_targetUnit) return;
            _targetUnit.TakeDamageClientRpc(AttackDMG);
            canAttack = false;
            Debug.Log($"{gameObject.name} attacked {_targetUnit.gameObject.name} for {AttackDMG} damage.");
        }

        [ClientRpc]
        private void TakeDamageClientRpc(float damage)
        {
            if(!IsOwner) return;
            CurrentHealthPoints.Value -= damage;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void WalkForwardServerRpc(){
            MoveOrDie(OwnerClientId == 0 ? Vector3.right : Vector3.left);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DespawnUnitServerRpc()
        {
            Debug.LogWarning($"Despawned: {gameObject.name}:{NetworkObjectId}.");
            NetworkObject.Despawn();
        }
        
        [ClientRpc]
        public void SetStartHealthPointsClientRpc()
        {
            if (!IsOwner) return;
            CurrentHealthPoints.Value = MaxHealthPoints;
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
