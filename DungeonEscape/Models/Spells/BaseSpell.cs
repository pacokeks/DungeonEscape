using System;

namespace DungeonEscape.Models.Spells
{
    /// <summary>
    /// Abstract base class for all spells in the game.
    /// Provides common properties and methods that all spells share.
    /// Handles validation and resource consumption for all derived spells.
    /// </summary>
    public abstract class BaseSpell
    {
        /// <summary>
        /// The name of the spell.
        /// </summary>
        public string Name { get; protected init; }

        /// <summary>
        /// Description of what the spell does.
        /// </summary>
        public string Description { get; protected init; }

        /// <summary>
        /// The type of resource required to cast this spell (Mana, Rage, Energy, or None).
        /// </summary>
        public ResourceType RequiredResourceType { get; protected init; }

        /// <summary>
        /// The amount of resource required to cast this spell.
        /// </summary>
        public int ResourceCost { get; protected init; }

        /// <summary>
        /// The type of damage this spell deals (Physical or Magical).
        /// </summary>
        public DamageType SpellDamageType { get; protected init; }

        /// <summary>
        /// Base constructor for creating a spell.
        /// Protected ensures only derived classes can instantiate.
        /// </summary>
        /// <param name="name">Name of the spell</param>
        /// <param name="description">Description of the spell</param>
        /// <param name="resourceType">Type of resource required</param>
        /// <param name="resourceCost">Amount of resource required</param>
        /// <param name="damageType">Type of damage dealt</param>
        protected BaseSpell(string name, string description, ResourceType resourceType, int resourceCost, DamageType damageType)
        {
            Name = name;
            Description = description;
            RequiredResourceType = resourceType;
            ResourceCost = resourceCost;
            SpellDamageType = damageType;
        }

        /// <summary>
        /// Checks if the caster can cast this spell.
        /// Validates resource type, resource amount, and if caster is alive.
        /// </summary>
        /// <param name="caster">The character attempting to cast the spell</param>
        /// <returns>True if the spell can be cast, false otherwise</returns>
        public virtual bool CanCast(BaseCharacter caster)
        {
            if (caster == null)
            {
                Console.WriteLine("No caster provided!");
                return false;
            }

            if (!caster.IsAlive)
            {
                Console.WriteLine($"{caster.Name} is defeated and cannot cast spells!");
                return false;
            }

            // Check if caster uses the correct resource type
            if (caster.PrimaryResourceType != RequiredResourceType && RequiredResourceType != ResourceType.None)
            {
                Console.WriteLine($"{caster.Name} cannot cast {Name} (requires {RequiredResourceType}, but uses {caster.PrimaryResourceType})");
                return false;
            }

            // Check if caster has enough resources
            if (caster.CurrentResource < ResourceCost)
            {
                Console.WriteLine($"{caster.Name} doesn't have enough {RequiredResourceType} for {Name}! ({caster.CurrentResource}/{ResourceCost})");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates that the target is valid for this spell.
        /// </summary>
        /// <param name="target">The intended target</param>
        /// <returns>True if target is valid, false otherwise</returns>
        protected virtual bool IsValidTarget(BaseCharacter target)
        {
            if (target == null)
            {
                Console.WriteLine("No valid target!");
                return false;
            }

            if (!target.IsAlive)
            {
                Console.WriteLine($"{target.Name} is already defeated!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Casts the spell from caster to target.
        /// Handles validation, resource consumption, and spell effect execution.
        /// </summary>
        /// <param name="caster">The character casting the spell</param>
        /// <param name="target">The target of the spell</param>
        /// <returns>True if spell was successfully cast, false otherwise</returns>
        public bool Cast(BaseCharacter caster, BaseCharacter target)
        {
            // Validate caster
            if (!CanCast(caster))
            {
                return false;
            }

            // Validate target
            if (!IsValidTarget(target))
            {
                return false;
            }

            // Consume resources
            if (!caster.TryConsumeResource(ResourceCost))
            {
                Console.WriteLine($"{caster.Name} failed to consume resources for {Name}!");
                return false;
            }

            // Execute the spell effect
            Console.WriteLine($"{caster.Name} casts {Name}!");
            ExecuteSpellEffect(caster, target);

            return true;
        }

        /// <summary>
        /// Executes the actual effect of the spell.
        /// Must be implemented by derived classes to define what the spell does.
        /// </summary>
        /// <param name="caster">The character casting the spell</param>
        /// <param name="target">The target of the spell</param>
        protected abstract void ExecuteSpellEffect(BaseCharacter caster, BaseCharacter target);

        /// <summary>
        /// Displays information about the spell.
        /// Virtual allows derived classes to add specific details.
        /// </summary>
        public virtual void ShowSpellInfo()
        {
            Console.WriteLine($"=== {Name} ===");
            Console.WriteLine($"{Description}");
            Console.WriteLine($"Cost: {ResourceCost} {RequiredResourceType}");
            Console.WriteLine($"Damage Type: {SpellDamageType}");
        }

        /// <summary>
        /// Read caster's SpellPower property via reflection.
        /// Returns 0 if caster is null, property not found, or value is not an int.
        /// Implemented step-by-step for clarity and safe unboxing.
        /// </summary>
        protected int GetSpellPower(BaseCharacter caster)
        {
            // 1) If no caster provided, return 0.
            if (caster == null)
                return 0;

            // 2) Try to find the "SpellPower" property on the caster type.
            var prop = caster.GetType().GetProperty("SpellPower");
            if (prop == null)
                return 0;

            // 3) Read the value and ensure it's an int before returning.
            var value = prop.GetValue(caster);
            if (value is int intValue)
                return intValue;

            // 4) If property exists but value is null or not an int, return 0.
            return 0;
        }

        /// <summary>
        /// Read caster's Strength property via reflection.
        /// Returns 0 if caster is null, property not found, or value is not an int.
        /// Implemented step-by-step for clarity and safe unboxing.
        /// </summary>
        protected int GetStrength(BaseCharacter caster)
        {
            // 1) If no caster provided, return 0.
            if (caster == null)
                return 0;

            // 2) Try to find the Strength property on the caster type.
            var prop = caster.GetType().GetProperty("Strength");    
            if (prop == null)
                return 0;

            // 3) Read the value and ensure it's an int before returning.
            var value = prop.GetValue(caster);
            if (value is int intValue)
                return intValue;

            // 4) If property exists but value is null or not an int, return 0
            return 0;
        }
    }
}