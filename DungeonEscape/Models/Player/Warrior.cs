using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DungeonEscape.Models.Spells;
using ResourceType = DungeonEscape.Models.ResourceType;

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

        /// <summary>
        /// Known abilities (spells) for the warrior.
        /// </summary>
        public List<BaseSpell> Abilities { get; private set; }

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

            Abilities = new List<BaseSpell>();
            InitializeAbilities();
        }

        /// <summary>
        /// Initializes the warrior's starting abilities.
        /// Can be customized or extended in derived classes.
        /// </summary>
        protected virtual void InitializeAbilities()
        {
            LearnAbility(new Execute());
            LearnAbility(new HeroicStrike());
            LearnAbility(new Whirlwind());
        }

        /// <summary>
        /// Adds a new ability to the warrior's repertoire.
        /// </summary>
        /// <param name="ability">The ability to learn</param>
        public void LearnAbility(BaseSpell ability)
        {
            if (ability == null)
            {
                Console.WriteLine("Cannot learn a null ability.");
                return;
            }

            if (ability.RequiredResourceType != ResourceType.Rage && ability.RequiredResourceType != ResourceType.None)
            {
                Console.WriteLine($"{Name} cannot learn {ability.Name} - requires {ability.RequiredResourceType}!");
                return;
            }

            Abilities.Add(ability);
            Console.WriteLine($"{Name} learned {ability.Name}!");
        }

        /// <summary>
        /// Uses an ability from the warrior's repertoire by name.
        /// </summary>
        /// <param name="abilityName">Name of the ability to use</param>
        /// <param name="target">Target of the ability</param>
        /// <returns>True if ability was used successfully</returns>
        public bool UseAbility(string abilityName, BaseCharacter target)
        {
            var ability = Abilities.FirstOrDefault(a => a.Name.Equals(abilityName, StringComparison.OrdinalIgnoreCase));

            if (ability == null)
            {
                Console.WriteLine($"{Name} doesn't know the ability '{abilityName}'!");
                return false;
            }

            return ability.Cast(this, target);
        }

        /// <summary>
        /// Uses an ability directly.
        /// </summary>
        /// <param name="ability">The ability to use</param>
        /// <param name="target">Target of the ability</param>
        /// <returns>True if ability was used successfully</returns>
        public bool UseAbility(BaseSpell ability, BaseCharacter target)
        {
            return ability.Cast(this, target);
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

        protected override void TakePhysicalDamage(int damage)
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
        /// Allow items to add rage via AddResource.
        /// </summary>
        public override void AddResource(int amount)
        {
            GenerateRage(amount);
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
            Console.WriteLine($"Known Abilities: {Abilities.Count}");
        }

        /// <summary>
        /// Shows all abilities in the warrior's repertoire.
        /// </summary>
        public void ShowAbilities()
        {
            Console.WriteLine($"\n=== {Name}'s Abilities ===");

            if (Abilities.Count == 0)
            {
                Console.WriteLine("No abilities learned yet.");
                return;
            }

            for (int i = 0; i < Abilities.Count; i++)
            {
                var ability = Abilities[i];
                Console.WriteLine($"\n{i + 1}. {ability.Name}");
                Console.WriteLine($"   {ability.Description}");
                Console.WriteLine($"   Cost: {ability.ResourceCost} {ability.RequiredResourceType}");
                Console.WriteLine($"   Type: {ability.SpellDamageType} damage");
            }
        }
    }
}