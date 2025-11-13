using System;

namespace DungeonEscape
{
    internal static class Program
    {
        public static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== DUNGEON ESCAPE - Demo Auswahl ===");
                Console.WriteLine("1) Combat Demo");
                Console.WriteLine("2) Spell System Demo");
                Console.WriteLine("3) Interactive Combat Demo");
                Console.WriteLine("Q) Beenden");
                Console.Write("\nAuswahl: ");

                var input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
                    return;

                Console.WriteLine();
                switch (input)
                {
                    case "1":
                        Game.RunCombatDemo();
                        break;
                    case "2":
                        SpellSystemDemo.RunSpellDemo();
                        break;
                    case "3":
                        InteractiveDemo.RunCombatDemo();
                        break;
                    default:
                        Console.WriteLine("Ungültige Auswahl.");
                        break;
                }

                Console.WriteLine("\nDrücke Enter, um zurück zum Menü zu gelangen...");
                Console.ReadLine();
            }
        }
    }
}