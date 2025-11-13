using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ResourceType = DungeonEscape.Models.ResourceType;

namespace DungeonEscape.Models
{
    public abstract class BaseCharacter
    {
        /// <summary>
        /// The character's name. Can only be set during initialization.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Current health points of the character. When it reaches 0, the character is defeated.
        /// Protected set ensures only derived classes can modify it.
        /// </summary>
        public int Health { get; protected set; }

        /// <summary>
        /// Maximum possible health points for this character.
        /// Protected set allows derived classes to modify it (e.g., buffs/debuffs).
        /// </summary>
        public int MaxHealth { get; protected set; }

        /// <summary>
        /// The character's defense attribute, reduces physical damage taken.
        /// Protected set allows derived classes to modify it (e.g., buffs/debuffs).
        /// </summary>
        public int Defense { get; protected set; }

        /// <summary>
        /// The character's Magic Resistance attribute, reduces damage from magical attacks.
        /// Protected set allows derived classes to modify it (e.g., buffs/debuffs).
        /// </summary>
        public int MagicResistance { get; protected set; }

        /// <summary>
        /// The character's BaseAttack attribute, the initial character's raw attack power.
        /// Protected set allows derived classes to modify it (e.g., buffs/debuffs).
        /// </summary>
        public int BaseAttack { get; protected set; } = 5;

        /// <summary>
        /// Returns true if the character's health is above 0, false otherwise.
        /// </summary>
        public bool IsAlive => Health > 0;

        /// <summary>
        /// Gets the type of resource this character uses (Mana, Rage, Energy, or None).
        /// Abstract property forces derived classes to define their resource type.
        /// </summary>
        public abstract ResourceType PrimaryResourceType { get; }

        /// <summary>
        /// Gets the current amount of the character's primary resource.
        /// Returns 0 if the character has no resource system.
        /// </summary>
        public abstract int CurrentResource { get; }

        /// <summary>
        /// Gets the maximum amount of the character's primary resource.
        /// Returns 0 if the character has no resource system.
        /// </summary>
        public abstract int MaxResource { get; }

        /// <summary>
        /// Constructor for creating a character with base stats.
        /// Protected access modifier ensures only derived classes can instantiate it.
        /// </summary>
        /// <param name="in_name">The name of the character</param>
        /// <param name="in_health">Initial and maximum health points</param>
        /// <param name="in_defense">Base defense attribute</param>
        /// <param name="in_magicResistance">Base magic resistance attribute</param>
        protected BaseCharacter(string in_name, int in_health, int in_defense, int in_magicResistance)
        {
            Name = in_name;
            MaxHealth = in_health;
            Health = MaxHealth; // Start with full health
            Defense = in_defense;
            MagicResistance = in_magicResistance;
        }

        // ---------- Simple defend flag ----------
        private bool _isDefending;

        /// <summary>
        /// True while the character has an active defend status.
        /// </summary>
        public bool IsDefending => _isDefending;

        /// <summary>
        /// Enter defensive stance. While defending, effective Defense and MagicResistance are doubled for damage calculations.
        /// This method only toggles a flag — no permanent stat changes.
        /// </summary>
        public void EnterDefend()
        {
            if (_isDefending) return;
            _isDefending = true;
            Console.WriteLine($"{Name} assumes a defensive stance (next incoming damage will use doubled defense values).");
        }

        /// <summary>
        /// Exit defensive stance. Restores normal damage calculations.
        /// </summary>
        public void ExitDefend()
        {
            if (!_isDefending) return;
            _isDefending = false;
            Console.WriteLine($"{Name} stops defending.");
        }

        /// <summary>
        /// Reduces the character's health based on incoming physical damage and defense value.
        /// If IsDefending is true, Defense is doubled for the calculation (only for this calculation).
        /// </summary>
        /// <param name="damage">The raw damage amount before defense calculation</param>
        protected virtual void TakePhysicalDamage(int damage)
        {
            int effectiveDefense = _isDefending ? Defense * 2 : Defense;
            int actualDamage = Math.Max(1, damage - effectiveDefense);

            Health = Math.Max(0, Health - actualDamage);
            Console.WriteLine($"{Name} takes {actualDamage} physical damage. Health: {Health}/{MaxHealth}");
        }

        /// <summary>
        /// Reduces the character's health based on incoming magical damage and magic resistance.
        /// If IsDefending is true, MagicResistance is doubled for the calculation (only for this calculation).
        /// </summary>
        /// <param name="damage">The raw magical damage amount before magic resistance calculation</param>
        protected virtual void TakeMagicalDamage(int damage)
        {
            int effectiveMagicRes = _isDefending ? MagicResistance * 2 : MagicResistance;
            int actualDamage = Math.Max(1, damage - effectiveMagicRes);

            Health = Math.Max(0, Health - actualDamage);
            Console.WriteLine($"{Name} takes {actualDamage} magical damage. Health: {Health}/{MaxHealth}");
        }

        /// <summary>
        /// Generic damage method that applies the appropriate resistance.
        /// Automatically routes to TakePhysicalDamage or TakeMagicalDamage based on type.
        /// Call ExitDefend() after the hit if you want defend to only affect a single incoming hit.
        /// </summary>
        /// <param name="damage">Raw damage amount</param>
        /// <param name="damageType">Type of damage being dealt</param>
        public virtual void TakeDamage(int damage, DamageType damageType = DamageType.Physical)
        {
            if (damageType == DamageType.Physical)
                TakePhysicalDamage(damage);
            else
                TakeMagicalDamage(damage);
        }

        /// <summary>
        /// Performs a normal attack on a target. This is available to all characters
        /// and does not consume resources. Concrete implementation in base class.
        /// </summary>
        /// <param name="target">The character to attack</param>
        public virtual void NormalAttack(BaseCharacter target)
        {
            if (!IsAlive)
            {
                Console.WriteLine($"{Name} is defeated and cannot attack!");
                return;
            }

            if (target == null)
            {
                Console.WriteLine($"{Name} has no valid target!");
                return;
            }

            if (!target.IsAlive)
            {
                Console.WriteLine($"{target.Name} is already defeated!");
                return;
            }

            int damage = CalculateAttackDamage();
            Console.WriteLine($"{Name} attacks {target.Name} for {damage} damage!");

            target.TakeDamage(damage, DamageType.Physical);

            // Hook for derived classes to add effects after attacking
            OnAfterNormalAttack(target, damage);
        }

        /// <summary>
        /// Hook method that derived classes can override to add effects after a normal attack.
        /// For example, Warriors generate rage, Rogues might add combo points, etc.
        /// Base implementation does nothing.
        /// </summary>
        /// <param name="target">The target that was attacked</param>
        /// <param name="damageDealt">The amount of damage that was dealt</param>
        protected virtual void OnAfterNormalAttack(BaseCharacter target, int damageDealt)
        {
            // Base implementation does nothing - override in derived classes to add effects
        }

        /// <summary>
        /// Attempts to consume the specified amount of the character's primary resource.
        /// Abstract method forces each derived class to implement their own resource consumption logic.
        /// </summary>
        /// <param name="amount">Amount of resource to consume</param>
        /// <returns>True if resources were consumed successfully, false if insufficient resources</returns>
        public abstract bool TryConsumeResource(int amount);

        /// <summary>
        /// Calculates the damage this character deals with a basic attack.
        /// Abstract method forces each derived class to implement their own damage calculation.
        /// </summary>
        /// <returns>The calculated damage amount</returns>
        public abstract int CalculateAttackDamage();

        /// <summary>
        /// Displays the character's current stats.
        /// Virtual allows derived classes to add class-specific stats.
        /// </summary>
        public virtual void ShowStats()
        {
            Console.WriteLine($"=== {Name} Stats ===");
            Console.WriteLine($"Health: {Health}/{MaxHealth}");
            Console.WriteLine($"Defense: {Defense}");
            Console.WriteLine($"Magic Resistance: {MagicResistance}");

            // Only show resource if the character actually uses one
            if (PrimaryResourceType != ResourceType.None)
            {
                Console.WriteLine($"{PrimaryResourceType}: {CurrentResource}/{MaxResource}");
            }

            Console.WriteLine($"Defending: {IsDefending}");
            Console.WriteLine($"Status: {(IsAlive ? "Alive" : "Defeated")}");
        }

        /// <summary>
        /// Heals the character by the specified amount, not exceeding MaxHealth.
        /// Sealed prevents derived classes from overriding - healing logic should be consistent.
        /// </summary>
        /// <param name="amount">Amount of health to restore</param>
        public void Heal(int amount)
        {
            if (!IsAlive)
            {
                Console.WriteLine($"{Name} is defeated and cannot be healed.");
                return;
            }

            int oldHealth = Health;
            Health = Math.Min(MaxHealth, Health + amount);
            int actualHealing = Health - oldHealth;

            Console.WriteLine($"{Name} is healed for {actualHealing} HP. Health: {Health}/{MaxHealth}");
        }
    }
}