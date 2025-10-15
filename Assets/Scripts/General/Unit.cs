using Fusion;
using UnityEngine;

public class Unit : NetworkBehaviour
{
    [Networked] public int OwnerPlayerId { get; set; }
    [Networked] public float Health { get; set; }
    [Networked] public float AttackDamage { get; set; }

    private Unit target;
    private float attackCooldown = 1f;
    private float lastAttackTime;

    public void SetOwner(int playerId)
    {
        OwnerPlayerId = playerId;
        // You can also change visual appearance based on owner
    }

    public override void FixedUpdateNetwork()
    {
        if (GamePhaseManager.Instance.CurrentPhase != GamePhase.Battle) return;
        if (Health <= 0) return;

        FindTarget();
        AttackTarget();
    }

    private void FindTarget()
    {
        if (target != null && target.Health > 0) return;

        // Simple target finding - you can make this more sophisticated
        Unit[] allUnits = FindObjectsOfType<Unit>();
        float closestDistance = Mathf.Infinity;

        foreach (Unit unit in allUnits)
        {
            if (unit.OwnerPlayerId != OwnerPlayerId && unit.Health > 0)
            {
                float distance = Vector3.Distance(transform.position, unit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = unit;
                }
            }
        }
    }

    private void AttackTarget()
    {
        if (target == null || target.Health <= 0) return;

        // Check if in range and cooldown is ready
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= 2f && Runner.SimulationTime - lastAttackTime >= attackCooldown)
        {
            target.TakeDamage(AttackDamage);
            lastAttackTime = Runner.SimulationTime;
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            // Handle unit death
            Runner.Despawn(Object);
        }
    }
}