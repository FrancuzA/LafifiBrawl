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
    
        private PlayerUnit _targetUnit;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            var wait = new WaitForSeconds(AttackSpd);
            canAttack = true;
            while (true)
            {
                yield return new WaitUntil(() => !canAttack);
                Debug.Log($"{gameObject.name} is waiting to attack again.");
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
            if (!canAttack) return;
            Attack(enemyUnit);
        
        }

        private void Attack(PlayerUnit enemyUnit)
        {
            enemyUnit.TakeDamageClientRpc(AttackDMG);
            Debug.Log($"{gameObject.name} attacked {enemyUnit.gameObject.name} for {AttackDMG} damage.");
        }

        [ServerRpc(RequireOwnership = false)]
        private void WalkForwardServerRpc(){
            MoveOrDie(OwnerClientId == 0 ? Vector3.right : Vector3.left);
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
