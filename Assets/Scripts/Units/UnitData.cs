using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Tactical RPG/Unit Data")]
public class UnitData : ScriptableObject
{
    [SerializeField] private UnitClass unitClass;
    [SerializeField] private int maxHealth;
    [SerializeField] private int attack;
    [SerializeField] private int armorPercent;
    [SerializeField] private int movementRange;
    [SerializeField] private int minAttackRange;
    [SerializeField] private int maxAttackRange;
    [SerializeField] private int initiativeSpeed;

    public UnitClass Class => unitClass;
    public int MaxHealth => maxHealth;
    public int Attack => attack;
    public int ArmorPercent => armorPercent;
    public int MovementRange => movementRange;
    public int MinAttackRange => minAttackRange;
    public int MaxAttackRange => maxAttackRange;
    public int InitiativeSpeed => initiativeSpeed;
}
