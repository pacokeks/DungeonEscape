using DungeonEscape.Models;

namespace DungeonEscape.Models.Items
{
    /// <summary>
    /// Restores primary resource (Mana/Rage/Energy) when used.
    /// Uses BaseCharacter.AddResource to apply resource generically.
    /// </summary>
    public class ResourceItem : BaseItem
    {
        public int Amount { get; }

        // Resource items are consumable by default (single‑use)
        public ResourceItem(string name, int amount, string description = "", bool consumable = true, int? charges = 1)
            : base(name, description, ItemTarget.Self, isConsumable: consumable, charges: charges)
        {
            Amount = amount;
        }

        public override bool Use(BaseCharacter user, BaseCharacter? target = null)
        {
            var recipient = target ?? user;

            if (!recipient.IsAlive)
            {
                System.Console.WriteLine($"{user.Name} cannot use {Name} on {recipient.Name} (not alive).");
                return false;
            }

            recipient.AddResource(Amount);
            System.Console.WriteLine($"{user.Name} uses {Name} on {recipient.Name} and restores {Amount} resource.");
            return true;
        }
    }
}