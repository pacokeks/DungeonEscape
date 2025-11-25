using System;
using DungeonEscape.Models;

namespace DungeonEscape.Models.Spells
{
    /// <summary>
    /// Blink: temporarily raises caster's dodge to 100% for a number of turns.
    /// Uses the generic stat-buff API (ApplyTemporaryStatBonus).
    /// </summary>
    public class Blink : BaseSpell
    {
        private readonly int evadeDuration;

        public Blink(int evadeDuration = 2)
            : base(
                  name: "Blink",
                  description: "Increases the caster's dodge to 100% for a short duration, allowing the caster to evade all incoming attacks.",
                  resourceType: ResourceType.Mana,
                  resourceCost: 30,
                  damageType: DamageType.Magical)
        {
            this.evadeDuration = evadeDuration;
        }

        protected override void ExecuteSpellEffect(BaseCharacter caster, BaseCharacter target)
        {
            if (caster == null)
            {
                Console.WriteLine("No caster for Blink.");
                return;
            }

            // compute current total dodge and required bonus to reach 100%
            double before = caster.GetTotalDodge();
            double needed = 1.0 - before;
            if (needed <= 0.0)
            {
                Console.WriteLine($"{caster.Name} already has 100% (or more) dodge. Blink has no additional effect.");
                return;
            }

            double bonusToApply = Math.Min(needed, 1.0); // clamp
            caster.ApplyTemporaryStatBonus(StatType.Dodge, bonusToApply, evadeDuration);

            Console.WriteLine($"{caster.Name} casts Blink: Dodge increased from {before:P0} to {caster.GetTotalDodge():P0} (+{bonusToApply:P0}) for {evadeDuration} turn(s).");
        }

        // For multi-target invocations, Blink applies only to the caster
        protected override void ExecuteSpellEffect(BaseCharacter caster, IReadOnlyCollection<BaseCharacter> targets)
        {
            ExecuteSpellEffect(caster, caster);
        }

        public override void ShowSpellInfo()
        {
            base.ShowSpellInfo();
            Console.WriteLine($"Duration: {evadeDuration} turn(s)");
            Console.WriteLine("Effect: Sets dodge to 100% (temporary).");
        }
    }
}