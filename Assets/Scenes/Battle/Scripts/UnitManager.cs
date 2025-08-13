using System;
using System.Collections.Generic;
using Scenes.Battle.Scripts.Enums;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System.Linq;
using Common.Packages.SceneSingleton;
using JetBrains.Annotations;

namespace Scenes.Battle.Scripts
{
    public class TransformUnitPair
    {
        public Transform transform {get; }
        public Unit unit {get; }

        public TransformUnitPair(Transform transform, Unit unit)
        {
            this.transform = transform;
            this.unit = unit;
        }

        public float CirculateDistance2(Vector2 position)
        {
            float dx = transform.position.x - position.x;
            float dy = transform.position.y - position.y;
            
            return dx * dx + dy * dy;
        }
    }
    public class UnitManager : SceneSingleton<UnitManager>
    {
        private Dictionary<Faction, List<TransformUnitPair>> units = new();
        
        public List<Unit> allies;
        public List<Unit> enemies;

        protected override void Awake()
        {
            base.Awake();
            units[Faction.Ally] = allies?.Select(unit => new TransformUnitPair(unit.transform, unit)).ToList() ?? new ();
            units[Faction.Enemy] = enemies?.Select(unit => new TransformUnitPair(unit.transform, unit)).ToList() ?? new ();
        }

        /**
         * range 거리 안에 있는 적 Unit 을 반환합니다.
         * <returns>Unit|null</returns>
         */
        [CanBeNull]
        public Unit FindTarget(Faction faction, Vector2 position, float range)
        {
            List<TransformUnitPair> targets = units[faction == Faction.Ally ? Faction.Enemy :  Faction.Ally];
            
            if (targets == null || targets.Count == 0)
               return null;
            
            TransformUnitPair min = targets.OrderBy((pair) => pair.CirculateDistance2(position)).First();
            
            float distance = min.CirculateDistance2(position);
            
            return distance * distance <= range * range ?  min.unit : null;
        }
    }
}

