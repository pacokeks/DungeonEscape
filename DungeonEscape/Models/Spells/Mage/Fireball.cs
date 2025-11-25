using System;

namespace DungeonEscape.Models.Spells
{
    public class Fireball : BaseSpell
    {
        private readonly int baseDamage;
        private readonly double spellPowerScaling;

        public Fireball(int baseDamage = 60, double spellPowerScaling = 1.0)
            : base(
                name: "Fireball",
                description: "Hurls a fiery ball that explodes on impact, dealing heavy fire damage.",
                resourceType: ResourceType.Mana,
                resourceCost: 50,
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

            Console.WriteLine($"  💥 A blazing fireball strikes {target.Name}!");
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