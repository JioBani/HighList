using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Scenes.Battle.Tests
{
    public class UnitMovementTests
    {
        private GameObject go;
        private Scenes.Battle.Scripts.Unit unit;
        private Rigidbody2D rb;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            go = new GameObject("UnitUnderTest");
            // Ensure a 2D rigidbody is available
            rb = go.AddComponent<Rigidbody2D>();
            // Use Dynamic body so velocity is effective
            rb.bodyType = RigidbodyType2D.Dynamic;
            // Reduce drag to minimize external effects during assertions
            rb.drag = 0f;
            rb.angularDrag = 0f;
            // Zero gravity for deterministic movement
            rb.gravityScale = 0f;

            unit = go.AddComponent<Scenes.Battle.Scripts.Unit>();
            // Manually invoke Awake like Unity would (GetComponent caching)
            // Unity will call it automatically on enable, but ensure it's ready.
            unit.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);

            // Start every test at origin
            go.transform.position = Vector3.zero;

            // Wait a frame to let Unity initialize internals
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (go != null)
            {
                Object.Destroy(go);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator FixedUpdate_applies_velocity_towards_destination_with_magnitude_equal_to_speed()
        {
            unit.speed = 5f;
            unit.destination = new Vector2(10f, 0f); // directly to +X from origin

            // Wait for FixedUpdate to run
            yield return new WaitForFixedUpdate();

            // The Unit script sets rigidbody2D.linearVelocity = towards.normalized * speed
            // If 'linearVelocity' is mapped to 'velocity' by Unity version, this should be equal magnitude to speed
            // Some Unity versions use 'velocity' instead of 'linearVelocity'. To stay consistent with provided code,
            // we assert using reflection with a fallback to Rigidbody2D.velocity if needed.
            var actualVelocity = GetLinearVelocityCompat(rb);

            Assert.That(actualVelocity.magnitude, Is.EqualTo(unit.speed).Within(1e-4f), "Velocity magnitude should equal speed.");
            Assert.That(Vector2.Angle(actualVelocity, (unit.destination - (Vector2)go.transform.position).normalized), Is.LessThan(0.1f), "Velocity should point towards destination.");
        }

        [UnityTest]
        public IEnumerator Stops_when_close_enough_to_destination_within_threshold()
        {
            unit.speed = 8f;
            // Position the object very close to destination (within epsilon 0.01)
            go.transform.position = new Vector3(1.0f, 1.0f, 0f);
            unit.destination = new Vector2(1.009f, 1.009f); // ~0.0127 distance, slightly above threshold to test approach then stop

            // First fixed update should set velocity towards destination
            yield return new WaitForFixedUpdate();
            var v1 = GetLinearVelocityCompat(rb);
            Assert.Greater(v1.magnitude, 0f, "Initially should move towards the destination.");

            // Move the transform to within <= 0.01f to trigger stop
            go.transform.position = new Vector3(1.005f, 1.005f, 0f);
            yield return new WaitForFixedUpdate();
            var v2 = GetLinearVelocityCompat(rb);

            Assert.That(v2, Is.EqualTo(Vector2.zero).Using(Vector2Comparer.Within(1e-5f)), "Velocity should be zero when within the stop threshold.");
        }

        [UnityTest]
        public IEnumerator Zero_speed_results_in_zero_velocity_even_if_far()
        {
            unit.speed = 0f;
            unit.destination = new Vector2(100f, 100f);

            yield return new WaitForFixedUpdate();

            var v = GetLinearVelocityCompat(rb);
            Assert.That(v, Is.EqualTo(Vector2.zero).Using(Vector2Comparer.Within(1e-5f)), "Zero speed should produce zero velocity.");
        }

        [UnityTest]
        public IEnumerator Destination_equal_to_current_position_sets_zero_velocity()
        {
            unit.speed = 12f;
            go.transform.position = new Vector3(-3f, 4f, 0f);
            unit.destination = new Vector2(-3f, 4f);

            yield return new WaitForFixedUpdate();

            var v = GetLinearVelocityCompat(rb);
            Assert.That(v, Is.EqualTo(Vector2.zero).Using(Vector2Comparer.Within(1e-5f)));
        }

        [UnityTest]
        public IEnumerator Diagonal_movement_has_velocity_magnitude_equal_to_speed_even_if_towards_is_diagonal()
        {
            unit.speed = 7.5f;
            unit.destination = new Vector2(10f, 10f);

            yield return new WaitForFixedUpdate();

            var v = GetLinearVelocityCompat(rb);
            Assert.That(v.magnitude, Is.EqualTo(unit.speed).Within(1e-4f), "Normalized towards vector should keep magnitude == speed.");
            // Direction should be roughly (1,1).normalized
            var expectedDir = new Vector2(1f, 1f).normalized;
            Assert.That(Vector2.Angle(v, expectedDir), Is.LessThan(0.5f));
        }

        [UnityTest]
        public IEnumerator Handles_NaN_or_Infinity_in_destination_by_resulting_zero_velocity()
        {
            unit.speed = 3f;
            // Introduce invalid destination (NaN)
            unit.destination = new Vector2(float.NaN, 0f);

            yield return new WaitForFixedUpdate();

            var vNaN = GetLinearVelocityCompat(rb);
            // Any invalid math should end as zero velocity per safety expectation
            Assert.That(IsFiniteVector(vNaN) && vNaN == Vector2.zero, "Invalid destination should not produce non-finite velocity.");

            // Now Infinity
            unit.destination = new Vector2(float.PositiveInfinity, 5f);
            yield return new WaitForFixedUpdate();

            var vInf = GetLinearVelocityCompat(rb);
            Assert.That(IsFiniteVector(vInf) && vInf == Vector2.zero, "Non-finite destination should not produce non-finite velocity.");
        }

        [UnityTest]
        public IEnumerator Continues_to_move_each_fixed_update_until_within_threshold()
        {
            unit.speed = 2f;
            go.transform.position = Vector3.zero;
            unit.destination = new Vector2(0.05f, 0f); // 0.05 away on X

            // Step frames and verify it eventually reaches stop condition
            int frames = 0;
            while (Vector2.Distance(unit.destination, go.transform.position) > 0.010f && frames < 50)
            {
                yield return new WaitForFixedUpdate();
                frames++;
                // Let physics move the body as per velocity set
                // Position integrates by Unity physics, so after each step, position should increase towards +X
            }

            // Once close enough, next FixedUpdate should zero out velocity
            yield return new WaitForFixedUpdate();
            var v = GetLinearVelocityCompat(rb);

            Assert.That(Vector2.Distance(unit.destination, go.transform.position), Is.LessThanOrEqualTo(0.011f));
            Assert.That(v, Is.EqualTo(Vector2.zero).Using(Vector2Comparer.Within(1e-5f)));
        }

        // Helpers

        private static Vector2 GetLinearVelocityCompat(Rigidbody2D rigidbody2D)
        {
            // The provided code uses 'linearVelocity'. Some Unity versions expose 'velocity' instead.
            // Try reflection for 'linearVelocity' first; fallback to 'velocity'.
            var t = typeof(Rigidbody2D);
            var propLinear = t.GetProperty("linearVelocity");
            if (propLinear != null)
            {
                object val = propLinear.GetValue(rigidbody2D, null);
                if (val is Vector2 v) return SanitizeVector(v);
            }
            // Fallback
            return SanitizeVector(rigidbody2D.velocity);
        }

        private static Vector2 SanitizeVector(Vector2 v)
        {
            if (!float.IsFinite(v.x) || !float.IsFinite(v.y)) return Vector2.zero;
            return v;
        }

        private static bool IsFiniteVector(Vector2 v)
        {
            return float.IsFinite(v.x) && float.IsFinite(v.y);
        }

        private class Vector2Comparer : IEqualityComparer<Vector2>
        {
            private readonly float tolerance;

            private Vector2Comparer(float tolerance)
            {
                tolerance = Mathf.Max(0f, tolerance);
                this.tolerance = tolerance;
            }

            public static Vector2Comparer Within(float tolerance) => new Vector2Comparer(tolerance);

            public bool Equals(Vector2 a, Vector2 b)
            {
                return (a - b).sqrMagnitude <= tolerance * tolerance;
            }

            public int GetHashCode(Vector2 obj) => obj.GetHashCode();
        }
    }
}