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

        /// <summary>
        /// Spellbook: all spells the mage knows.
        /// </summary>
        public List<BaseSpell> Spellbook { get; private set; }

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

            Spellbook = new List<BaseSpell>();
            InitializeSpellbook();
        }

        /// <summary>
        /// Initialize default spells for the mage.
        /// </summary>
        protected virtual void InitializeSpellbook()
        {
            LearnSpell(new Fireball());
            LearnSpell(new Frostbolt());
            LearnSpell(new ArcaneMissiles());
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
            Console.WriteLine($"Known Spells: {Spellbook.Count}");
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
        /// Learn a new spell. Validates resource type compatibility.
        /// </summary>
        public void LearnSpell(BaseSpell spell)
        {
            if (spell == null)
            {
                Console.WriteLine("Cannot learn a null spell.");
                return;
            }

            if (spell.RequiredResourceType != ResourceType.Mana && spell.RequiredResourceType != ResourceType.None)
            {
                Console.WriteLine($"{Name} cannot learn {spell.Name} - requires {spell.RequiredResourceType}!");
                return;
            }

            Spellbook.Add(spell);
            Console.WriteLine($"{Name} learned {spell.Name}!");
        }

        /// <summary>
        /// Cast a spell by name from the spellbook.
        /// </summary>
        public bool CastSpell(string spellName, BaseCharacter target)
        {
            if (string.IsNullOrWhiteSpace(spellName))
            {
                Console.WriteLine("Invalid spell name.");
                return false;
            }

            var spell = Spellbook.FirstOrDefault(s => s.Name.Equals(spellName, StringComparison.OrdinalIgnoreCase));
            if (spell == null)
            {
                Console.WriteLine($"{Name} doesn't know the spell '{spellName}'!");
                return false;
            }

            return spell.Cast(this, target);
        }

        /// <summary>
        /// Shows all spells in the mage's spellbook.
        /// </summary>
        public void ShowSpellbook()
        {
            Console.WriteLine($"\n=== {Name}'s Spellbook ===");
            if (Spellbook.Count == 0)
            {
                Console.WriteLine("No spells learned yet.");
                return;
            }

            for (int i = 0; i < Spellbook.Count; i++)
            {
                var s = Spellbook[i];
                Console.WriteLine($"\n{i + 1}. {s.Name}");
                Console.WriteLine($"   {s.Description}");
                Console.WriteLine($"   Cost: {s.ResourceCost} {s.RequiredResourceType}");
                Console.WriteLine($"   Type: {s.SpellDamageType} damage");
            }
        }
    }
}