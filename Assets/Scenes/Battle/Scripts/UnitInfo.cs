
using Scenes.Battle.Scripts.Enums;

namespace Scenes.Battle.Scripts
{
    public class UnitInfo
    {
        // 진영(아군, 적)
        public Faction faction { get; private set; }
        public float range { get; private set; }

        public UnitInfo(
            Faction faction,
            float range
        )
        {
            this.faction = faction;
            this.range = range;
        }
    }   
}