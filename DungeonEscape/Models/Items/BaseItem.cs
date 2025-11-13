using System;
using System.Collections.Generic;
using System.Linq;
using DungeonEscape.Models;

namespace DungeonEscape.Models.Items
{
    /// <summary>
    /// Abstract base for all items.
    /// Implement Use(user, target) to apply item effects.
    /// </summary>
    public abstract class BaseItem
    {
        public string Name { get; init; }
        public string Description { get; init; }

        /// <summary>
        /// Which targets this item may be used on.
        /// UI / controller should enforce this.
        /// </summary>
        public ItemTarget AllowedTarget { get; protected init; } = ItemTarget.Any;

        /// <summary>
        /// If true the item is consumed (removed) after use.
        /// If false the item remains in inventory (like equipment).
        /// </summary>
        public bool IsConsumable { get; protected set; } = true;

        /// <summary>
        /// Optional number of uses (charges). Null = not tracked (use IsConsumable to decide).
        /// If Charges == 1 and IsConsumable == true the item is removed after one successful use.
        /// If Charges > 1 it will be decremented each successful use; when it reaches 0 the item is removed.
        /// </summary>
        public int? Charges { get; protected set; }

        protected BaseItem(string name, string description, ItemTarget allowedTarget = ItemTarget.Any, bool isConsumable = true, int? charges = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            AllowedTarget = allowedTarget;
            IsConsumable = isConsumable;
            Charges = charges;
        }

        /// <summary>
        /// Use the item. Return true if used successfully.
        /// user = the character using the item, target = optional target (can be same as user).
        /// Implement concrete effect in overrides.
        /// </summary>
        public abstract bool Use(BaseCharacter user, BaseCharacter? target = null);

        /// <summary>
        /// After a successful Use, call this to update charges/consumption.
        /// Returns true if the item should be removed from inventory.
        /// </summary>
        public bool ConsumeOneUse()
        {
            if (Charges.HasValue)
            {
                Charges = Math.Max(0, Charges.Value - 1);
                return Charges.Value == 0;
            }

            return IsConsumable;
        }

        /// <summary>
        /// Convenience helper for UI/Controller: validates whether a given target is acceptable for this item.
        /// allies may be provided to evaluate Friend targets.
        /// </summary>
        public bool CanTarget(BaseCharacter user, BaseCharacter? target = null, IEnumerable<BaseCharacter>? allies = null)
        {
            if (user == null)
            {
                return false;
            }

            var actualTarget = target ?? user;

            if (!actualTarget.IsAlive)
            {
                return false;
            }

            switch (AllowedTarget)
            {
                case ItemTarget.Self:
                    return ReferenceEquals(actualTarget, user);

                case ItemTarget.Enemy:
                    // Enemy means not the user and not one of allies (if allies provided)
                    if (ReferenceEquals(actualTarget, user))
                    {
                        return false;
                    }

                    if (allies != null)
                    {
                        if (allies.Cast<BaseCharacter>().Any(a => ReferenceEquals(a, actualTarget)))
                        {
                            return false;
                        }
                    }

                    return true;

                case ItemTarget.Friend:
                    if (ReferenceEquals(actualTarget, user))
                    {
                        return true;
                    }

                    if (allies != null)
                    {
                        return allies.Cast<BaseCharacter>().Any(a => ReferenceEquals(a, actualTarget));
                    }

                    return false;

                case ItemTarget.Any:
                default:
                    return true;
            }
        }

        public virtual void ShowInfo()
        {
            System.Console.WriteLine($"Item: {Name}");
            System.Console.WriteLine($"{Description}");
            System.Console.WriteLine($"Allowed Target: {AllowedTarget}");
            System.Console.WriteLine($"Consumable: {IsConsumable}  Charges: {(Charges.HasValue ? Charges.Value.ToString() : "∞")}");
        }
    }
}