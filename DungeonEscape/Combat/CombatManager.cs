using System;
using System.Linq;
using System.Collections.Generic;
using DungeonEscape.Models;
using DungeonEscape.Models.Player;
using DungeonEscape.Models.Spells;
using DungeonEscape.Models.Items;

namespace DungeonEscape.Combat
{
    public static class CombatManager
    {
        public static void RunInteractiveCombat(BaseCharacter player, BaseCharacter enemy)
        {
            if (player == null || enemy == null)
            {
                throw new ArgumentNullException();
            }

            var rnd = new Random();

            while (player.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine("\n=== New Turn ===");
                ShowBriefStatus(player, enemy);

                // PLAYER TURN
                if (!PlayerTurn(player, enemy))
                {
                    return; // allows quitting
                }

                if (!enemy.IsAlive)
                {
                    Console.WriteLine($"{player.Name} wins!");
                    break;
                }

                // ENEMY TURN (simple AI)
                EnemyTurn(enemy, player, rnd);

                if (!player.IsAlive)
                {
                    Console.WriteLine($"{enemy.Name} wins!");
                    break;
                }

                // ---- TICK BUFFS ONCE PER ROUND (after both acted) ----
                player.TickTurn();
                enemy.TickTurn();
            }
        }

        private static void ShowBriefStatus(BaseCharacter player, BaseCharacter enemy)
        {
            var playerResource = player.PrimaryResourceType != ResourceType.None
                ? $"{player.PrimaryResourceType}: {player.CurrentResource}/{player.MaxResource}"
                : string.Empty;

            var enemyResource = enemy.PrimaryResourceType != ResourceType.None
                ? $"{enemy.PrimaryResourceType}: {enemy.CurrentResource}/{enemy.MaxResource}"
                : string.Empty;

            Console.WriteLine($"{player.Name}: HP {player.Health}/{player.MaxHealth}" +
                              $"{(string.IsNullOrEmpty(playerResource) ? string.Empty : $" | {playerResource}")}  Defending: {player.IsDefending}");

            Console.WriteLine($"{enemy.Name}: HP {enemy.Health}/{enemy.MaxHealth}" +
                              $"{(string.IsNullOrEmpty(enemyResource) ? string.Empty : $" | {enemyResource}")}  Defending: {enemy.IsDefending}");

            Console.WriteLine();
        }

        private static bool PlayerTurn(BaseCharacter player, BaseCharacter enemy)
        {
            while (true)
            {
                Console.WriteLine("Choose action: 1) Attack  2) Abilities/Spells  3) Defend  4) Status  5) Skip  Q) Quit");
                Console.Write("Action: ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                switch (input)
                {
                    case "1":
                        {
                            player.NormalAttack(enemy);

                            // defend affects damage calculation only; restore defend state on target after hit
                            if (enemy.IsDefending)
                            {
                                enemy.ExitDefend();
                            }

                            return true;
                        }

                    case "2":
                        {
                            UsePlayerAbility(player, enemy);

                            // if enemy was defending, restore after spell/ability
                            if (enemy.IsDefending)
                            {
                                enemy.ExitDefend();
                            }

                            return true;
                        }

                    case "3":
                        {
                            // use current BaseCharacter defend API
                            player.EnterDefend();
                            return true;
                        }

                    case "4":
                        {
                            player.ShowStats();
                            enemy.ShowStats();
                            break;
                        }

                    case "5":
                        {
                            Console.WriteLine($"{player.Name} skips.");
                            return true;
                        }

                    default:
                        {
                            Console.WriteLine("Invalid choice.");
                            break;
                        }
                }
            }
        }

        private static void UsePlayerAbility(BaseCharacter player, BaseCharacter enemy)
        {
            // Use switch type-pattern to avoid ambiguous if-chains
            switch (player)
            {
                case Mage mage:
                    {
                        if (!mage.Spellbook.Any())
                        {
                            Console.WriteLine("No spells known.");
                            return;
                        }

                        for (int i = 0; i < mage.Spellbook.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {mage.Spellbook[i].Name} - Cost {mage.Spellbook[i].ResourceCost} {mage.Spellbook[i].RequiredResourceType}");
                        }

                        Console.Write("Select spell (0 cancel): ");
                        if (!int.TryParse(Console.ReadLine(), out int sIdx) || sIdx < 0 || sIdx > mage.Spellbook.Count)
                        {
                            Console.WriteLine("Invalid");
                            return;
                        }

                        if (sIdx == 0)
                        {
                            return;
                        }

                        var spell = mage.Spellbook[sIdx - 1];
                        // If spell supports multi-target and should hit all enemies, CombatManager caller can use Cast(caster, IEnumerable<BaseCharacter>)
                        spell.Cast(mage, enemy);
                        return;
                    }

                case Warrior war:
                    {
                        if (!war.Abilities.Any())
                        {
                            Console.WriteLine("No abilities known.");
                            return;
                        }

                        for (int i = 0; i < war.Abilities.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}) {war.Abilities[i].Name} - Cost {war.Abilities[i].ResourceCost} {war.Abilities[i].RequiredResourceType}");
                        }

                        Console.Write("Select ability (0 cancel): ");
                        if (!int.TryParse(Console.ReadLine(), out int aIdx) || aIdx < 0 || aIdx > war.Abilities.Count)
                        {
                            Console.WriteLine("Invalid");
                            return;
                        }

                        if (aIdx == 0)
                        {
                            return;
                        }

                        var ability = war.Abilities[aIdx - 1];

                        // If ability is AoE (e.g. Whirlwind), call multi-target overload
                        if (ability is Whirlwind ww)
                        {
                            ww.Cast(war, new List<BaseCharacter> { enemy }.Where(e => e != null).ToList());
                        }
                        else
                        {
                            ability.Cast(war, enemy);
                        }

                        return;
                    }

                default:
                    {
                        Console.WriteLine("No special actions available for this character.");
                        return;
                    }
            }
        }

        private static void EnemyTurn(BaseCharacter enemy, BaseCharacter player, Random rnd)
        {
            Console.WriteLine($"\n--- {enemy.Name}'s turn ---");

            // 20% defend chance
            if (rnd.NextDouble() < 0.20)
            {
                enemy.EnterDefend();
                return;
            }

            // prefer abilities if available and affordable (70% chance)
            if (enemy is Warrior ew)
            {
                var usable = ew.Abilities.Where(a => a.ResourceCost <= ew.CurrentResource).OrderByDescending(a => a.ResourceCost).ToList();

                if (usable.Any() && rnd.NextDouble() < 0.7)
                {
                    var choice = usable[rnd.Next(usable.Count)];

                    // enemy ability might be AoE (Whirlwind) - cast appropriately
                    if (choice is Whirlwind wwChoice)
                    {
                        wwChoice.Cast(ew, new List<BaseCharacter> { player }.Where(p => p.IsAlive).ToList());
                    }
                    else
                    {
                        choice.Cast(ew, player);
                    }

                    // if player was defending, restore after hit
                    if (player.IsDefending)
                    {
                        player.ExitDefend();
                    }

                    return;
                }
            }
            else if (enemy is Mage em)
            {
                var usable = em.Spellbook.Where(s => s.ResourceCost <= em.CurrentResource).OrderByDescending(s => s.ResourceCost).ToList();

                if (usable.Any() && rnd.NextDouble() < 0.7)
                {
                    var choice = usable[rnd.Next(usable.Count)];
                    choice.Cast(em, player);

                    if (player.IsDefending)
                    {
                        player.ExitDefend();
                    }

                    return;
                }
            }

            // fallback: normal attack
            enemy.NormalAttack(player);

            // restore defend if present (defend affects next incoming hit only)
            if (player.IsDefending)
            {
                player.ExitDefend();
            }
        }

        /// <summary>
        /// Prompt target selection for the given item. Respects the item's AllowedTarget.
        /// Shows friendly targets grouped first, then enemy targets. User enters a single number.
        /// allies parameter can be null or empty; user is always included in friendly list.
        /// </summary>
        public static BaseCharacter? PromptSelectTargetForItem(BaseCharacter user, BaseCharacter enemy, BaseItem item, IEnumerable<BaseCharacter>? allies = null)
        {
            if (item == null || user == null)
            {
                return null;
            }

            // Build lists
            var friends = new List<BaseCharacter>();
            friends.Add(user);

            if (allies != null)
            {
                foreach (var a in allies)
                {
                    if (a != null && a != user)
                    {
                        friends.Add(a);
                    }
                }
            }

            // Only include living targets
            friends = friends.Where(f => f.IsAlive).Distinct().ToList();

            var enemies = new List<BaseCharacter>();
            if (enemy != null && enemy.IsAlive)
            {
                enemies.Add(enemy);
            }

            // If item restricts allowed target, filter lists
            if (item.AllowedTarget == ItemTarget.Self)
            {
                if (user.IsAlive)
                {
                    return user;
                }

                return null;
            }
            else if (item.AllowedTarget == ItemTarget.Enemy)
            {
                if (!enemies.Any())
                {
                    return null;
                }

                return PromptChooseFromGroupedLists(friends: new List<BaseCharacter>(), enemies: enemies);
            }
            else if (item.AllowedTarget == ItemTarget.Friend)
            {
                if (!friends.Any())
                {
                    return null;
                }

                return PromptChooseFromGroupedLists(friends: friends, enemies: new List<BaseCharacter>());
            }
            else
            {
                return PromptChooseFromGroupedLists(friends: friends, enemies: enemies);
            }
        }

        // Helper - prints grouped lists with continuous numbering and returns chosen BaseCharacter or null
        private static BaseCharacter? PromptChooseFromGroupedLists(List<BaseCharacter> friends, List<BaseCharacter> enemies)
        {
            var mapping = new List<BaseCharacter>();
            int counter = 1;

            if (friends != null && friends.Count > 0)
            {
                Console.WriteLine("Friendly Targets:");
                foreach (var f in friends)
                {
                    Console.WriteLine($"{counter}. {f.Name}");
                    mapping.Add(f);
                    counter++;
                }
            }

            if (enemies != null && enemies.Count > 0)
            {
                if (mapping.Count > 0)
                {
                    Console.WriteLine();
                }

                Console.WriteLine("Enemy Targets:");
                foreach (var e in enemies)
                {
                    Console.WriteLine($"{counter}. {e.Name}");
                    mapping.Add(e);
                    counter++;
                }
            }

            if (mapping.Count == 0)
            {
                Console.WriteLine("No valid targets available.");
                return null;
            }

            Console.Write("Enter target number (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > mapping.Count)
            {
                Console.WriteLine("Invalid selection.");
                return null;
            }

            if (choice == 0)
            {
                return null;
            }

            return mapping[choice - 1];
        }

        // New: party-based round-robin combat
        public static void RunPartyCombat(List<BaseCharacter> partyMembers, List<BaseCharacter> enemies)
        {
            if (partyMembers == null || partyMembers.Count == 0) throw new ArgumentException("Party must have at least one member.");
            if (enemies == null || enemies.Count == 0) throw new ArgumentException("There must be at least one enemy.");

            var rnd = new Random();

            // Main loop
            while (partyMembers.Any(p => p.IsAlive) && enemies.Any(e => e.IsAlive))
            {
                Console.WriteLine("\n=== New Round ===");
                // Show brief status
                Console.WriteLine("Party:");
                foreach (var m in partyMembers)
                {
                    var res = m.PrimaryResourceType != ResourceType.None
                        ? $"| {m.PrimaryResourceType}: {m.CurrentResource}/{m.MaxResource} "
                        : string.Empty;

                    Console.WriteLine($" - {m.Name}: HP {m.Health}/{m.MaxHealth} {res}Defending: {m.IsDefending}");
                }

                Console.WriteLine("Enemies:");
                foreach (var en in enemies)
                {
                    var res = en.PrimaryResourceType != ResourceType.None
                        ? $"| {en.PrimaryResourceType}: {en.CurrentResource}/{en.MaxResource} "
                        : string.Empty;

                    Console.WriteLine($" - {en.Name}: HP {en.Health}/{en.MaxHealth} {res}Defending: {en.IsDefending}");
                }

                Console.WriteLine();

                // Party members act in order
                foreach (var member in partyMembers.ToList()) // ToList to allow modifications
                {
                    if (!member.IsAlive)
                    {
                        continue;
                    }

                    if (!enemies.Any(e => e.IsAlive))
                    {
                        break;
                    }

                    Console.WriteLine($"\n--- {member.Name}'s turn ---");
                    PlayerMemberTurn(member, partyMembers, enemies);

                    // remove dead enemies
                    enemies = enemies.Where(e => e.IsAlive).ToList();
                }

                if (!enemies.Any(e => e.IsAlive))
                {
                    Console.WriteLine("\n🎉 Party wins!");
                    break;
                }

                // Enemies act
                foreach (var enemy in enemies.ToList())
                {
                    if (!enemy.IsAlive)
                    {
                        continue;
                    }

                    Console.WriteLine($"\n--- {enemy.Name}'s turn ---");

                    // enemy simple AI: defend sometimes, else use ability if available, else attack random alive party member
                    if (rnd.NextDouble() < 0.15)
                    {
                        enemy.EnterDefend();
                        Console.WriteLine($"{enemy.Name} braces for incoming attacks.");
                        continue;
                    }

                    if (enemy is Warrior ew)
                    {
                        var usable = ew.Abilities.Where(a => a.ResourceCost <= ew.CurrentResource).OrderByDescending(a => a.ResourceCost).ToList();
                        if (usable.Any() && rnd.NextDouble() < 0.7)
                        {
                            var choice = usable[rnd.Next(usable.Count)];
                            // prefer AoE if available
                            if (choice is Whirlwind ww)
                            {
                                ww.Cast(ew, partyMembers.Where(p => p.IsAlive).ToList());
                            }
                            else
                            {
                                var target = partyMembers.Where(p => p.IsAlive).OrderBy(_ => rnd.Next()).FirstOrDefault();
                                if (target != null)
                                {
                                    choice.Cast(ew, target);
                                    if (target.IsDefending) target.ExitDefend();
                                }
                            }
                            continue;
                        }
                    }
                    else if (enemy is Mage em)
                    {
                        var usable = em.Spellbook.Where(s => s.ResourceCost <= em.CurrentResource).OrderByDescending(s => s.ResourceCost).ToList();
                        if (usable.Any() && rnd.NextDouble() < 0.7)
                        {
                            var choice = usable[rnd.Next(usable.Count)];
                            var target = partyMembers.Where(p => p.IsAlive).OrderBy(_ => rnd.Next()).FirstOrDefault();
                            if (target != null)
                            {
                                choice.Cast(em, target);
                                if (target.IsDefending) target.ExitDefend();
                            }
                            continue;
                        }
                    }

                    // fallback: normal attack a random alive party member
                    var victim = partyMembers.Where(p => p.IsAlive).OrderBy(_ => rnd.Next()).FirstOrDefault();
                    if (victim != null)
                    {
                        enemy.NormalAttack(victim);
                        if (victim.IsDefending) victim.ExitDefend();
                    }
                }

                // ---- TICK BUFFS ONCE PER ROUND (after all party and enemies acted) ----
                foreach (var m in partyMembers)
                {
                    m.TickTurn();
                }

                foreach (var en in enemies)
                {
                    en.TickTurn();
                }

                if (!partyMembers.Any(p => p.IsAlive))
                {
                    Console.WriteLine("\n💀 All party members defeated.");
                    break;
                }
            }
        }

        private static void PlayerMemberTurn(BaseCharacter member, List<BaseCharacter> party, List<BaseCharacter> enemies)
        {
            while (true)
            {
                Console.WriteLine("Choose action: \n1) Attack \n2) Abilities/Spells \n3) Defend \n4) Items \n5) Status \n6) Skip");
                Console.Write("Action: ");
                var input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                switch (input)
                {
                    case "1": // Attack
                        {
                            // choose enemy target
                            var target = PromptChooseFromGroupedLists(party, enemies);
                            if (target == null)
                            {
                                break;
                            }

                            member.NormalAttack(target);

                            if (target.IsDefending)
                            {
                                target.ExitDefend();
                            }

                            return;
                        }

                    case "2": // Abilities/Spells
                        {
                            switch (member)
                            {
                                case Mage mage:
                                    {
                                        if (!mage.Spellbook.Any())
                                        {
                                            Console.WriteLine("No spells.");
                                            break;
                                        }

                                        for (int i = 0; i < mage.Spellbook.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}) {mage.Spellbook[i].Name} - Cost {mage.Spellbook[i].ResourceCost}");
                                        }

                                        Console.Write("Select spell (0 cancel): ");
                                        if (!int.TryParse(Console.ReadLine(), out int sIdx) || sIdx < 0 || sIdx > mage.Spellbook.Count)
                                        {
                                            Console.WriteLine("Invalid");
                                            break;
                                        }

                                        if (sIdx == 0)
                                        {
                                            break;
                                        }

                                        var spell = mage.Spellbook[sIdx - 1];

                                        // special-case AoE spells: Whirlwind is warrior ability, Blink is self-buff (single).
                                        if (spell is Blink blink)
                                        {
                                            blink.Cast(mage, mage);
                                        }
                                        else
                                        {
                                            var spellTarget = PromptSelectTarget(member, party, enemies, ItemTarget.Any);
                                            if (spellTarget == null) break;
                                            spell.Cast(mage, spellTarget);
                                            if (spellTarget.IsDefending) spellTarget.ExitDefend();
                                        }

                                        return;
                                    }

                                case Warrior war:
                                    {
                                        if (!war.Abilities.Any())
                                        {
                                            Console.WriteLine("No abilities.");
                                            break;
                                        }

                                        for (int i = 0; i < war.Abilities.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}) {war.Abilities[i].Name} - Cost {war.Abilities[i].ResourceCost}");
                                        }

                                        Console.Write("Select ability (0 cancel): ");
                                        if (!int.TryParse(Console.ReadLine(), out int aIdx) || aIdx < 0 || aIdx > war.Abilities.Count)
                                        {
                                            Console.WriteLine("Invalid");
                                            break;
                                        }

                                        if (aIdx == 0)
                                        {
                                            break;
                                        }

                                        var ability = war.Abilities[aIdx - 1];

                                        if (ability is Whirlwind ww)
                                        {
                                            ww.Cast(war, enemies.Where(e => e.IsAlive).ToList());
                                        }
                                        else
                                        {
                                            var abilityTarget = PromptSelectTarget(member, party, enemies, ItemTarget.Any);
                                            if (abilityTarget == null) break;
                                            ability.Cast(war, abilityTarget);
                                            if (abilityTarget.IsDefending) abilityTarget.ExitDefend();
                                        }

                                        return;
                                    }

                                default:
                                    {
                                        Console.WriteLine("No abilities available.");
                                        break;
                                    }
                            }

                            break;
                        }

                    case "3": // Defend
                        {
                            member.EnterDefend();
                            Console.WriteLine($"{member.Name} is defending.");
                            return;
                        }

                    case "4": // Items
                        {
                            if (!member.Inventory.Any())
                            {
                                Console.WriteLine("No items.");
                                break;
                            }

                            member.ShowInventory();
                            Console.Write("Enter item number (0 cancel): ");
                            if (!int.TryParse(Console.ReadLine(), out int itemIdx) || itemIdx < 0 || itemIdx > member.Inventory.Count)
                            {
                                Console.WriteLine("Invalid");
                                break;
                            }

                            if (itemIdx == 0)
                            {
                                break;
                            }

                            var item = member.Inventory[itemIdx - 1];

                            // Use unified PromptSelectTarget with allowedTarget from item (works for Friend/Enemy/Self/Any)
                            var target = PromptSelectTarget(member, party, enemies, item.AllowedTarget);
                            if (target == null)
                            {
                                break;
                            }

                            var used = member.UseItem(item.Name, target);
                            if (used)
                            {
                                Console.WriteLine($"{member.Name} used {item.Name} on {target.Name}.");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to use {item.Name}.");
                            }

                            return;
                        }

                    case "5": // Status
                        {
                            foreach (var m in party)
                            {
                                m.ShowStats();
                            }

                            foreach (var e in enemies)
                            {
                                e.ShowStats();
                            }

                            break;
                        }

                    case "6":
                        {
                            Console.WriteLine($"{member.Name} skips.");
                            return;
                        }

                    default:
                        {
                            Console.WriteLine("Invalid choice.");
                            break;
                        }
                }
            }
        }

        // Shows friendly targets first, then enemies, with continuous numbering.
        // allies may contain the user; lists only living characters.
        public static BaseCharacter? PromptSelectTarget(
            BaseCharacter user,
            IReadOnlyCollection<BaseCharacter> allies,
            IReadOnlyCollection<BaseCharacter> enemies,
            ItemTarget allowedTarget = ItemTarget.Any)
        {
            if (user == null)
            {
                return null;
            }

            // Build lists of living targets
            var friends = (allies ?? Array.Empty<BaseCharacter>()).Where(a => a != null && a.IsAlive).Distinct().ToList();
            if (!friends.Contains(user))
            {
                friends.Insert(0, user);
            }

            var foes = (enemies ?? Array.Empty<BaseCharacter>()).Where(e => e != null && e.IsAlive).Distinct().ToList();

            // Respect allowedTarget
            if (allowedTarget == ItemTarget.Self)
            {
                if (user.IsAlive)
                {
                    return user;
                }

                return null;
            }

            if (allowedTarget == ItemTarget.Enemy)
            {
                return PromptChooseFromGroupedLists(new List<BaseCharacter>(), foes);
            }

            if (allowedTarget == ItemTarget.Friend)
            {
                return PromptChooseFromGroupedLists(friends, new List<BaseCharacter>());
            }

            // Any
            return PromptChooseFromGroupedLists(friends, foes);
        }
    }
}