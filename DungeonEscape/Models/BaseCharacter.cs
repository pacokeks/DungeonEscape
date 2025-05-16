using System;
using DungeonEscape.Models.Spells;

namespace DungeonEscape.Models
{
    /// <summary>
    /// Base abstract class for all character types in the game.
    /// Provides common properties and methods that all characters share.
    /// </summary>
    public abstract class BaseCharacter
    {
        /// <summary>
        /// The character's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current health points of the character. When it reaches 0, the character is defeated.
        /// Protected set ensures derived classes can modify it, but external code cannot.
        /// </summary>
        public int Health { get; protected set; }

        /// <summary>
        /// Maximum possible health points for this character.
        /// </summary>
        public int MaxHealth { get; protected set; }
        public virtual int Mana { get; protected set; } = 0;
        public virtual int MaxMana { get; protected set; } = 0;
        public virtual int Rage { get; protected set; } = 0;
        public virtual int MaxRage { get; protected set; } = 0;
        public virtual int Energy { get; protected set; } = 0;
        public virtual int MaxEnergy { get; protected set; } = 0;


        /// <summary>
        /// The character's defense attribute, reduces damage taken.
        /// </summary>
        public int Defense { get; protected set; }

        /// <summary>
        /// The character's Magic Resistance attribute, reduces damage from magical attacks.
        /// </summary>
        public int MagicResistance { get; protected set; }

        /// <summary>
        /// Returns true if the character's health is above 0, false otherwise.
        /// Implemented as an expression-bodied property for conciseness.
        /// </summary>
        public bool IsAlive => Health > 0;

        /// <summary>
        /// Constructor for creating a character with base stats.
        /// Protected access modifier ensures only derived classes can instantiate it.
        /// </summary>
        /// <param name="name">The name of the character</param>
        /// <param name="health">Initial and maximum health points</param>
        /// <param name="strength">Base strength attribute</param>
        /// <param name="defense">Base defense attribute</param>
        protected BaseCharacter(string name, int health, int defense, int magicResistance)
        {
            // Initialize the character's properties with the provided values
            Name = name;
            MaxHealth = health;
            Health = MaxHealth; // Start with full health
            Defense = defense;
            MagicResistance = magicResistance;
        }

        /// <summary>
        /// Reduces the character's health based on incoming damage and defense value.
        /// Virtual keyword allows derived classes to override this method with custom behavior.
        /// </summary>
        /// <param name="damage">The raw damage amount before defense calculation</param>
        public virtual void TakeDamage(int damage)
        {
            // Calculate actual damage by subtracting defense, ensuring at least 1 damage is dealt
            int actualDamage = Math.Max(1, damage - Defense);

            // Reduce health but ensure it doesn't go below 0
            Health = Math.Max(0, Health - actualDamage);

            // Output the result of the damage calculation
            Console.WriteLine($"{Name} takes {actualDamage} damage. Health: {Health}/{MaxHealth}");
        }

        public virtual bool TryConsumeResources(ResourceType resourceType, int amount)
        {
            switch (resourceType)
            {
                case ResourceType.Mana:
                    if (Mana >= amount)
                    {
                        Mana -= amount;
                        return true;
                    }
                    return false;
                case ResourceType.Rage:
                    if (Rage >= amount)
                    {
                        Rage -= amount;
                        return true;
                    }
                    return false;
                case ResourceType.Energy:
                    if (Energy >= amount)
                    {
                        Energy -= amount;
                        return true;
                    }
                    return false;
                case ResourceType.None:
                    return true; // No resources to consume
                default:
                    throw new ArgumentOutOfRangeException(nameof(resourceType), $"Unknown resource type: {resourceType}");
            }
        }
        public abstract int CalculateAttackDamage();

        public virtual void ShowStats()
        {
            Console.WriteLine($"=== {Name} Stats ===");
            Console.WriteLine($"Health: {Health}/{MaxHealth}");
            Console.WriteLine($"Defense: {Defense}");
            Console.WriteLine($"Magic Resistance: {MagicResistance}");

            if (Mana >= 0)
            {
                Console.WriteLine($"Mana: {Mana}/{MaxMana}");
            }
            else if (Rage >= 0)
            {
                Console.WriteLine($"Rage: {Rage}/{MaxRage}");
            }
            else if (Energy >= 0)
            {
                Console.WriteLine($"Energy: {Energy}/{MaxEnergy}");
            }
            Console.WriteLine($"Status: {(IsAlive ? "Alive" : "Defeated")}");
        }
    }
}