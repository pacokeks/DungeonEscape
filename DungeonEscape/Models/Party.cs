using System.Collections.Generic;

namespace DungeonEscape.Models
{
    /// <summary>
    /// Simple party container for allied characters.
    /// </summary>
    public class Party
    {
        public string Name { get; init; }
        public List<BaseCharacter> Members { get; } = new List<BaseCharacter>();

        public Party(string name)
        {
            Name = name;
        }

        public void Add(BaseCharacter member)
        {
            if (member == null)
            {
                return;
            }

            if (!Members.Contains(member))
            {
                Members.Add(member);
            }
        }
    }
}