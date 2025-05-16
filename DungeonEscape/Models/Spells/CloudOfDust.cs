using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonEscape.Models.Spells
{
    public class CloudOfDust : BaseSpell
    {
        public CloudOfDust() : base(name: "Cloud of Dust", resourceType: ResourceType.Mana, SpellType.Magic, resourceCost: 100, cooldownTurns: 10)
        {
            // Initialize the spell with its properties
            // Name = "Cloud of Dust";
            // ResourceType = ResourceType.None;
            // ResourceCost = 0;
            // CooldownTurns = 0;
        }

        public override void Cast(BaseCharacter caster, BaseCharacter target)
        {
            throw new NotImplementedException();
        }
    }
}
