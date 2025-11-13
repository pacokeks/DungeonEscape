using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DungeonEscape.Models;
using DungeonEscape.Models.Spells;
using ResourceType = DungeonEscape.Models.Spells.ResourceType;

public abstract class BaseSpell
{
    public string Name { get; }
    public int ResourceCost { get; }
    public ResourceType RequiredResourceType { get; }

    // Public method - handles ALL validation
    public bool Cast(BaseCharacter caster, BaseCharacter target)
    {
        // 1. Validate caster & resources
        // 2. Consume resources
        // 3. Call ExecuteSpellEffect()
    }

    // Abstract - derived classes implement this
    protected abstract void ExecuteSpellEffect(
        BaseCharacter caster,
        BaseCharacter target);
}