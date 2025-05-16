using System;
using DungeonEscape.Models;
using DungeonEscape.Models.Player;
using DungeonEscape.Models.Spells;

namespace DungeonEscape
{
    public class TestRunner
    {
        public static void ShowMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("===== DungeonEscape Test Menu =====");
                Console.WriteLine("1. Run All Tests");
                Console.WriteLine("2. Test Character Creation");
                Console.WriteLine("3. Test Mage Spells");
                Console.WriteLine("4. Test Warrior Abilities");
                Console.WriteLine("5. Test Rogue Abilities");
                Console.WriteLine("6. Test Combat");
                Console.WriteLine("7. Exit");
                Console.WriteLine("Select an option (use the number): ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        RunAllTests();
                        break;
                    case "2":
                        TestCharacterCreation();
                        break;
                    case "3":
                        TestMageSpells();
                        break;
                    case "4":
                        TestWarriorAbilities();
                        break;
                    case "5":
                        TestRogueAbilities();
                        break;
                    case "6":
                        TestCombat();
                        break;
                    case "7":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\n Press any key to return to the menu");
                    Console.ReadKey();
                }

            }
        }

        public static void RunAllTests()
        {
            Console.Clear();
            Console.WriteLine("=== Running All Tests ===\n");

            TestCharacterCreation();
            TestMageSpells();
            TestWarriorAbilities();
            TestRogueAbilities();
            TestCombat();

            Console.WriteLine("\n === All Tests Completed ===");
        }
        public static void TestCharacterCreation()
        {
            Console.WriteLine("=== Test: Characher Creation ===\n");

            Mage Pascal = new Mage("Pascal", health: 80, defense: 5, magicResistance: 15, intelligence: 20);
            Warrior Mete = new Warrior("Mete", health: 100, defense: 15, magicResistance: 5, strength: 15);
            Pascal.ShowStats();
            Mete.ShowStats();

            Console.WriteLine("\n Character Creation Test Completed");
        }

        public static void TestMageSpells()
        {
            Console.Clear();
            Console.WriteLine("=== Test: Mage Spells ===\n");

            Mage Pascal = new Mage("Pascal", health: 80, defense: 5, magicResistance: 15, intelligence: 20);
            TestEnemy Roy = new TestEnemy("Roy", health: 100, defense: 5, magicResistance: 5);

            Console.WriteLine($"Created Mage: {Pascal.Name} (Health: {Pascal.Health}/{Pascal.MaxHealth}, Mana: {Pascal.Mana}/{Pascal.MaxMana})");
            Console.WriteLine($"Created TestEnemy: {Roy.Name} (Health: {Roy.Health}/{Roy.MaxHealth})");

            Console.WriteLine($"\nAvailable Spells for {Pascal.Name}:");
            foreach (var spell in Pascal.Spells)
            {
                Console.WriteLine($"- {spell.GetDescription()}");
            }

            FireBall? fireball = Pascal.Spells.Find(spell => spell is FireBall) as FireBall;
            if (fireball != null)
            {
                Console.WriteLine($"\nTesting 1st cast of {fireball.Name}: ");

                Console.WriteLine($"\nInitial Mana of {Pascal.Name}: {Pascal.Mana}/{Pascal.MaxMana}");
                fireball.Cast(Pascal, Roy);
                Console.WriteLine($"Pascal Mana: {Pascal.Mana}/{Pascal.MaxMana}\n");

                Console.WriteLine($"Casting Fireball for the 2nd time: (should be on cooldown)");
                fireball.Cast(Pascal, Roy);
                Console.WriteLine($"Pascal Mana: {Pascal.Mana}/{Pascal.MaxMana}\n");

                Console.WriteLine($"Casting Fireball for the 3rd time: (should be on cooldown)");
                fireball.Cast(Pascal, Roy);
                Console.WriteLine($"Pascal Mana: {Pascal.Mana}/{Pascal.MaxMana}\n");

                Console.WriteLine("\nResetting Cooldown.");
                fireball.ResetCooldown();
                Console.WriteLine($"Casting Fireball for the 4th time:");
                fireball.Cast(Pascal, Roy);
                Console.WriteLine($"Pascal Mana: {Pascal.Mana}/{Pascal.MaxMana}\n");
            }

            //CloudOfDust? cloudOfDust = Pascal.Spells.Find(spell => spell is CloudOfDust) as CloudOfDust;
            //if (cloudOfDust != null)
            //{
            //    Console.WriteLine($"\n Testing first cast of {cloudOfDust.Name}: ");

            //    cloudOfDust.Cast(Pascal, Roy);
            //    Console.WriteLine($"Pascal Mana: {Pascal.Mana}/{Pascal.MaxMana}\n");

            //    Console.WriteLine($"Casting CloudOfDust for the 2nd time:");
            //    cloudOfDust.Cast(Pascal, Roy);
            //    Console.WriteLine($"Pascal Mana: {Pascal.Mana}/{Pascal.MaxMana}\n");

            //    Console.WriteLine($"Casting CloudOfDust for the 3rd time:");
            //    cloudOfDust.Cast(Pascal, Roy);
            //    Console.WriteLine($"Pascal Mana: {Pascal.Mana}/{Pascal.MaxMana}\n");

            //    Console.WriteLine("\nResetting Cooldown.");
            //    cloudOfDust.ResetCooldown();
            //    Console.WriteLine($"Casting CloudOfDust for the 4th time:");
            //    cloudOfDust.Cast(Pascal, Roy);
            //    Console.WriteLine($"Pascal Mana: {Pascal.Mana}/{Pascal.MaxMana}\n");
            //}
        }

        public static void TestWarriorAbilities()
        {
            Console.Clear();
            Console.WriteLine("=== Test: Warrior Spells ===\n");

            Warrior Mete = new Warrior(name: "Mete", health: 120, defense: 15, magicResistance: 5, strength: 15);
            TestEnemy Roy = new TestEnemy(name: "Roy", health: 100, defense: 5, magicResistance: 5);

            Console.WriteLine($"Created Warrior: {Mete.Name} (Health: {Mete.Health}/{Mete.MaxHealth}, Rage: {Mete.Rage}/{Mete.MaxRage})");
            Console.WriteLine($"Created TestEnemy: {Roy.Name} (Health: {Roy.Health}/{Roy.MaxHealth})");

            Console.WriteLine($"\nAvailable Abilities of {Mete.Name}:");
            foreach (var ability in Mete.Abilities)
            {
                Console.WriteLine($"- {ability.GetDescription()}");
            }

            Console.WriteLine($"\nInitial Rage of {Mete.Name}: {Mete.Rage}/{Mete.MaxRage}");

            Console.WriteLine("\nGenerating rage with basic attacks:");
            for (int i = 0; i < 3; i++)
            {
                int damage = Mete.CalculateAttackDamage();
                Roy.TakeDamage(damage);
                Console.WriteLine($"{Mete.Name}s Rage: {Mete.Rage}/{Mete.MaxRage}");
            }

            BerserkerStrike? berserkerStrike = Mete.Abilities.Find(abililty => abililty is BerserkerStrike) as BerserkerStrike;
            if (berserkerStrike != null)
            {
                Console.WriteLine($"Testing {berserkerStrike.Name} for the 1st time:");
                berserkerStrike.Cast(Mete, Roy);
                Console.WriteLine($"\nRage of {Mete.Name}: {Mete.Rage}/{Mete.MaxRage}");

                Console.WriteLine($"Testing {berserkerStrike.Name} for the 2nd time: (should be on cooldown)");
                berserkerStrike.Cast(Mete, Roy);
                Console.WriteLine($"\nRage of {Mete.Name}: {Mete.Rage}/{Mete.MaxRage}");

                Console.WriteLine($"Testing {berserkerStrike.Name} for the 3rd time:(should be on cooldown)");
                berserkerStrike.Cast(Mete, Roy);
                Console.WriteLine($"\nRage of {Mete.Name}: {Mete.Rage}/{Mete.MaxRage}");

                Console.WriteLine("\nGenerating more rage with basic attacks:");
                for (int i = 0; i < 3; i++)
                {
                    int damage = Mete.CalculateAttackDamage();
                    Roy.TakeDamage(damage);
                    Console.WriteLine($"{Mete.Name}s Rage: {Mete.Rage}/{Mete.MaxRage}");
                }

                Console.WriteLine($"\nResetting cooldown and casting {berserkerStrike.Name} for the 4th time: ");
                berserkerStrike.ResetCooldown();
                berserkerStrike.Cast(Mete, Roy);
                Console.WriteLine($"\nRage of {Mete.Name}: {Mete.Rage}/{Mete.MaxRage}");
            }
            Console.WriteLine("\nWarrior Test Completed.");
        }

        public static void TestRogueAbilities()
        {
            Console.WriteLine("Test Rogue");
        }

        public static void TestCombat()
        {
            Console.Clear();
            Console.WriteLine("=== Test:Combat Routine ===\n");

            Mage Pascal = new Mage("Pascal", health: 80, defense: 5, magicResistance: 15, intelligence: 20);
            Warrior Mete = new Warrior(name: "Mete", health: 120, defense: 15, magicResistance: 5, strength: 15);
            TestEnemy Roy = new TestEnemy(name: "Roy", health: 100, defense: 5, magicResistance: 5);

            Console.WriteLine("Combat participants:");
            Console.WriteLine($"Mage: {Pascal.Name} (Health: {Pascal.Health}/{Pascal.MaxHealth}, Mana: {Pascal.Mana}/{Pascal.MaxMana})");
            Console.WriteLine($"Warrior: {Mete.Name} (Health: {Mete.Health}/{Mete.MaxHealth}, Rage: {Mete.Rage}/{Mete.MaxRage})");
            Console.WriteLine($"TestEnemy: {Roy.Name} (Health: {Roy.Health}/{Roy.MaxHealth})\n");

            Console.WriteLine($"=== Round 1 ===");


        }

        public class TestEnemy : BaseCharacter
        {
            public TestEnemy(string name, int health, int defense, int magicResistance) : base(name, health, defense, magicResistance)
            {

            }

            public override int CalculateAttackDamage()
            {
                return new Random().Next(5, 15);
            }
        }
    }
}