using System;
using System.Collections.Generic;
using DungeonEscape.Combat;
using DungeonEscape.Models;
using DungeonEscape.Models.Items;
using DungeonEscape.Models.Player;

namespace DungeonEscape
{
    public class InteractiveDemo
    {
        public static void RunCombatDemo()
        {
            Console.WriteLine("╔═══════════════════════════════════════╗");
            Console.WriteLine("║  DUNGEON ESCAPE - Interactive Combat  ║");
            Console.WriteLine("╚═══════════════════════════════════════╝\n");

            // Build party
            var party = new Party("Heroes");
            var p1 = new Mage("Merlin", 120, 8, 30, 150, 40);
            var p2 = new Warrior("Lancelot", 200, 18, 5, 100, 30);
            party.Add(p1);
            party.Add(p2);

            // Enemies
            var boss = new Warrior("Dungeon Boss", 600, 20, 8, 100, 35);
            var enemies = new List<BaseCharacter> { boss };

            // Give starter items
            p1.AddItem(new HealingItem("Small Potion", 50, "Restores 50 HP"));
            p1.AddItem(new ResourceItem("Minor Mana Potion", 30, "Restores 30 mana"));
            p2.AddItem(new HealingItem("Small Potion", 50, "Restores 50 HP"));

            // Start party combat
            CombatManager.RunPartyCombat(party.Members, enemies);
        }
    }
}