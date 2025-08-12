using System;
using Common.Packages.PathFinder;
using UnityEngine;

namespace Scenes.Battle.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Unit : MonoBehaviour, Movable
    {
        private Rigidbody2D _rigid;
        public Vector2 destination;
        public float speed;
        
        void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
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
    }
}

