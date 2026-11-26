using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    private readonly List<Unit> registeredUnits = new List<Unit>();
    private readonly List<Unit> turnOrder = new List<Unit>();
    private bool moveConsumed;
    private bool attackConsumed;
    private bool battleStopped;
    private int activeUnitIndex;

    public int CurrentRound { get; private set; }
    public Team PriorityTeam { get; private set; }
    public Unit ActiveUnit { get; private set; }
    public event Action<Unit> ActiveUnitChanged;

    public void BeginBattle(IReadOnlyList<Unit> units)
    {
        if (battleStopped)
        {
            ActiveUnit = null;
            ActiveUnitChanged?.Invoke(null);
            return;
        }

        registeredUnits.Clear();
        turnOrder.Clear();
        HashSet<Unit> uniqueUnits = new HashSet<Unit>();

        if (units != null)
        {
            for (int i = 0; i < units.Count; i++)
            {
                Unit unit = units[i];

                if (unit != null && unit.Data != null && uniqueUnits.Add(unit))
                    registeredUnits.Add(unit);
            }
        }

        PriorityTeam = UnityEngine.Random.Range(0, 2) == 0 ? Team.Blue : Team.Red;
        CurrentRound = 0;
        activeUnitIndex = -1;
        StartNextRound();
    }

    public void RemoveUnit(Unit unit)
    {
        if (unit == null)
            return;

        registeredUnits.Remove(unit);

        if (battleStopped)
            return;

        int turnIndex = turnOrder.IndexOf(unit);

        if (turnIndex < 0)
            return;

        if (unit == ActiveUnit)
        {
            turnOrder.RemoveAt(turnIndex);
            activeUnitIndex = turnIndex - 1;
            AdvanceTurn();
            return;
        }

        if (turnIndex < activeUnitIndex)
            activeUnitIndex--;

        turnOrder.RemoveAt(turnIndex);
    }

    public void StopBattle()
    {
        if (battleStopped)
            return;

        battleStopped = true;
        registeredUnits.Clear();
        turnOrder.Clear();
        activeUnitIndex = -1;
        moveConsumed = false;
        attackConsumed = false;
        ActiveUnit = null;
        ActiveUnitChanged?.Invoke(null);
    }

    public bool IsActiveUnit(Unit unit)
    {
        return !battleStopped && unit != null &&
            unit == ActiveUnit && unit.CurrentHealth > 0;
    }

    public bool CanMove(Unit unit)
    {
        return IsActiveUnit(unit) && !moveConsumed;
    }

    public bool CanAttack(Unit unit)
    {
        return IsActiveUnit(unit) && !attackConsumed;
    }

    public bool CompleteMove(Unit unit)
    {
        if (!CanMove(unit))
            return false;

        moveConsumed = true;
        AdvanceIfActionsConsumed();
        return true;
    }

    public bool CompleteAttack(Unit unit)
    {
        if (!CanAttack(unit))
            return false;

        attackConsumed = true;
        AdvanceIfActionsConsumed();
        return true;
    }

    public bool EndTurn(Unit unit)
    {
        if (!IsActiveUnit(unit))
            return false;

        AdvanceTurn();
        return true;
    }

    private void StartNextRound()
    {
        if (battleStopped)
            return;

        if (CurrentRound > 0)
            PriorityTeam = GetOpposingTeam(PriorityTeam);

        CurrentRound++;
        BuildTurnOrder();
        activeUnitIndex = 0;

        if (turnOrder.Count == 0)
        {
            ActiveUnit = null;
            moveConsumed = false;
            attackConsumed = false;
            ActiveUnitChanged?.Invoke(null);
            Debug.LogWarning($"Round {CurrentRound}: no living units can act.");
            return;
        }

        ActivateUnit(turnOrder[activeUnitIndex]);
        Debug.Log($"Round {CurrentRound} priority team: {PriorityTeam}.");
    }

    private void BuildTurnOrder()
    {
        turnOrder.Clear();
        Dictionary<int, List<Unit>> unitsBySpeed =
            new Dictionary<int, List<Unit>>();
        List<int> speeds = new List<int>();

        foreach (Unit unit in registeredUnits)
        {
            if (unit == null || unit.CurrentHealth <= 0 || unit.Data == null)
                continue;

            int speed = unit.Data.InitiativeSpeed;

            if (!unitsBySpeed.TryGetValue(speed, out List<Unit> sameSpeedUnits))
            {
                sameSpeedUnits = new List<Unit>();
                unitsBySpeed.Add(speed, sameSpeedUnits);
                speeds.Add(speed);
            }

            sameSpeedUnits.Add(unit);
        }

        speeds.Sort((left, right) => right.CompareTo(left));

        foreach (int speed in speeds)
        {
            Queue<Unit> blueUnits = new Queue<Unit>();
            Queue<Unit> redUnits = new Queue<Unit>();

            foreach (Unit unit in unitsBySpeed[speed])
            {
                if (unit.Team == Team.Blue)
                    blueUnits.Enqueue(unit);
                else if (unit.Team == Team.Red)
                    redUnits.Enqueue(unit);
            }

            AddAlternatingTeams(blueUnits, redUnits);
        }
    }

    private void AddAlternatingTeams(
        Queue<Unit> blueUnits,
        Queue<Unit> redUnits
    )
    {
        Team nextTeam = PriorityTeam;

        while (blueUnits.Count > 0 || redUnits.Count > 0)
        {
            Queue<Unit> nextQueue =
                nextTeam == Team.Blue ? blueUnits : redUnits;

            if (nextQueue.Count == 0)
            {
                nextTeam = GetOpposingTeam(nextTeam);
                nextQueue = nextTeam == Team.Blue ? blueUnits : redUnits;
            }

            turnOrder.Add(nextQueue.Dequeue());
            nextTeam = GetOpposingTeam(nextTeam);
        }
    }

    private void ActivateUnit(Unit unit)
    {
        ActiveUnit = unit;
        moveConsumed = false;
        attackConsumed = false;
        ActiveUnitChanged?.Invoke(unit);
        Debug.Log(
            $"Round {CurrentRound} | Team: {unit.Team} | Active unit: {unit.name}.",
            unit
        );
    }

    private void AdvanceIfActionsConsumed()
    {
        if (moveConsumed && attackConsumed)
            AdvanceTurn();
    }

    private void AdvanceTurn()
    {
        if (battleStopped)
            return;

        activeUnitIndex++;

        while (activeUnitIndex < turnOrder.Count)
        {
            Unit nextUnit = turnOrder[activeUnitIndex];

            if (nextUnit != null && nextUnit.CurrentHealth > 0)
            {
                ActivateUnit(nextUnit);
                return;
            }

            activeUnitIndex++;
        }

        StartNextRound();
    }

    private static Team GetOpposingTeam(Team team)
    {
        return team == Team.Blue ? Team.Red : Team.Blue;
    }
}
