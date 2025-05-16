using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DungeonEscape.Models.Spells;

namespace DungeonEscape.Models.Player
{
    public class Mage : BaseCharacter
    {
        /// <summary>
        /// Current Mana pints of the Mage. Used for casting Spells.
        /// </summary>
        public override int Mana { get; protected set; } = 100;
        /// <summary>
        /// Maximum Mana points of the Mage.
        /// </summary>
        public override int MaxMana { get; protected set; } = 100;
        /// <summary>
        /// The Mages intelligence attribute, affects spell damage and mana regeneration.
        /// </summary>
        public int Intelligence { get; private set; }
        /// <summary>
        /// The Mages spell power attribute, affects spell damage.
        /// To be implemented. (mage.Intellegence / 4)
        /// </summary>
        public int SpellPower { get; private set; }

        public List<BaseSpell> Spells { get; private set; }

        public Mage(string name, int health = 60, int defense = 5, int magicResistance = 15, int intelligence = 15) : base(name, health, defense, magicResistance)
        {
            // Initialize mage-specific properties
            MaxMana = 100;
            Mana = MaxMana;
            this.Intelligence = intelligence;
            SpellPower = Intelligence / 2;

            Spells = new List<BaseSpell>
            {
                new FireBall(),
                new CloudOfDust(),
                //new Frostbolt(),
            };
        }

        public void CastSpell(BaseSpell spell, BaseCharacter target)
        {
            // Check if the spell is in the mage's spellbook
            if (Spells.Contains(spell))
            {
                // Implement the logic for casting a spell
                spell.Cast(this, target);
            }
            else
            {
                Console.WriteLine($"{Name} doesn't know the spell {spell.Name}.");
            }
        }

        public override int CalculateAttackDamage()
        {
            // Mages could use Intelligence for spell damage
            int baseDamage = SpellPower;
            return baseDamage;
        }

        public override void ShowStats()
        {
            base.ShowStats();

            Console.WriteLine($"Intelligence: {Intelligence}");
            Console.WriteLine($"Spellpower: {SpellPower}");

            Console.WriteLine("\nSpells:");
            foreach (var spell in Spells)
            {
                Console.WriteLine($". {spell.Name} ({spell.ResourceCost} {spell.ResourceType}), Cooldown: {spell.CurrentCooldown}/{spell.CooldownTurns}");
            }
            Console.WriteLine("");
        }
    }
}
