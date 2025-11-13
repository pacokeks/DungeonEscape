using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DungeonEscape.Models.Spells;
using ResourceType = DungeonEscape.Models.Spells.ResourceType;

namespace DungeonEscape.Models.Player
{
    /// <summary>
    /// Mage class that uses Mana as primary resource.
    /// Mages have powerful spells but weaker physical attacks.
    /// </summary>
    public class Mage : BaseCharacter
    {
        /// <summary>
        /// Current mana points. Private set - only this class manages mana.
        /// </summary>
        public int Mana { get; private set; }

        /// <summary>
        /// Maximum mana points. Private set - only this class manages mana.
        /// </summary>
        public int MaxMana { get; private set; }

        /// <summary>
        /// Spell power attribute that increases magical damage.
        /// </summary>
        public int SpellPower { get; private set; }

        // Implement abstract properties from base class
        public override ResourceType PrimaryResourceType => ResourceType.Mana;
        public override int CurrentResource => Mana;
        public override int MaxResource => MaxMana;

        /// <summary>
        /// Creates a new Mage with the provided base stats.
        /// </summary>
        /// <param name="in_name">Character name</param>
        /// <param name="in_health">Initial and maximum health points</param>
        /// <param name="in_defense">Base defense attribute</param>
        /// <param name="in_magicResistance">Base magic resistance attribute</param>
        /// <param name="in_maxMana">Maximum mana this mage can have</param>
        /// <param name="in_spellPower">Spell power that increases magical damage</param>
        public Mage(string in_name, int in_health, int in_defense, int in_magicResistance, int in_maxMana, int in_spellPower)
            : base(in_name, in_health, in_defense, in_magicResistance)
        {
            MaxMana = in_maxMana;
            Mana = MaxMana; // Start with full mana
            SpellPower = in_spellPower;
            BaseAttack = 10; // Mages have lower base physical attack
        }

        /// <summary>
        /// Attempts to consume the specified amount of mana.
        /// Returns true if enough mana was available and consumed.
        /// </summary>
        /// <param name="usage_amount">Amount of mana to consume</param>
        /// <returns>True if consumption succeeded, false otherwise</returns>
        public override bool TryConsumeResource(int usage_amount)
        {
            if (Mana >= usage_amount)
            {
                Mana -= usage_amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the mage's basic attack damage.
        /// Mages have weak physical attacks but scale slightly with spell power.
        /// </summary>
        /// <returns>Calculated damage amount</returns>
        public override int CalculateAttackDamage()
        {
            // Mage's basic attack: weak physical with slight spell power scaling
            return BaseAttack + (SpellPower / 3);
        }

        /// <summary>
        /// Displays mage-specific stats in addition to base stats.
        /// </summary>
        public override void ShowStats()
        {
            // Call base implementation first
            base.ShowStats();

            // Add mage-specific stats
            Console.WriteLine($"Spell Power: {SpellPower}");
        }

        /// <summary>
        /// Regenerates mana over time or after combat.
        /// </summary>
        /// <param name="regen_amount">Amount of mana to restore</param>
        public void RegenerateMana(int regen_amount)
        {
            int oldMana = Mana;
            Mana = Math.Min(MaxMana, Mana + regen_amount);

            if (Mana > oldMana)
            {
                Console.WriteLine($"{Name} regenerates {Mana - oldMana} mana. Mana: {Mana}/{MaxMana}");
            }
        }

        /// <summary>
        /// Powerful spell that costs mana and deals high magical damage.
        /// </summary>
        /// <param name="target">The character to attack with fireball</param>
        public void CastFireball(BaseCharacter target)
        {
            const int manaCost = 50;
            const int baseDamage = 60;

            if (!IsAlive)
            {
                Console.WriteLine($"{Name} is defeated and cannot cast spells!");
                return;
            }

            if (target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} has no valid target for Fireball!");
                return;
            }

            if (!TryConsumeResource(manaCost))
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Fireball! ({Mana}/{manaCost})");
                return;
            }

            int damage = baseDamage + SpellPower;
            Console.WriteLine($"{Name} casts Fireball at {target.Name}!");

            // Use TakeMagicalDamage to properly apply magical damage with magic resistance
            target.TakeMagicalDamage(damage);
        }
    }
}