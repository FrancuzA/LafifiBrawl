using System;
using System.Collections.Generic;
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
        [SerializeField] private float attackCooldownTimer;
        private const float Speed = 5f;
        
        [Header("Look")]
        private SpriteRenderer _spriteRenderer;
    
        [Tooltip("Belly = 0,\nGrzegorz = 1,\nKon = 2,\nLafifi = 3,\nAngelika = 4,\nRat = 5")]
        public List<Sprite> sprites = new ();
        [Header("Health")]
        public ushort MaxHealthPoints;
        [SerializeField] private NetworkVariable<float> currentHealthPoints = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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
        }

        private void Update()
        {
            if (attackCooldownTimer > -1f) attackCooldownTimer -= Time.deltaTime;

            if (!IsServer) return;
            WalkForwardServerRpc();
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            attackCooldownTimer = AttackSpd;
            currentHealthPoints.OnValueChanged += HealthChanged;
        }

        private void HealthChanged(float previousValue, float newValue)
        {
            if (newValue <= 0) DespawnUnitServerRpc();
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if(!other.gameObject.CompareTag("Unit")) return;
            other.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
            if(enemyUnit.OwnerClientId == OwnerClientId) return;
            if(_targetUnit == null || _targetUnit != enemyUnit) 
                _targetUnit = enemyUnit;
            if (attackCooldownTimer > 0f) return;
            AttackEnemyUnitServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void AttackEnemyUnitServerRpc()
        {
            if(!_targetUnit) return;
            _targetUnit.TakeDamageClientRpc(AttackDMG);
            attackCooldownTimer = AttackSpd;
        }
        
        [ClientRpc]
        private void TakeDamageClientRpc(float damage)
        {
            if(!IsOwner) return;
            currentHealthPoints.Value -= damage;
            if (currentHealthPoints.Value <= 0)
            {
                DespawnUnitServerRpc();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void DespawnUnitServerRpc()
        {
            currentHealthPoints.OnValueChanged -= HealthChanged;
            NetworkObject.Despawn();
        }
        
        private void MoveOrDie(Vector3 dir)
        {
            if (_targetUnit)
            {
                dir = (_targetUnit.transform.position - transform.position).normalized;
            }
            else
            {
                var collider2Ds = Physics2D.OverlapCircleAll(transform.position, 50f);
                foreach (var coli in collider2Ds)
                {
                    if (!coli.gameObject.CompareTag("Unit")) continue;
                    coli.gameObject.TryGetComponent(out PlayerUnit enemyUnit);
                    if (enemyUnit.OwnerClientId == OwnerClientId) continue;
                    _targetUnit = enemyUnit;
                    break;
                }
            }
            transform.Translate(dir * (Speed * Time.deltaTime));
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void WalkForwardServerRpc(){
            MoveOrDie(OwnerClientId == 0 ? Vector3.right : Vector3.left);
        }
        
        [ClientRpc]
        public void SetStartHealthPointsClientRpc()
        {
            if (!IsOwner) return;
            currentHealthPoints.Value = MaxHealthPoints;
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
