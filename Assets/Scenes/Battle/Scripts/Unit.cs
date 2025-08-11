using System;
using Common.Packages.PathFinder;
using UnityEngine;

namespace Scenes.Battle.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Unit : MonoBehaviour
    {
        private Rigidbody2D rigidbody2D;
        public Vector2 destination;
        public float speed;
        
        void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void FixedUpdate()
        {
            Move(destination);
        }

        void Move(Vector2 _destination)
        {
            // TODO 대각선 이동의 경우 speed 가 더 빨라지는 현상 해결
            if (Vector2.Distance(_destination, transform.position) > 0.01f)
            {
                Vector2 towards = _destination - (Vector2)transform.position;
                rigidbody2D.linearVelocity = towards.normalized * speed;
            }
            else
            {
                rigidbody2D.linearVelocity = Vector2.zero;
            }
        }
    }
}

