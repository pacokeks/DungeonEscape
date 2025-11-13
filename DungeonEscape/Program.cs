using System;
using DungeonEscape.Models;
using DungeonEscape.Models.Player;

namespace DungeonEscape
{
    public class Game
    {
        public static void Main()
        {
            Console.WriteLine("╔═══════════════════════════════════════╗");
            Console.WriteLine("║  DUNGEON ESCAPE - Combat System Demo ║");
            Console.WriteLine("╚═══════════════════════════════════════╝\n");

            // Create player characters
            var mage = new Mage("Merlin", 120, 8, 30, 150, 40);
            var warrior = new Warrior("Conan", 250, 25, 5, 100, 35);

            // Create an enemy
            var boss = new Warrior("Dungeon Boss", 300, 20, 8, 100, 30);

            // ============================================
            // DEMO 1: Warrior Rage System
            // ============================================
            Console.WriteLine("═══ DEMO 1: Warrior Rage Generation ═══\n");

            warrior.ShowStats();
            Console.WriteLine("\n--- Warrior attacks 3 times to build rage ---");

            // Normal attacks generate rage automatically via OnAfterNormalAttack hook
            warrior.NormalAttack(boss);
            warrior.NormalAttack(boss);
            warrior.NormalAttack(boss);

            Console.WriteLine("\n--- Now warrior has enough rage for Execute ---");
            warrior.Execute(boss);

            Console.WriteLine("\n--- Not enough rage anymore ---");
            warrior.Execute(boss); // This will fail

            // ============================================
            // DEMO 2: Warrior generates rage from taking damage
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 2: Rage from Taking Damage ═══\n");

            Console.WriteLine("--- Boss attacks warrior ---");
            boss.NormalAttack(warrior); // Warrior takes damage AND generates rage

            Console.WriteLine("\n--- Warrior can use Heroic Strike now ---");
            warrior.HeroicStrike(boss);

            // ============================================
            // DEMO 3: Mage Mana System
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 3: Mage Mana Management ═══\n");

            mage.ShowStats();
            Console.WriteLine("\n--- Mage casts spells until out of mana ---");

            mage.CastFireball(boss);  // 50 mana (100 left)
            mage.CastFireball(boss);  // 50 mana (50 left)
            mage.CastFireball(boss);  // 50 mana (0 left)
            mage.CastFireball(boss);  // Not enough mana!

            Console.WriteLine("\n--- Mage can still use normal attacks ---");
            mage.NormalAttack(boss);  // Weak but free!

            Console.WriteLine("\n--- Mage regenerates mana ---");
            mage.RegenerateMana(100);
            mage.CastFireball(boss);  // Can cast again!

            // ============================================
            // DEMO 4: Full Combat Simulation
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 4: Full Combat Simulation ═══\n");

            var hero = new Warrior("Hero", 200, 20, 5, 100, 30);
            var enemy = new Warrior("Evil Knight", 150, 15, 8, 100, 25);

            Console.WriteLine($"*** {hero.Name} vs {enemy.Name} ***\n");

            int round = 1;
            while (hero.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine($"--- Round {round} ---");

                // Hero's turn - use Execute if enough rage, otherwise normal attack
                if (hero.CurrentResource >= 40)
                {
                    hero.Execute(enemy);
                }
                else
                {
                    hero.NormalAttack(enemy);
                }

                if (!enemy.IsAlive)
                {
                    Console.WriteLine($"\n🎉 {hero.Name} wins!");
                    break;
                }

                // Enemy's turn
                if (enemy.CurrentResource >= 25)
                {
                    enemy.HeroicStrike(hero);
                }
                else
                {
                    enemy.NormalAttack(hero);
                }

                if (!hero.IsAlive)
                {
                    Console.WriteLine($"\n💀 {hero.Name} was defeated!");
                    break;
                }

                Console.WriteLine();
                round++;

                // Prevent infinite loop
                if (round > 20)
                {
                    Console.WriteLine("Combat timeout - it's a draw!");
                    break;
                }
            }

            Console.WriteLine("\n=== Final Stats ===");
            hero.ShowStats();
            Console.WriteLine();
            enemy.ShowStats();

            // ============================================
            // DEMO 5: Comparison of Attack Styles
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 5: Attack Power Comparison ═══\n");

            var testMage = new Mage("Test Mage", 100, 5, 20, 200, 50);
            var testWarrior = new Warrior("Test Warrior", 200, 15, 5, 100, 40);
            var dummy = new Warrior("Training Dummy", 1000, 5, 5, 0, 0);

            Console.WriteLine("Normal Attack Damage:");
            Console.WriteLine($"  {testMage.Name}: {testMage.CalculateAttackDamage()} damage");
            Console.WriteLine($"  {testWarrior.Name}: {testWarrior.CalculateAttackDamage()} damage");

            Console.WriteLine("\nSpecial Abilities:");
            Console.WriteLine("  Mage Fireball: 60 + SpellPower (50 mana cost)");
            Console.WriteLine("  Warrior Execute: 80 + (Strength * 2) (40 rage cost)");
            Console.WriteLine("  Warrior Heroic Strike: 50 + Strength (25 rage cost)");

            Console.WriteLine("\n=== DEMO COMPLETE ===");
        }
    }
}