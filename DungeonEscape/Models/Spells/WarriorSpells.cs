using System;

namespace DungeonEscape.Models.Spells
{
    public class Execute : BaseSpell
    {
        private readonly int baseDamage;
        private readonly double strengthScaling;

        public Execute(int baseDamage = 60, double strengthScaling = 2.0)
            : base(
                name: "Execute",
                description: "A devastating finishing move that deals massive damage.",
                resourceType: ResourceType.Rage,
                resourceCost: 40,
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

            double targetHealthPercent = (double)target.Health / target.MaxHealth;
            if (targetHealthPercent < 0.3)
            {
                totalDamage = (int)(totalDamage * 1.5);
                Console.WriteLine($"  ⚔️ EXECUTE! A devastating blow strikes {target.Name} while weakened!");
            }
            else
            {
                Console.WriteLine($"  ⚔️ {caster.Name} executes {target.Name}!");
            }

            target.TakeDamage(totalDamage, DamageType.Physical);
        }

        public override void ShowSpellInfo()
        {
            base.ShowSpellInfo();
            Console.WriteLine($"Base Damage: {baseDamage}");
            Console.WriteLine($"Scaling: {strengthScaling * 100}% of Strength");
            Console.WriteLine($"Special: 50% more damage if target below 30% health");
        }
    }

    public class HeroicStrike : BaseSpell
    {
        private readonly int baseDamage;
        private readonly double strengthScaling;

        public HeroicStrike(int baseDamage = 50, double strengthScaling = 1.0)
            : base(
                name: "Heroic Strike",
                description: "A powerful strike that deals solid physical damage.",
                resourceType: ResourceType.Rage,
                resourceCost: 25,
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

            Console.WriteLine($"  🗡️ {caster.Name} strikes heroically!");
            target.TakeDamage(totalDamage, DamageType.Physical);
        }

        public override void ShowSpellInfo()
        {
            base.ShowSpellInfo();
            Console.WriteLine($"Base Damage: {baseDamage}");
            Console.WriteLine($"Scaling: {strengthScaling * 100}% of Strength");
        }
    }

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

        public override void ShowSpellInfo()
        {
            base.ShowSpellInfo();
            Console.WriteLine($"Base Damage: {baseDamage}");
            Console.WriteLine($"Scaling: {strengthScaling * 100}% of Strength");
            Console.WriteLine($"Special: Hits all nearby enemies");
        }
    }
}