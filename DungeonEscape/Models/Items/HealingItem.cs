using DungeonEscape.Models;

namespace DungeonEscape.Models.Items
{
    /// <summary>
    /// Simple healing consumable.
    /// </summary>
    public class HealingItem : BaseItem
    {
        public int HealAmount { get; }

        // By default healing items are consumable and single‑use (Charges = 1).
        public HealingItem(string name, int healAmount, string description = "", bool consumable = true, int? charges = 1)
            : base(name, description, ItemTarget.Any, isConsumable: consumable, charges: charges)
        {
            HealAmount = healAmount;
        }

        public override bool Use(BaseCharacter user, BaseCharacter? target = null)
        {
            var recipient = target ?? user;

            if (!recipient.IsAlive)
            {
                System.Console.WriteLine($"{user.Name} cannot use {Name} on {recipient.Name} (not alive).");
                return false;
            }

            recipient.Heal(HealAmount);
            System.Console.WriteLine($"{user.Name} uses {Name} on {recipient.Name} and restores {HealAmount} HP.");
            return true;
        }
    }
}