using DungeonEscape.Combat;
using DungeonEscape.Models;
using DungeonEscape.Models.Items;
using DungeonEscape.Models.Player;
using DungeonEscape.Models.Spells;
using System;
using System.Collections.Generic;

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
            p1.LearnSpell(new Frostbolt());
            p1.LearnSpell(new ArcaneMissiles());
            p1.LearnSpell(new Blink());
            party.Add(p2);

            // Enemies
            var boss = new Warrior("Dungeon Boss", 600, 20, 8, 100, 35);
            var boss2 = new Mage("Dungeon Boss2", 600, 20, 8, 100, 35);
            boss2.LearnSpell(new Frostbolt());
            boss2.LearnSpell(new ArcaneMissiles());
            boss2.LearnSpell(new Blink());
            var enemies = new List<BaseCharacter> { boss, boss2 };

            // Give starter items
            p1.AddItem(new HealingItem("Small Potion", 50, "Restores 50 HP"));
            p1.AddItem(new ResourceItem("Minor Mana Potion", 30, "Restores 30 mana"));
            p2.AddItem(new HealingItem("Small Potion", 50, "Restores 50 HP"));

            // Start party combat
            CombatManager.RunPartyCombat(party.Members, enemies);
        }
    }
}