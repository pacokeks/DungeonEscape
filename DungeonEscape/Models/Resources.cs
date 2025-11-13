namespace DungeonEscape.Models.Spells
{
    /// <summary>
    /// Defines the types of resources that characters can use for abilities.
    /// </summary>
    public enum ResourceType
    {
        None,   // Character has no resource system (e.g., basic enemies)
        Mana,   // Used by magic users (Mages, Priests)
        Rage,   // Used by warriors, generated through combat
        Energy  // Used by rogues, regenerates quickly
    }

    /// <summary>
    /// Defines the types of damage that spells and abilities can deal.
    /// </summary>
    public enum DamageType
    {
        Physical,  // Physical damage, reduced by Defense
        Magical    // Magical damage, reduced by MagicResistance
    }
}