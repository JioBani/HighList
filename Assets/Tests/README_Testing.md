Testing Framework and How to Run

- Framework: NUnit with Unity Test Framework (UnityEngine.TestTools).
- Test Types:
  - PlayMode tests: Assets/Tests/PlayMode/UnitMovementTests.cs (uses [UnityTest] to step physics and validate MonoBehaviour behavior).
  - EditMode tests: Assets/Tests/EditMode/UnitMoveLogicEditModeTests.cs (invokes private Move via reflection to validate core math deterministically).
- Running:
  - Open Unity Editor, open Test Runner window (Window > General > Test Runner).
  - Run PlayMode and EditMode test suites.