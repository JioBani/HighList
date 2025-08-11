using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Scenes.Battle.Tests.EditMode
{
    public class UnitMoveLogicEditModeTests
    {
        private GameObject go;
        private Scenes.Battle.Scripts.Unit unit;
        private Rigidbody2D rb;
        private MethodInfo moveMethod;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("UnitUnderTest_EditMode");
            rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.gravityScale = 0f;

            unit = go.AddComponent<Scenes.Battle.Scripts.Unit>();
            unit.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);

            moveMethod = typeof(Scenes.Battle.Scripts.Unit).GetMethod("Move", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(moveMethod, "Expected private Move method to exist.");
        }

        [TearDown]
        public void TearDown()
        {
            if (go != null) UnityEngine.Object.DestroyImmediate(go);
        }

        [Test]
        public void Move_Sets_Zero_Velocity_When_Within_Threshold()
        {
            go.transform.position = new Vector3(1f, 2f, 0f);
            var dest = new Vector2(1.005f, 2.005f);
            unit.speed = 10f;

            moveMethod.Invoke(unit, new object[] { dest });

            var v = GetLinearVelocityCompat(rb);
            Assert.That(v, Is.EqualTo(Vector2.zero).Using(new Vector2Comparer(1e-5f)));
        }

        [Test]
        public void Move_Sets_Normalized_Velocity_Times_Speed_When_Far()
        {
            go.transform.position = Vector3.zero;
            var dest = new Vector2(3f, 4f); // normalized is (0.6, 0.8)
            unit.speed = 5f;

            moveMethod.Invoke(unit, new object[] { dest });

            var v = GetLinearVelocityCompat(rb);
            Assert.That(v.magnitude, Is.EqualTo(5f).Within(1e-4f));
            Assert.That(Vector2.Angle(v, dest.normalized), Is.LessThan(1e-2f));
        }

        [Test]
        public void Move_With_Zero_Speed_Results_In_Zero_Velocity()
        {
            go.transform.position = Vector3.zero;
            var dest = new Vector2(10f, -7f);
            unit.speed = 0f;

            moveMethod.Invoke(unit, new object[] { dest });

            var v = GetLinearVelocityCompat(rb);
            Assert.That(v, Is.EqualTo(Vector2.zero).Using(new Vector2Comparer(1e-5f)));
        }

        private static Vector2 GetLinearVelocityCompat(Rigidbody2D rigidbody2D)
        {
            var t = typeof(Rigidbody2D);
            var propLinear = t.GetProperty("linearVelocity");
            if (propLinear != null)
            {
                object val = propLinear.GetValue(rigidbody2D, null);
                if (val is Vector2 v) return SanitizeVector(v);
            }
            return SanitizeVector(rigidbody2D.velocity);
        }

        private static Vector2 SanitizeVector(Vector2 v)
        {
            if (!float.IsFinite(v.x) || !float.IsFinite(v.y)) return Vector2.zero;
            return v;
        }

        private class Vector2Comparer : IEqualityComparer<Vector2>
        {
            private readonly float tol;
            public Vector2Comparer(float tolerance) => tol = Mathf.Max(0f, tolerance);
            public bool Equals(Vector2 a, Vector2 b) => (a - b).sqrMagnitude <= tol * tol;
            public int GetHashCode(Vector2 obj) => obj.GetHashCode();
        }
    }
}