using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    private readonly HashSet<Unit> remainingUnits = new HashSet<Unit>();

    public bool IsInitialized { get; private set; }
    public bool IsFinished { get; private set; }
    public Team? Winner { get; private set; }
    public event Action BattleEnded;

    internal void BeginBattle(IReadOnlyList<Unit> units)
    {
        if (IsInitialized)
        {
            Debug.LogError("BattleManager has already been initialized.");
            return;
        }

        IsInitialized = true;

        if (units != null)
        {
            for (int i = 0; i < units.Count; i++)
            {
                Unit unit = units[i];

                if (unit != null && unit.Data != null && unit.CurrentHealth > 0)
                    remainingUnits.Add(unit);
            }
        }

        bool blueHasUnits = HasRemainingUnits(Team.Blue);
        bool redHasUnits = HasRemainingUnits(Team.Red);

        if (!blueHasUnits && redHasUnits)
            FinishBattle(Team.Red);
        else if (!redHasUnits && blueHasUnits)
            FinishBattle(Team.Blue);
    }

    internal void RegisterUnitDefeated(Unit unit)
    {
        if (!IsInitialized || IsFinished || unit == null ||
            !remainingUnits.Remove(unit))
        {
            return;
        }

        Team opposingTeam = GetOpposingTeam(unit.Team);

        if (!HasRemainingUnits(unit.Team) && HasRemainingUnits(opposingTeam))
            FinishBattle(opposingTeam);
    }

    private bool HasRemainingUnits(Team team)
    {
        foreach (Unit unit in remainingUnits)
        {
            if (unit != null && unit.CurrentHealth > 0 && unit.Team == team)
                return true;
        }

        return false;
    }

    private void FinishBattle(Team winner)
    {
        if (IsFinished)
            return;

        Winner = winner;
        IsFinished = true;
        Debug.Log($"{winner} team wins the battle.");
        BattleEnded?.Invoke();
    }

    private static Team GetOpposingTeam(Team team)
    {
        return team == Team.Blue ? Team.Red : Team.Blue;
    }
}
