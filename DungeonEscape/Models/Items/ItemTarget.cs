namespace DungeonEscape.Models.Items
{
    /// <summary>
    /// Defines which target types an item is allowed to be used on.
    /// Kept as separate file to allow reuse and future extension (e.g., Party/Ally support).
    /// </summary>
    public enum ItemTarget
    {
        Self,   // only the user
        Enemy,  // only an enemy target
        Friend, // allies / party members (requires Party/Allies support in UI/CombatManager)
        Any     // any valid target (self, enemy, friend)
    }
}