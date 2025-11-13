using System;
using System.Linq;
using DungeonEscape.Models;
using DungeonEscape.Models.Player;
using DungeonEscape.Models.Spells;

namespace DungeonEscape.Combat
{
    /// <summary>
    /// Reusable interactive combat manager.
    /// Call CombatManager.RunInteractiveCombat(player, enemy)
    /// </summary>
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
            }
        }

        private static void ShowBriefStatus(BaseCharacter player, BaseCharacter enemy)
        {
            Console.WriteLine($"{player.Name}: HP {player.Health}/{player.MaxHealth}  Resource: {player.CurrentResource}/{player.MaxResource}  Defending: {player.IsDefending}");
            Console.WriteLine($"{enemy.Name} : HP {enemy.Health}/{enemy.MaxHealth}  Resource: {enemy.CurrentResource}/{enemy.MaxResource}  Defending: {enemy.IsDefending}");
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
                        player.NormalAttack(enemy);

                        // defend affects damage calculation only; restore defend state on target after hit
                        if (enemy.IsDefending)
                        {
                            enemy.ExitDefend();
                        }

                        return true;

                    case "2":
                        UsePlayerAbility(player, enemy);

                        // if enemy was defending, restore after spell/ability
                        if (enemy.IsDefending)
                        {
                            enemy.ExitDefend();
                        }

                        return true;

                    case "3":
                        // use current BaseCharacter defend API
                        player.EnterDefend();
                        return true;

                    case "4":
                        player.ShowStats();
                        enemy.ShowStats();
                        break;

                    case "5":
                        Console.WriteLine($"{player.Name} skips.");
                        return true;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void UsePlayerAbility(BaseCharacter player, BaseCharacter enemy)
        {
            // Use switch type-pattern to avoid ambiguous if-chains
            switch (player)
            {
                case Mage mage:
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

                    if (sIdx == 0) return;

                    var spell = mage.Spellbook[sIdx - 1];
                    spell.Cast(mage, enemy);
                    return;

                case Warrior war:
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

                    if (aIdx == 0) return;

                    var ability = war.Abilities[aIdx - 1];
                    ability.Cast(war, enemy);
                    return;

                default:
                    Console.WriteLine("No special actions available for this character.");
                    return;
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
                    choice.Cast(ew, player);

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
    }
}