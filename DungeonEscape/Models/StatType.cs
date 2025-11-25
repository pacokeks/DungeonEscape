namespace DungeonEscape.Models
{
    /// <summary>
    /// Supported stat keys for temporary buffs.
    /// Extend as needed (e.g. CriticalChance, AttackPower, etc.).
    /// </summary>
    public enum StatType
    {
        Dodge,
        Defense,
        MagicResistance,
        Attack
    }
}