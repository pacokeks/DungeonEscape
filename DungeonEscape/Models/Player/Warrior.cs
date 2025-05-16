using System;
using DungeonEscape.Models;
using System.Collections.Generic;
using DungeonEscape.Models.Spells;

namespace DungeonEscape.Models.Player
{
    /// <summary>
    /// Warrior class - specializes in close combat with high health and strength.
    /// Inherits from BaseCharacter and adds rage mechanic.
    /// </summary>
    public class Warrior : BaseCharacter
    {
        /// <summary>
        /// The character's strength attribute, affects attack damage.
        /// </summary>
        public int Strength { get; private set; } // Warrior's strength attribute
        /// <summary>
        /// Current rage points. Used for special abilities and increased damage.
        /// </summary>
        public override int Rage { get; protected set; }

        /// <summary>
        /// Maximum possible rage points for this warrior.
        /// </summary>
        public override int MaxRage { get; protected set; }

        /// <summary>
        /// List of spells/abilities the warrior can use.
        /// </summary>
        public List<BaseSpell> Abilities { get; private set; }

        /// <summary>
        /// Constructor for creating a new Warrior character.
        /// </summary>
        /// <param name="name">The warrior's name</param>
        public Warrior(string name, int health, int defense, int magicResistance, int strength) : base(name, health, defense, magicResistance)
        {
            // Warriors have a rage mechanic - initialize it
            MaxRage = 100;
            Rage = 0; // Start with no rage
            this.Strength = strength;

            // Initialize warrior's special abilities
            Abilities = new List<BaseSpell>
            {
                new BerserkerStrike() // Add the warrior's special ability
            };
        }

        /// <summary>
        /// Overrides the base damage calculation to include rage bonuses.
        /// Also generates rage on each attack.
        /// </summary>
        /// <returns>The modified damage amount including rage bonuses</returns>
        public override int CalculateAttackDamage()
        {
            // First get the base damage from the parent class
            int baseDamage = Strength;

            // Calculate additional damage based on current rage (1 damage per 10 rage)
            int rageBonus = Math.Min(1,Rage / 10);

            // Generate rage on attack - increases after each attack
            Rage = Math.Min(MaxRage, Rage + 15);

            // Return total damage (base + rage bonus)
            return baseDamage + rageBonus;
        }

        /// <summary>
        /// Consumes rage for abilities. Only warriors can use rage as a resource.
        /// </summary>
        /// <param name="amount">Amount of rage to consume</param>
        /// <returns>True if enough rage was available and consumed, false otherwise</returns>
        public bool ConsumeRage(int amount)
        {
            if (Rage >= amount)
            {
                Rage -= amount;
                return true;
            }
            return false;
        }

        public override void ShowStats()
        {
            base.ShowStats();

            Console.WriteLine($"Strength: {Strength}");

            Console.WriteLine("\nAbilities:");
            foreach (var ability in Abilities)
            {
                Console.WriteLine($". {ability.Name} ({ability.ResourceCost} {ability.ResourceType}), Cooldown: {ability.CurrentCooldown}/{ability.CooldownTurns}");
            }
            Console.WriteLine("");
        }
    }
}