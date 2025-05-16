using System;
using System.Dynamic;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using DungeonEscape.Models;
using DungeonEscape.Models.Player;
using static System.Net.Mime.MediaTypeNames;

namespace DungeonEscape.Models.Spells
{
    /// <summary>
    /// Enum defining different resource types used for abilities and spells.
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// Mana is used by mages and other spellcasters.
        /// </summary>
        Mana,

        /// <summary>
        /// Rage is generated and used by warriors.
        /// </summary>
        Rage,

        /// <summary>
        /// Energy is used by rogues and similar classes.
        /// </summary>
        Energy,

        /// <summary>
        /// Some abilities don't require any resource.
        /// </summary>
        None
    }



    /// <summary>
    /// Enum defining different types of spells and abilities.
    /// For example: Physical, Magic, Heal, Buff, Debuff.
    /// </summary>
    public enum SpellType
    {
        /// <summary>
        /// A spell/ability that deals physical damage to the target.
        /// </summary>
        Physical,
        /// <summary>
        /// A spell/ability that deals magic damage to the target.
        /// </summary>
        Magic,
        /// <summary>
        /// A spell that heals the target.
        /// </summary>
        Heal,
        /// <summary>
        /// A spell that provides a buff to the caster or allies.
        /// </summary>
        Buff,
        /// <summary>
        /// A spell that debuffs the target, reducing their effectiveness.
        /// </summary>
        Debuff
    }

    /// <summary>
    /// Base abstract class for all spells and abilities in the game.
    /// Supports different resource types (mana, rage, energy, etc.).
    /// </summary>
    public abstract class BaseSpell
    {
        /// <summary>
        /// The name of the spell or ability.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The type of resource this spell or ability uses.
        /// </summary>
        public ResourceType ResourceType { get; protected set; }

        /// <summary>
        /// The type of spell or ability (e.g., Physical, Magic, Heal, Buff, Debuff).
        /// </summary>
        public SpellType SpellType { get; protected set; }

        /// <summary>
        /// The cost in the appropriate resource (mana, rage, energy, etc.).
        /// </summary>
        public int ResourceCost { get; protected set; }

        /// <summary>
        /// The number of turns before this spell/ability can be cast again after use.
        /// </summary>
        public int CooldownTurns { get; protected set; }

        /// <summary>
        /// Tracks the current cooldown state. When greater than 0, the spell cannot be cast.
        /// </summary>
        public int CurrentCooldown { get; protected set; }

        /// <summary>
        /// Constructor for creating a new spell or ability with base properties.
        /// </summary>
        /// <param name="name">The name of the spell/ability</param>
        /// <param name="resourceType">The type of resource it uses</param>
        /// <param name="resourceCost">The cost in the appropriate resource</param>
        /// <param name="cooldownTurns">The cooldown period after casting</param>
        protected BaseSpell(string name, ResourceType resourceType, SpellType spellType, int resourceCost, int cooldownTurns)
        {
            Name = name;
            ResourceType = resourceType;
            ResourceCost = resourceCost;
            CooldownTurns = cooldownTurns;
            CurrentCooldown = 0; // Spells/abilities start ready to use
        }

        /// <summary>
        /// Abstract method that must be implemented by derived classes.
        /// </summary>
        /// <param name="caster">The character using the ability</param>
        /// <param name="target">The target of the spell</param>
        public abstract void Cast(BaseCharacter caster, BaseCharacter target);

        /// <summary>
        /// Checks if the spell is currently on cooldown.
        /// </summary>
        /// <returns>True if on colldown and connut be used, otherwise false</returns>
        public bool isOnCooldown()
        {
            // Check if the spell is currently on cooldown
            return CurrentCooldown > 0;
        }

        /// <summary>
        /// Reduces the cooldown of the spell by 1 turn.
        /// Called at the end of each turn.
        /// </summary>
        public void ReduceCooldown()
        {
            // Reduce the cooldown by 1 turn
            if (CurrentCooldown > 0)
            {
                CurrentCooldown--;
            }
        }

        /// <summary>
        /// Resets the cooldown of the spell to 0.
        /// For example after a battle, when the spell is learned or after a spell to reset the cooldown.
        /// </summary>
        public void ResetCooldown()
        {
            // Reset the cooldown to 0
            CurrentCooldown = 0;
        }

        /// <summary>
        /// Checks if the caster has enough of the required resources to cast the spell/ability.
        /// </summary>
        /// <param name="caster">The character attempting to use the spell/ability</param>
        /// <returns>True if the caster has enough resources, otherwise false</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool HasEnoughResources(BaseCharacter caster)
        {
            // Check if the caster has enough resources to cast the spell
            switch (ResourceType)
            {
                case ResourceType.Mana:
                    return caster is Mage mage && mage.Mana >= ResourceCost;
                case ResourceType.Rage:
                    return caster is Warrior warrior && warrior.Rage >= ResourceCost;
                case ResourceType.Energy:
                    // Commented out until Rogue is implemented
                    // return caster is Rogue rogue && rogue.Energy >= ResourceCost;
                    return false;
                case ResourceType.None:
                    // No resources required
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Attempts to consume the required resources for casting the spell/ability.
        /// </summary>
        /// <param name="caster">The charracter using the spell/ability</param>
        /// <returns>True of resources were consumes, otherwise false</returns>
        public virtual bool ConsumeResources(BaseCharacter caster)
        {
            // Check if the spell is on cooldown
            if (isOnCooldown())
            {
                return false;
            }

            // Consume resources based on the resource type
            return caster.TryConsumeResources(ResourceType, ResourceCost);
        }


        /// <summary>
        /// Returns a description of the spell/ability.
        /// </summary>
        /// <returns>Returns a string with the name of the character, the resourcecost, tpye and cooldown</returns>
        public virtual string GetDescription()
        {

            string resourceInfo = ResourceType == ResourceType.None ?
                "No resources required" : 
                $"{ResourceCost} {ResourceType}";
            // Return a description of the spell/ability
            return $"{Name} (Cost: {ResourceCost} {ResourceType}, Cooldown: {CooldownTurns} turns)";
        }


    }
    
}