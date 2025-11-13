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
    /// Warrior class that uses Rage as primary resource.
    /// Warriors start with 0 rage and generate it through combat (attacking and taking damage).
    /// </summary>
    public class Warrior : BaseCharacter
    {
        /// <summary>
        /// Current rage points. Private set - only this class manages rage.
        /// </summary>
        public int Rage { get; private set; }

        /// <summary>
        /// Maximum rage points. Set during construction.
        /// </summary>
        public int MaxRage { get; private set; }

        /// <summary>
        /// Strength attribute that increases physical damage.
        /// </summary>
        public int Strength { get; private set; }

        // Implement abstract properties from base class
        public override ResourceType PrimaryResourceType => ResourceType.Rage;
        public override int CurrentResource => Rage;
        public override int MaxResource => MaxRage;

        /// <summary>
        /// Creates a new Warrior with the provided base stats.
        /// </summary>
        /// <param name="in_name">Character name</param>
        /// <param name="in_health">Initial and maximum health points</param>
        /// <param name="in_defense">Base defense attribute</param>
        /// <param name="in_magicResistance">Base magic resistance attribute</param>
        /// <param name="in_maxRage">Maximum rage this warrior can have</param>
        /// <param name="in_strength">Base strength that modifies physical damage</param>
        public Warrior(string in_name, int in_health, int in_defense, int in_magicResistance, int in_maxRage, int in_strength)
            : base(in_name, in_health, in_defense, in_magicResistance)
        {
            MaxRage = in_maxRage;
            Rage = 0; // Warriors start with 0 rage and generate it through combat
            Strength = in_strength;
            BaseAttack = BaseAttack + Strength / 2; // Warriors have higher base attack due to strength
        }

        /// <summary>
        /// Attempts to consume the specified amount of rage.
        /// Returns true if enough rage was available and consumed.
        /// </summary>
        /// <param name="usage_amount">Amount of rage to consume</param>
        /// <returns>True if consumption succeeded, false otherwise</returns>
        public override bool TryConsumeResource(int usage_amount)
        {
            if (Rage >= usage_amount)
            {
                Rage -= usage_amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the basic physical attack damage for the warrior.
        /// Warriors deal strong physical damage.
        /// </summary>
        /// <returns>Calculated damage amount</returns>
        public override int CalculateAttackDamage()
        {
            // Warrior's attack uses the modified base attack (which includes strength bonus)
            return BaseAttack + Strength;
        }

        /// <summary>
        /// Hook implementation: Warriors generate rage after successful normal attacks.
        /// This is called automatically by NormalAttack() in the base class.
        /// </summary>
        /// <param name="target">The target that was attacked</param>
        /// <param name="damageDealt">The amount of damage that was dealt</param>
        protected override void OnAfterNormalAttack(BaseCharacter target, int damageDealt)
        {
            // Generate rage when attacking
            GenerateRage(15);
        }

        /// <summary>
        /// Warriors also generate rage when taking damage.
        /// Override TakePhysicalDamage to add this behavior.
        /// </summary>
        /// <param name="damage">The raw damage amount before defense calculation</param>
        public override void TakePhysicalDamage(int damage)
        {
            base.TakePhysicalDamage(damage); // Call base implementation first

            if (IsAlive)
            {
                // Generate rage based on damage taken
                int rageFromDamage = Math.Max(5, damage / 10);
                GenerateRage(rageFromDamage);
            }
        }

        /// <summary>
        /// Generates rage for the warrior, capped at MaxRage.
        /// </summary>
        /// <param name="regen_amount">Amount of rage to generate</param>
        public void GenerateRage(int regen_amount)
        {
            int oldRage = Rage;
            Rage = Math.Min(MaxRage, Rage + regen_amount);

            if (Rage > oldRage)
            {
                Console.WriteLine($"{Name} generates {Rage - oldRage} rage. Rage: {Rage}/{MaxRage}");
            }
        }

        /// <summary>
        /// Displays warrior-specific stats in addition to base stats.
        /// </summary>
        public override void ShowStats()
        {
            // Call base implementation first
            base.ShowStats();

            // Add warrior-specific stats
            Console.WriteLine($"Strength: {Strength}");
        }

        /// <summary>
        /// Powerful execute ability that costs rage and deals devastating damage.
        /// </summary>
        /// <param name="target">The character to execute</param>
        public void Execute(BaseCharacter target)
        {
            const int rageCost = 40;
            const int baseDamage = 80;

            if (!IsAlive)
            {
                Console.WriteLine($"{Name} is defeated and cannot use Execute!");
                return;
            }

            if (target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} has no valid target for Execute!");
                return;
            }

            if (!TryConsumeResource(rageCost))
            {
                Console.WriteLine($"{Name} doesn't have enough rage for Execute! ({Rage}/{rageCost})");
                return;
            }

            int damage = baseDamage + (Strength * 2);
            Console.WriteLine($"{Name} executes {target.Name} for {damage} devastating damage!");
            target.TakePhysicalDamage(damage);
        }

        /// <summary>
        /// Heroic Strike - Medium rage cost, medium damage boost.
        /// </summary>
        /// <param name="target">The character to strike</param>
        public void HeroicStrike(BaseCharacter target)
        {
            const int rageCost = 25;
            const int baseDamage = 50;

            if (!IsAlive)
            {
                Console.WriteLine($"{Name} is defeated and cannot use Heroic Strike!");
                return;
            }

            if (target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} has no valid target for Heroic Strike!");
                return;
            }

            if (!TryConsumeResource(rageCost))
            {
                Console.WriteLine($"{Name} doesn't have enough rage for Heroic Strike! ({Rage}/{rageCost})");
                return;
            }

            int damage = baseDamage + Strength;
            Console.WriteLine($"{Name} uses Heroic Strike on {target.Name} for {damage} damage!");
            target.TakePhysicalDamage(damage);
        }
    }
}