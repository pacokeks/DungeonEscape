using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonEscape.Models.Player;

namespace DungeonEscape.Models.Spells
{
    public class BerserkerStrike : BaseSpell
    {
        private float damageMultiplier = 1.5f;

        public BerserkerStrike() : base(name: "Berserker Strike", resourceType: ResourceType.Rage, SpellType.Physical, resourceCost: 20, cooldownTurns: 3)
        {
            //// Initialize the spell with its properties
            //Name = "Berserker Strike";
            //ResourceType = ResourceType.Rage;
            //ResourceCost = 20;
            //CooldownTurns = 3;
        }

        public override void Cast(BaseCharacter caster, BaseCharacter target)
        {
            if (caster is Warrior warrior)
            {
                if (isOnCooldown())
                {
                    Console.WriteLine($"{Name} is on cooldown for {CurrentCooldown} turns.");
                    return;
                }

                else if (caster.TryConsumeResources(ResourceType, ResourceCost))
                {
                    // Calculate damage based on the caster's attack power and the damage multiplier
                    int damage = (int)(warrior.CalculateAttackDamage() * damageMultiplier);

                    // Apply the damage to the target
                    target.TakeDamage(damage);

                    // Set the cooldown
                    CurrentCooldown = CooldownTurns;

                    Console.WriteLine($"{caster.Name} uses {Name} on {target.Name}, dealing {damage - target.Defense} damage!");
                }

                else
                {
                    // Not enough rage to use the ability
                    Console.WriteLine($"{caster.Name} does not have enough resources to cast {Name}.");
                    return;
                }
            }
            else
            {
                throw new InvalidOperationException("Only a Warrior can cast Berserker Strike.");
            }

        }
    }
}
