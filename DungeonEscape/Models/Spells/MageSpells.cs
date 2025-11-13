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
            //target.TakeMagicalDamage(totalDamage);
        }

        public override void ShowSpellInfo()
        {
            base.ShowSpellInfo();
            Console.WriteLine($"Base Damage: {baseDamage}");
            Console.WriteLine($"Scaling: {spellPowerScaling * 100}% of Spell Power");
        }
    }

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

    public class ArcaneMissiles : BaseSpell
    {
        private readonly int baseDamage;
        private readonly int numberOfMissiles;

        public ArcaneMissiles(int baseDamage = 15, int numberOfMissiles = 3)
            : base(
                name: "Arcane Missiles",
                description: "Fires multiple arcane missiles at the target.",
                resourceType: ResourceType.Mana,
                resourceCost: 20,
                damageType: DamageType.Magical
            )
        {
            this.baseDamage = baseDamage;
            this.numberOfMissiles = numberOfMissiles;
        }

        protected override void ExecuteSpellEffect(BaseCharacter caster, BaseCharacter target)
        {
            int spellPower = GetSpellPower(caster);
            int damagePerMissile = baseDamage + (spellPower / 4);

            Console.WriteLine($"  ✨ {numberOfMissiles} arcane missiles strike {target.Name}!");

            for (int i = 0; i < numberOfMissiles; i++)
            {
                if (target.IsAlive)
                {
                    // Use generic TakeDamage with magical type
                    target.TakeDamage(damagePerMissile, DamageType.Magical);
                }
            }
        }

        public override void ShowSpellInfo()
        {
            base.ShowSpellInfo();
            Console.WriteLine($"Missiles: {numberOfMissiles}");
            Console.WriteLine($"Damage per Missile: {baseDamage}");
        }
    }
}
