using System;
using DungeonEscape;
using DungeonEscape.Models;
using DungeonEscape.Models.Player;

namespace DungeonEscape.Models.Spells
{
    /// <summary>
    /// Fireball - a basic spell that deals fire damage to a target.
    /// To be implemented: damage over time after 3 casts, target will be standing in lava.
    /// </summary>
    public class FireBall : BaseSpell
    {
        /// <summary>
        /// Base damage of the Fireball spell.
        /// </summary>
        private int baseDamage = 30;
        /// <summary>
        /// Number of times the spell has been cast.
        /// </summary>
        private int castTimes = 0;
        /// <summary>
        /// Constructor for the Fireball spell.
        /// </summary>
        public FireBall() : base(name: "Fireball", resourceType: ResourceType.Mana, SpellType.Magic, resourceCost: 10, cooldownTurns: 2)
        {
            // Initialize the spell with its properties
            //Name = "Fireball";
            //ResourceType = ResourceType.Mana;
            //ResourceCost = 10;
            //CooldownTurns = 2;
        }

        /// <summary>
        /// Casts a fireball at the target, dealing elemental fire damage.
        /// </summary>
        /// <param name="caster">The character casting the spell</param>
        /// <param name="target">The target to receive the damage</param>
        /// <exception cref="InvalidOperationException"></exception>
        public override void Cast(BaseCharacter caster, BaseCharacter target)
        {
            // Check if the spell is on cooldown
            if (isOnCooldown())
            {
                Console.WriteLine($"{Name} is on cooldown for {CurrentCooldown} turns.");
                return;
            }


            // Check if the caster is a Mage
            if (caster is Mage mage)
            {
                //// Check if the caster is a Mage
                //if (castTimes < 3)
                //{
                //    // Increase the cast times
                //    castTimes++;
                //    Console.WriteLine($"{caster.Name} casts {Name} on {target.Name}, dealing {baseDamage} damage!");
                //}
                //else
                //{
                //    // Reset the cast times and apply damage over time
                //    castTimes = 0;
                //    Console.WriteLine($"{caster.Name} casts {Name} on {target.Name}, dealing {baseDamage} damage over time!");
                //}

                // Check if resources are consumed
                if (ConsumeResources(caster))
                {
                    // Calculate damage based on the base damage
                    int damage = (int)(baseDamage); // Fixed the calculation
                                                                            // Apply the damage to the target
                    target.TakeDamage(damage);
                    // Set the cooldown
                    CurrentCooldown = CooldownTurns;
                    Console.WriteLine($"{caster.Name} casts {Name} on {target.Name}, dealing {damage - target.MagicResistance} damage!");
                }
                // Check if the caster has enough resources to cast the spell
                else
                {
                    // Not enough mana to use the ability
                    Console.WriteLine($"{caster.Name} does not have enough resources to cast {Name}.");
                    return;
                }
            }
            // Check if the caster is not a Mage
            else
            {
                throw new InvalidOperationException("Only a Mage can cast Fireball.");
            }
        }
    }
}