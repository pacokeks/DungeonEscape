using System;

namespace DungeonEscape.Models.Spells
{
    public class ArcaneMissiles : BaseSpell
    {
        private readonly int baseDamage;
        private readonly int numberOfMissiles;
        private static readonly Random rng = new Random();

        /// <summary>
        /// If numberOfMissiles is null, a random value between 2 and 4 (inclusive) is chosen.
        /// Pass an explicit number to use a fixed count.
        /// </summary>
        public ArcaneMissiles(int baseDamage = 15, int? numberOfMissiles = null)
            : base(
                name: "Arcane Missiles",
                description: "Fires multiple arcane missiles at the target.",
                resourceType: ResourceType.Mana,
                resourceCost: 20,
                damageType: DamageType.Magical
            )
        {
            this.baseDamage = baseDamage;
            if (numberOfMissiles.HasValue && numberOfMissiles.Value > 0)
            {
                this.numberOfMissiles = numberOfMissiles.Value;
            }
            else
            {
                this.numberOfMissiles = rng.Next(2, 5); // 2..4
            }
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