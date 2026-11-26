using System.Collections.Generic;
using UnityEngine;

public class CombatSystem
{
    private readonly GridManager gridManager;
    private readonly GameObject attackOverlayPrefab;
    private readonly TurnManager turnManager;
    private readonly List<GameObject> attackOverlays = new List<GameObject>();

    public CombatSystem(
        GridManager gridManager,
        GameObject attackOverlayPrefab,
        TurnManager turnManager
    )
    {
        this.gridManager = gridManager;
        this.attackOverlayPrefab = attackOverlayPrefab;
        this.turnManager = turnManager;
    }

    public bool TryAttackUnit(Unit attacker, int targetX, int targetZ)
    {
        if (attacker == null || attacker.Data == null || attacker.IsMoving ||
            !turnManager.CanAttack(attacker) ||
            gridManager.GetUnitAt(attacker.gridX, attacker.gridZ) != attacker ||
            !IsWithinAttackRange(attacker, targetX, targetZ))
        {
            return false;
        }

        Unit target = gridManager.GetUnitAt(targetX, targetZ);

        if (target == null || target.IsDefeated ||
            target.Team == attacker.Team || target.Data == null)
            return false;

        float damageAfterArmor =
            attacker.Data.Attack * (1f - target.Data.ArmorPercent / 100f);
        int calculatedDamage = Mathf.Max(
            0,
            Mathf.FloorToInt(damageAfterArmor + 0.5f)
        );
        int appliedDamage = target.ApplyDamage(calculatedDamage);

        Debug.Log(
            $"{attacker.name} dealt {appliedDamage} damage to {target.name}. " +
            $"Remaining health: {target.CurrentHealth}/{target.Data.MaxHealth}.",
            target
        );
        turnManager.CompleteAttack(attacker);
        return true;
    }

    public void ShowAttackRange(Unit unit)
    {
        ClearAttackRange();

        if (unit == null || unit.Data == null || attackOverlayPrefab == null)
            return;

        for (int x = 0; x < gridManager.width; x++)
        {
            for (int z = 0; z < gridManager.height; z++)
            {
                if (!IsWithinAttackRange(unit, x, z))
                    continue;

                Vector3 position = new Vector3(x, 0.12f, z);
                GameObject overlay = Object.Instantiate(
                    attackOverlayPrefab,
                    position,
                    Quaternion.identity,
                    gridManager.transform
                );
                attackOverlays.Add(overlay);
            }
        }
    }

    public void ClearAttackRange()
    {
        foreach (GameObject overlay in attackOverlays)
        {
            if (overlay != null)
                Object.Destroy(overlay);
        }

        attackOverlays.Clear();
    }

    private bool IsWithinAttackRange(Unit unit, int targetX, int targetZ)
    {
        if (unit == null || unit.Data == null ||
            !gridManager.IsWithinBoard(targetX, targetZ))
        {
            return false;
        }

        int distance =
            Mathf.Abs(targetX - unit.gridX) + Mathf.Abs(targetZ - unit.gridZ);

        return distance >= unit.Data.MinAttackRange &&
            distance <= unit.Data.MaxAttackRange;
    }
}
