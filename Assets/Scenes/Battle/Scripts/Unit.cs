using Scenes.Battle.Scripts.Enums;
using UnityEngine;

namespace Scenes.Battle.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Unit : MonoBehaviour, Movable
    {
        private Rigidbody2D _rigid;
        public Vector2 destination;
        public float speed;
        public UnitInfo unitInfo;
        public Unit target;
        private UnitManager unitManager;
        
        void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            
            // 임시 unit 정보
            unitInfo = new UnitInfo(
                Faction.Ally,
                5.0f
            );
            
            Init(UnitManager.Instance);
        }

        void Init(UnitManager unitManager)
        {
            this.unitManager = unitManager;
        }

        void FixedUpdate()
        {
            Move(destination);
        }

        public void Move(Vector2 _destination)
        {
            if (Vector2.Distance(_destination, transform.position) > 0.01f)
            {
                Vector2 towards = _destination - (Vector2)transform.position;
                _rigid.linearVelocity = towards.normalized * speed;
            }
            else
            {
                _rigid.linearVelocity = Vector2.zero;
            }
        }

        private void FindTarget()
        {
            target = unitManager.FindTarget(
                faction: unitInfo.faction,
                position: transform.position,
                range:  unitInfo.range
            );
        }
    }
}

