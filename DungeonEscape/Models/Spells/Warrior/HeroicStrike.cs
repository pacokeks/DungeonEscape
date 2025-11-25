using System;

namespace DungeonEscape.Models.Spells
{
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
}