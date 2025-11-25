using System;

namespace DungeonEscape.Models.Spells
{
    public class Frostbolt : BaseSpell
    {
        private readonly int baseDamage;
        private readonly double spellPowerScaling;

        public Frostbolt(int baseDamage = 40, double spellPowerScaling = 0.8)
            : base(
                name: "Frostbolt",
                description: "Launches a bolt of frost at the enemy, dealing moderate cold damage.",
                resourceType: ResourceType.Mana,
                resourceCost: 30,
                damageType: DamageType.Magical
            )
        {
            this.baseDamage = baseDamage;
            this.spellPowerScaling = spellPowerScaling;
        }

        protected override void ExecuteSpellEffect(BaseCharacter caster, BaseCharacter target)
        {
            int spellPower = GetSpellPower(caster);
            int totalDamage = baseDamage + (int)(spellPower * spellPowerScaling);

            Console.WriteLine($"  ❄️ A freezing bolt hits {target.Name}!");
            target.TakeDamage(totalDamage, DamageType.Magical);
        }

        public override void ShowSpellInfo()
        {
            base.ShowSpellInfo();
            Console.WriteLine($"Base Damage: {baseDamage}");
            Console.WriteLine($"Scaling: {spellPowerScaling * 100}% of Spell Power");
        }
    }
}