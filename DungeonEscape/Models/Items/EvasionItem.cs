using System;
using DungeonEscape.Models;

namespace DungeonEscape.Models.Items
{
    /// <summary>
    /// Consumable that grants a temporary dodge bonus.
    /// bonus = 0.5 means +50% dodge.
    /// </summary>
    public class EvasionItem : BaseItem
    {
        public double Bonus { get; }
        public int Duration { get; }

        public EvasionItem(string name, double bonus, int durationTurns, string description = "")
            : base(name, description, ItemTarget.Self, isConsumable: true, charges: 1)
        {
            Bonus = bonus;
            Duration = durationTurns;
        }

        public override bool Use(BaseCharacter user, BaseCharacter? target = null)
        {
            var recipient = target ?? user;

            if (!recipient.IsAlive)
            {
                Console.WriteLine($"{user.Name} cannot use {Name} on {recipient.Name} (not alive).");
                return false;
            }

            recipient.ApplyTemporaryStatBonus(StatType.Dodge, Bonus, Duration);
            Console.WriteLine($"{user.Name} uses {Name} on {recipient.Name}: +{Bonus:P0} dodge for {Duration} turn(s).");
            return true;
        }
    }
}