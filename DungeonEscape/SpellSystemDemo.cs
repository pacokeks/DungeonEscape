using System;
using DungeonEscape.Models;
using DungeonEscape.Models.Player;
using DungeonEscape.Models.Spells;

namespace DungeonEscape
{
    public class SpellSystemDemo
    {
        public static void RunSpellDemo()
        {
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║  DUNGEON ESCAPE - Spell System Demo      ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝\n");

            // Create characters
            var mage = new Mage("Gandalf", 120, 8, 30, 150, 40);
            var warrior = new Warrior("Conan", 250, 25, 5, 100, 35);
            var enemy = new Warrior("Orc Warrior", 200, 15, 8, 100, 30);

            // ============================================
            // DEMO 1: Mage Spellbook
            // ============================================
            Console.WriteLine("═══ DEMO 1: Mage Spellbook ═══\n");

            mage.ShowSpellbook();

            Console.WriteLine("\n--- Casting different spells ---");
            mage.CastSpell("Fireball", enemy);      // High damage, high cost
            mage.CastSpell("Frostbolt", enemy);     // Medium damage, medium cost
            mage.CastSpell("Arcane Missiles", enemy); // Low damage, low cost, multiple hits

            Console.WriteLine();
            mage.ShowStats();

            // ============================================
            // DEMO 2: Warrior Abilities
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 2: Warrior Abilities ═══\n");

            warrior.ShowAbilities();

            Console.WriteLine("\n--- Building rage and using abilities ---");
            warrior.NormalAttack(enemy);  // Generates 15 rage
            warrior.NormalAttack(enemy);  // Generates 15 rage (30 total)

            Console.WriteLine();
            warrior.UseAbility("Whirlwind", enemy);  // Costs 30 rage

            Console.WriteLine("\n--- Building more rage ---");
            warrior.NormalAttack(enemy);  // 15 rage
            warrior.NormalAttack(enemy);  // 30 rage
            warrior.NormalAttack(enemy);  // 45 rage

            Console.WriteLine();
            warrior.UseAbility("Execute", enemy);    // Costs 40 rage

            // ============================================
            // DEMO 3: Resource Management
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 3: Resource Management ═══\n");

            var testMage = new Mage("Merlin", 100, 5, 20, 100, 50);
            var dummy = new Warrior("Training Dummy", 500, 10, 10, 0, 0);

            Console.WriteLine("--- Mage tries to spam Fireball ---");
            testMage.CastSpell("Fireball", dummy);   // 50 mana (50 left)
            testMage.CastSpell("Fireball", dummy);   // 50 mana (0 left)
            testMage.CastSpell("Fireball", dummy);   // Not enough mana!

            Console.WriteLine("\n--- Mage uses cheaper spells instead ---");
            testMage.RegenerateMana(30);
            testMage.CastSpell("Arcane Missiles", dummy);  // 20 mana

            Console.WriteLine("\n--- Mage can still normal attack when out of mana ---");
            testMage.NormalAttack(dummy);

            // ============================================
            // DEMO 4: Learning New Spells
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 4: Learning New Spells ═══\n");

            Console.WriteLine("Current spellbook:");
            testMage.ShowSpellbook();

            Console.WriteLine("\n--- Learning a new spell ---");
            var newFireball = new Fireball(baseDamage: 80, spellPowerScaling: 1.2);  // Upgraded version!
            testMage.LearnSpell(newFireball);

            Console.WriteLine("\n--- Trying to learn wrong resource type spell ---");
            var execute = new Execute();  // Requires Rage
            testMage.LearnSpell(execute);  // This will fail!

            // ============================================
            // DEMO 5: Spell Details
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 5: Spell Information ═══\n");

            var fireball = new Fireball();
            var frostbolt = new Frostbolt();
            var executeAbility = new Execute();

            fireball.ShowSpellInfo();
            Console.WriteLine();
            frostbolt.ShowSpellInfo();
            Console.WriteLine();
            executeAbility.ShowSpellInfo();

            // ============================================
            // DEMO 6: Combat Scenario
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 6: Full Combat Scenario ═══\n");

            var hero = new Mage("Archmage", 150, 10, 25, 200, 45);
            var boss = new Warrior("Dark Knight", 300, 30, 15, 100, 40);

            Console.WriteLine($"*** {hero.Name} vs {boss.Name} ***\n");

            int round = 1;
            while (hero.IsAlive && boss.IsAlive && round <= 10)
            {
                Console.WriteLine($"--- Round {round} ---");

                // Hero's turn - use best available spell
                if (hero.CurrentResource >= 50)
                {
                    hero.CastSpell("Fireball", boss);
                }
                else if (hero.CurrentResource >= 30)
                {
                    hero.CastSpell("Frostbolt", boss);
                }
                else if (hero.CurrentResource >= 20)
                {
                    hero.CastSpell("Arcane Missiles", boss);
                }
                else
                {
                    hero.NormalAttack(boss);
                    hero.RegenerateMana(15);  // Regenerate some mana
                }

                if (!boss.IsAlive)
                {
                    Console.WriteLine($"\n🎉 {hero.Name} wins!");
                    break;
                }

                // Boss's turn
                if (boss.CurrentResource >= 40)
                {
                    boss.UseAbility("Execute", hero);
                }
                else if (boss.CurrentResource >= 25)
                {
                    boss.UseAbility("Heroic Strike", hero);
                }
                else
                {
                    boss.NormalAttack(hero);
                }

                if (!hero.IsAlive)
                {
                    Console.WriteLine($"\n💀 {hero.Name} was defeated!");
                    break;
                }

                Console.WriteLine();
                round++;
            }

            Console.WriteLine("\n=== Final Stats ===");
            hero.ShowStats();
            Console.WriteLine();
            boss.ShowStats();

            // ============================================
            // DEMO 7: Spell Comparison
            // ============================================
            Console.WriteLine("\n\n═══ DEMO 7: Damage Comparison ═══\n");

            var testDummy = new Warrior("Test Dummy", 1000, 10, 10, 0, 0);
            var testMageComparison = new Mage("Test Mage", 100, 5, 20, 300, 50);

            Console.WriteLine("Testing all mage spells on dummy:\n");

            Console.WriteLine($"Dummy Health: {testDummy.Health}/{testDummy.MaxHealth}");
            testMageComparison.CastSpell("Fireball", testDummy);

            Console.WriteLine($"\nDummy Health: {testDummy.Health}/{testDummy.MaxHealth}");
            testMageComparison.CastSpell("Frostbolt", testDummy);

            Console.WriteLine($"\nDummy Health: {testDummy.Health}/{testDummy.MaxHealth}");
            testMageComparison.CastSpell("Arcane Missiles", testDummy);

            Console.WriteLine($"\nDummy Final Health: {testDummy.Health}/{testDummy.MaxHealth}");

            Console.WriteLine("\n\n═══ DEMO COMPLETE ═══");
        }
    }
}