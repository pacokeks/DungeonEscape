using System;
using System.Collections.Generic;
using System.Linq;
using DungeonEscape.Models;
using DungeonEscape.Models.Player;
using DungeonEscape.Models.Spells;

namespace DungeonEscape
{
    public class InteractiveDemo
    {
        public static void RunCombatDemo()
        {
            Console.WriteLine("╔═══════════════════════════════════════╗");
            Console.WriteLine("║  DUNGEON ESCAPE - Interactive Combat  ║");
            Console.WriteLine("╚═══════════════════════════════════════╝\n");

            // Create player characters
            BaseCharacter player = new Mage("Merlin", 120, 8, 30, 150, 40); // change to Warrior(...) to test warrior player
            BaseCharacter enemy = new Warrior("Dungeon Boss", 500, 20, 8, 100, 30);

            var rnd = new Random();

            while (player.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine("\n=== New Turn ===");
                Console.WriteLine($"Player: {player.Name} — Health: {player.Health}/{player.MaxHealth}  Mana: {player.CurrentResource}/{player.MaxResource}");
                Console.WriteLine($"Enemy : {enemy.Name}  — Health: {enemy.Health}/{enemy.MaxHealth}  Resource: {enemy.CurrentResource}/{enemy.MaxResource}");
                Console.WriteLine();

                // PLAYER TURN
                bool endPlayerTurn = false;
                while (!endPlayerTurn)
                {
                    Console.WriteLine("Choose action:");
                    Console.WriteLine("1) Attack");
                    Console.WriteLine("2) Abilities / Spells");
                    Console.WriteLine("3) Defend (doubles defense for next incoming hit)");
                    Console.WriteLine("4) Status");
                    Console.WriteLine("5) Skip");
                    Console.WriteLine("Q) Quit demo");
                    Console.Write("\nSelection: ");
                    var input = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(input)) continue;
                    if (input.Equals("q", StringComparison.OrdinalIgnoreCase)) return;

                    switch (input)
                    {
                        case "1": // Attack (normal physical)
                            {
                                player.NormalAttack(enemy);

                                // defend affects damage calculation only; restore defend state on target after hit
                                if (enemy.IsDefending)
                                {
                                    enemy.ExitDefend();
                                }

                                endPlayerTurn = true;
                                break;
                            }
                        case "2": // Abilities / Spells
                            {
                                if (player is Mage mage)
                                {
                                    if (mage.Spellbook.Count == 0)
                                    {
                                        Console.WriteLine("No spells known.");
                                        break;
                                    }

                                    Console.WriteLine("\nSpellbook:");
                                    for (int i = 0; i < mage.Spellbook.Count; i++)
                                    {
                                        var s = mage.Spellbook[i];
                                        Console.WriteLine($"{i + 1}) {s.Name} — Cost: {s.ResourceCost} {s.RequiredResourceType}");
                                    }
                                    Console.Write("Choose spell number (or 0 to cancel): ");
                                    if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 0 || idx > mage.Spellbook.Count)
                                    {
                                        Console.WriteLine("Invalid choice.");
                                        break;
                                    }
                                    if (idx == 0) break;

                                    var spell = mage.Spellbook[idx - 1];
                                    bool casted = spell.Cast(mage, enemy);

                                    if (enemy.IsDefending && casted)
                                    {
                                        enemy.ExitDefend();
                                    }

                                    endPlayerTurn = true;
                                }
                                else if (player is Warrior war)
                                {
                                    if (war.Abilities.Count == 0)
                                    {
                                        Console.WriteLine("No abilities known.");
                                        break;
                                    }

                                    Console.WriteLine("\nAbilities:");
                                    for (int i = 0; i < war.Abilities.Count; i++)
                                    {
                                        var a = war.Abilities[i];
                                        Console.WriteLine($"{i + 1}) {a.Name} — Cost: {a.ResourceCost} {a.RequiredResourceType}");
                                    }
                                    Console.Write("Choose ability number (or 0 to cancel): ");
                                    if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 0 || idx > war.Abilities.Count)
                                    {
                                        Console.WriteLine("Invalid choice.");
                                        break;
                                    }
                                    if (idx == 0) break;

                                    var ability = war.Abilities[idx - 1];
                                    bool used = ability.Cast(war, enemy);

                                    if (enemy.IsDefending && used)
                                    {
                                        enemy.ExitDefend();
                                    }

                                    endPlayerTurn = true;
                                }
                                else
                                {
                                    Console.WriteLine("No special actions available.");
                                    break;
                                }
                                break;
                            }
                        case "3": // Defend
                            player.EnterDefend();
                            Console.WriteLine($"{player.Name} takes a defensive stance (next incoming hit will use doubled defenses).");
                            endPlayerTurn = true;
                            break;
                        case "4": // Status
                            player.ShowStats();
                            enemy.ShowStats();
                            break;
                        case "5": // Skip
                            Console.WriteLine($"{player.Name} skips the turn.");
                            endPlayerTurn = true;
                            break;
                        default:
                            Console.WriteLine("Invalid selection.");
                            break;
                    } // switch
                } // player action loop

                if (!enemy.IsAlive)
                {
                    Console.WriteLine($"\n🎉 {player.Name} defeated {enemy.Name}!");
                    break;
                }

                // ENEMY TURN (simple AI)
                Console.WriteLine("\n--- Enemy's turn ---");

                // Decide action: prefer ability if resource sufficient, else attack, sometimes defend
                bool enemyActed = false;

                // 20% chance to defend
                if (rnd.NextDouble() < 0.20)
                {
                    enemy.EnterDefend();
                    Console.WriteLine($"{enemy.Name} braces for incoming attacks (defending).");
                    enemyActed = true;
                }

                if (!enemyActed)
                {
                    // If enemy has abilities and resource, try to use best-cost ability
                    if (enemy is Warrior enemyWar)
                    {
                        // pick highest-cost ability that can be paid
                        var usable = enemyWar.Abilities
                            .Where(a => a.RequiredResourceType == enemyWar.PrimaryResourceType && a.ResourceCost <= enemyWar.CurrentResource)
                            .OrderByDescending(a => a.ResourceCost)
                            .ToList();

                        if (usable.Count > 0 && rnd.NextDouble() < 0.7) // 70% chance to use ability when possible
                        {
                            var choice = usable[rnd.Next(usable.Count)];
                            int beforeHp = player.Health;
                            bool used = choice.Cast(enemyWar, player);
                            if (player.IsDefending && used)
                            {
                                player.ExitDefend();
                            }
                            enemyActed = true;
                        }
                    }
                    else if (enemy is Mage enemyMage)
                    {
                        var usable = enemyMage.Spellbook
                            .Where(s => s.RequiredResourceType == enemyMage.PrimaryResourceType && s.ResourceCost <= enemyMage.CurrentResource)
                            .OrderByDescending(s => s.ResourceCost)
                            .ToList();

                        if (usable.Count > 0 && rnd.NextDouble() < 0.7)
                        {
                            var choice = usable[rnd.Next(usable.Count)];
                            int beforeHp = player.Health;
                            bool used = choice.Cast(enemyMage, player);
                            if (player.IsDefending && used)
                            {
                                player.ExitDefend();
                            }
                            enemyActed = true;
                        }
                    }
                }

                if (!enemyActed)
                {
                    // Fallback: normal attack
                    enemy.NormalAttack(player);
                    if (player.IsDefending)
                    {
                        player.ExitDefend();
                    }
                }

                // End of round checks
                if (!player.IsAlive)
                {
                    Console.WriteLine($"\n💀 {player.Name} was defeated by {enemy.Name}!");
                    break;
                }
            } // main while

            Console.WriteLine("\n=== Combat ended ===");
            Console.WriteLine("Returning to demo menu...");
        }
    }
}