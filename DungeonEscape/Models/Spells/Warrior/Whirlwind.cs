using System;
using System.Collections.Generic;

namespace DungeonEscape.Models.Spells
{
    public class Whirlwind : BaseSpell
    {
        private readonly int baseDamage;
        private readonly double strengthScaling;

        public Whirlwind(int baseDamage = 35, double strengthScaling = 0.8)
            : base(
                name: "Whirlwind",
                description: "Spin in a circle, striking all nearby enemies.",
                resourceType: ResourceType.Rage,
                resourceCost: 30,
                damageType: DamageType.Physical
            )
        {
            this.baseDamage = baseDamage;
            this.strengthScaling = strengthScaling;
        }

        protected override void ExecuteSpellEffect(BaseCharacter caster, BaseCharacter target)
        {
            int strength = GetStrength(caster);
            int totalDamage = baseDamage + (int)(strength * strengthScaling);

            Console.WriteLine($"  🌀 {caster.Name} spins in a deadly whirlwind!");
            target.TakeDamage(totalDamage, DamageType.Physical);
        }

        protected override void ExecuteSpellEffect(BaseCharacter caster, IReadOnlyCollection<BaseCharacter> targets)
        {
            int strength = GetStrength(caster);
            int totalDamage = baseDamage + (int)(strength * strengthScaling);

            Console.WriteLine($"  🌀 {caster.Name} performs Whirlwind and hits {targets.Count} targets!");

            foreach (var t in targets)
            {
                if (t.IsAlive)
                {
                    t.TakeDamage(totalDamage, DamageType.Physical);
                }
            }
        }

        public override void ShowSpellInfo()
        {
            base.ShowSpellInfo();
            Console.WriteLine($"Base Damage: {baseDamage}");
            Console.WriteLine($"Scaling: {strengthScaling * 100}% of Strength");
            Console.WriteLine($"Special: Hits all nearby enemies");
        }
    }
}