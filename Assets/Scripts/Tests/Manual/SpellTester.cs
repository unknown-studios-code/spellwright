using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Spellwright.Components.Common;
using Spellwright.Components.Spawning;

namespace Spellwright.Tests.Manual
{
    [RequireComponent(typeof(Camera))]
    public class SpellTester : MonoBehaviour
    {
        [Header("Spawn Configuration")]
        [SerializeField] private Key spawnKey = Key.Space;
        [SerializeField] private int spawnCount = 1;
        [SerializeField] private float spawnRadius = 2f;
        [SerializeField] private float spellSpeed = 15f;
        [SerializeField] private float spellLifetime = 5f;
        [SerializeField] private float3 spawnPosition = float3.zero;
        [SerializeField] private float3 spawnDirection = new float3(0, 0, 1);

        [Header("Visualization")]
        [SerializeField] private bool showVelocityVectors = true;
        [SerializeField] private bool showSpawnPoints = true;
        [SerializeField] private bool showLifetimeIndicators = true;
        [SerializeField] private bool showLifetimeText = true;
        [SerializeField] private float vectorScale = 1f;
        [SerializeField] private Color velocityColor = Color.cyan;
        [SerializeField] private Color spawnPointColor = Color.yellow;
        [SerializeField] private Color lifetimeFullColor = Color.green;
        [SerializeField] private Color lifetimeExpiredColor = Color.red;
        [SerializeField] private float sphereRadius = 0.2f;

        [Header("Lifecycle Statistics")]
        [SerializeField] private bool showLifecycleStats = true;
        [SerializeField] private Vector2 statsPosition = new Vector2(10, 10);

        [Header("Debug")]
        [SerializeField] private bool enableManualMovement = true;
        [SerializeField] private bool autoSpawn = false;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private bool verboseLogging = false;

        private EntityManager _entityManager;
        private Entity _testPrefab;
        private Entity _caster;
        private float _lastSpellSpeed;
        private float _lastSpellLifetime;
        private float _nextSpawnTime;
        private int _totalSpawned;
        private int _totalDestroyed;
        private EntityQuery _spellQuery;

        private void Start()
        {
            if (!ValidateSetup()) return;

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            InitializeQueries();
            InitializeTestEntities();
        }

        private bool ValidateSetup()
        {
            if (World.DefaultGameObjectInjectionWorld == null)
            {
                Debug.LogError("[SpellTester] No default world found. Ensure ECS is initialized.");
                return false;
            }
            return true;
        }

        private void InitializeQueries()
        {
            _spellQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Unity.Transforms.LocalTransform>(),
                ComponentType.ReadOnly<Velocity>(),
                ComponentType.ReadOnly<SpellOwner>(),
                ComponentType.ReadOnly<Lifetime>()
            );
        }

        private void Update()
        {
            HandlePrefabRecreation();
            HandleAutoSpawn();
            HandleManualSpawn();
            HandleManualMovement();
            TrackDestroyedEntities();
        }

        private void HandlePrefabRecreation()
        {
            if (math.abs(spellSpeed - _lastSpellSpeed) > 0.01f ||
                math.abs(spellLifetime - _lastSpellLifetime) > 0.01f)
            {
                RecreatePrefab();
            }
        }

        private void HandleAutoSpawn()
        {
            if (autoSpawn && Time.time >= _nextSpawnTime)
            {
                SpawnTestSpells();
                _nextSpawnTime = Time.time + spawnInterval;
            }
        }

        private void HandleManualSpawn()
        {
            if (Keyboard.current != null && Keyboard.current[spawnKey].wasPressedThisFrame)
            {
                SpawnTestSpells();
            }
        }

        private void HandleManualMovement()
        {
            if (enableManualMovement)
            {
                MoveSpells();
            }
        }

        private void TrackDestroyedEntities()
        {
            int currentCount = _spellQuery.CalculateEntityCount();
            int expectedAlive = _totalSpawned - _totalDestroyed;

            if (currentCount < expectedAlive)
            {
                _totalDestroyed += (expectedAlive - currentCount);
                LogVerbose($"Lifecycle destroyed {expectedAlive - currentCount} spell(s) | Total destroyed: {_totalDestroyed}");
            }
        }

        private void RecreatePrefab()
        {
            if (_entityManager != default && _entityManager.Exists(_testPrefab))
            {
                _entityManager.DestroyEntity(_testPrefab);
                LogInfo($"Prefab destroyed");
            }

            _testPrefab = CreateTestPrefab();
            _lastSpellSpeed = spellSpeed;
            _lastSpellLifetime = spellLifetime;

            LogInfo($"Prefab recreated | Speed: {spellSpeed:F2} | Lifetime: {spellLifetime:F2}s");
        }

        private void MoveSpells()
        {
            if (!_spellQuery.IsEmpty)
            {
                var entities = _spellQuery.ToEntityArray(Allocator.Temp);

                for (int i = 0; i < entities.Length; i++)
                {
                    var transform = _entityManager.GetComponentData<Unity.Transforms.LocalTransform>(entities[i]);
                    var velocity = _entityManager.GetComponentData<Velocity>(entities[i]);

                    transform.Position += velocity.Value * Time.deltaTime;
                    _entityManager.SetComponentData(entities[i], transform);
                }

                entities.Dispose();
            }
        }

        private void InitializeTestEntities()
        {
            _testPrefab = CreateTestPrefab();
            _caster = _entityManager.CreateEntity();
            _lastSpellSpeed = spellSpeed;
            _lastSpellLifetime = spellLifetime;

            LogInfo($"Initialized | Prefab: {_testPrefab.Index}:{_testPrefab.Version} | Caster: {_caster.Index}:{_caster.Version} | Press '{spawnKey}' to spawn");
        }

        private Entity CreateTestPrefab()
        {
            Entity prefab = _entityManager.CreateEntity();

            _entityManager.AddComponentData(prefab, new SpellOwner { OwnerEntity = Entity.Null });
            _entityManager.AddComponentData(prefab, new Unity.Transforms.LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            _entityManager.AddComponentData(prefab, new Speed { Value = spellSpeed });
            _entityManager.AddComponentData(prefab, new Lifetime { Duration = spellLifetime, SpawnTime = 0 });
            _entityManager.AddComponent<Prefab>(prefab);

            LogVerbose($"Prefab components | SpellOwner: ✓ | Transform: ✓ | Speed: {spellSpeed:F2} | Lifetime: {spellLifetime:F2}s | Prefab Tag: ✓");

            return prefab;
        }

        private void SpawnTestSpells()
        {
            float3 cameraPosition = transform.position;
            float3 cameraForward = transform.forward;

            float3 basePosition = math.lengthsq(spawnPosition) > 0.01f
                ? cameraPosition + spawnPosition
                : cameraPosition + cameraForward * 2f;

            float3 baseDirection = math.lengthsq(spawnDirection) > 0.01f
                ? math.normalize(spawnDirection)
                : cameraForward;

            int spawnedThisBatch = 0;
            for (int i = 0; i < spawnCount; i++)
            {
                float angle = (360f / spawnCount) * i;
                float3 offset = CalculateRadialOffset(angle);
                float3 finalPosition = basePosition + offset;
                float3 finalDirection = math.normalize(baseDirection + offset * 0.3f);

                CreateSpawnRequest(finalPosition, finalDirection);
                spawnedThisBatch++;
            }

            _totalSpawned += spawnedThisBatch;
            LogInfo($"Spawned {spawnedThisBatch} spell(s) | Total: {_totalSpawned} | Position: {basePosition:F2} | Direction: {baseDirection:F2} | Active: {_spellQuery.CalculateEntityCount()}");
        }

        private float3 CalculateRadialOffset(float angleDegrees)
        {
            if (spawnCount == 1) return float3.zero;

            float angleRad = math.radians(angleDegrees);
            float x = math.cos(angleRad) * spawnRadius;
            float y = math.sin(angleRad) * spawnRadius;

            return new float3(x, y, 0);
        }

        private void CreateSpawnRequest(float3 position, float3 direction)
        {
            Entity request = _entityManager.CreateEntity();

            _entityManager.AddComponentData(request, new SpawnRequest
            {
                PrefabEntity = _testPrefab,
                CasterEntity = _caster,
                SpawnPosition = position,
                SpawnDirection = direction
            });

            LogVerbose($"SpawnRequest created | Position: {position:F2} | Direction: {direction:F2}");
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || _entityManager == default || _spellQuery == default) return;

            DrawSpellEntities();
        }

        private void DrawSpellEntities()
        {
            if (_spellQuery.IsEmpty) return;

            var entities = _spellQuery.ToEntityArray(Allocator.Temp);
            var transforms = _spellQuery.ToComponentDataArray<Unity.Transforms.LocalTransform>(Allocator.Temp);
            var velocities = _spellQuery.ToComponentDataArray<Velocity>(Allocator.Temp);
            var lifetimes = _spellQuery.ToComponentDataArray<Lifetime>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                float3 position = transforms[i].Position;
                float3 velocity = velocities[i].Value;
                Lifetime lifetime = lifetimes[i];

                DrawSpawnPoint(position);
                DrawVelocityVector(position, velocity);
                DrawLifetimeIndicator(position, lifetime);
            }

            entities.Dispose();
            transforms.Dispose();
            velocities.Dispose();
            lifetimes.Dispose();
        }

        private void DrawSpawnPoint(float3 position)
        {
            if (!showSpawnPoints) return;

            Gizmos.color = spawnPointColor;
            Gizmos.DrawWireSphere(position, sphereRadius);
        }

        private void DrawVelocityVector(float3 position, float3 velocity)
        {
            if (!showVelocityVectors || math.lengthsq(velocity) < 0.001f) return;

            Gizmos.color = velocityColor;
            float3 normalized = math.normalize(velocity);
            float3 endPoint = position + normalized * vectorScale;

            Gizmos.DrawLine(position, endPoint);
            DrawArrowHead(position, endPoint, 0.2f);
        }

        private void DrawLifetimeIndicator(float3 position, Lifetime lifetime)
        {
            if (!showLifetimeIndicators) return;

            double elapsedTime = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
            float remainingTime = lifetime.Duration - (float)(elapsedTime - lifetime.SpawnTime);
            float normalizedLifetime = math.clamp(remainingTime / lifetime.Duration, 0f, 1f);

            Gizmos.color = Color.Lerp(lifetimeExpiredColor, lifetimeFullColor, normalizedLifetime);
            Gizmos.DrawWireSphere(position, sphereRadius * 0.5f);

            if (showLifetimeText && Camera.current != null)
            {
                Vector3 screenPos = Camera.current.WorldToScreenPoint(position);
                if (screenPos.z > 0)
                {
                    Vector3 guiPos = new Vector3(screenPos.x, Screen.height - screenPos.y, 0);
                    DrawLifetimeLabel(guiPos, remainingTime);
                }
            }
        }

        private void DrawLifetimeLabel(Vector3 guiPosition, float remainingTime)
        {
#if UNITY_EDITOR
            GUIStyle style = new GUIStyle();
            style.normal.textColor = remainingTime > 1f ? lifetimeFullColor : lifetimeExpiredColor;
            style.fontSize = 10;
            style.alignment = TextAnchor.MiddleCenter;

            string label = remainingTime > 0 ? $"{remainingTime:F1}s" : "EXPIRED";
            Vector2 size = style.CalcSize(new GUIContent(label));
            Rect rect = new Rect(guiPosition.x - size.x / 2, guiPosition.y - size.y / 2, size.x, size.y);

            UnityEditor.Handles.BeginGUI();
            GUI.Label(rect, label, style);
            UnityEditor.Handles.EndGUI();
#endif
        }

        private void DrawArrowHead(float3 start, float3 end, float arrowSize)
        {
            float3 direction = math.normalize(end - start);
            float3 right = math.cross(direction, math.up());
            float3 up = math.cross(right, direction);

            if (math.lengthsq(right) < 0.001f)
            {
                right = math.cross(direction, math.forward());
                up = math.cross(right, direction);
            }

            float3 arrowTip1 = end - direction * arrowSize + right * arrowSize * 0.5f;
            float3 arrowTip2 = end - direction * arrowSize - right * arrowSize * 0.5f;

            Gizmos.DrawLine(end, arrowTip1);
            Gizmos.DrawLine(end, arrowTip2);
        }

        private void OnGUI()
        {
            if (!showLifecycleStats || !Application.isPlaying) return;

            DrawLifecycleStatistics();
        }

        private void DrawLifecycleStatistics()
        {
            int activeCount = _spellQuery.CalculateEntityCount();

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.alignment = TextAnchor.UpperLeft;
            boxStyle.padding = new RectOffset(10, 10, 10, 10);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 12;
            labelStyle.normal.textColor = Color.white;

            string stats = $"<b>Lifecycle Statistics</b>\n" +
                          $"Active Spells: {activeCount}\n" +
                          $"Total Spawned: {_totalSpawned}\n" +
                          $"Total Destroyed: {_totalDestroyed}\n" +
                          $"Lifetime: {spellLifetime:F1}s\n" +
                          $"Speed: {spellSpeed:F1} u/s\n" +
                          $"\n<b>Controls</b>\n" +
                          $"Spawn: {spawnKey}\n" +
                          $"Auto Spawn: {(autoSpawn ? "ON" : "OFF")}";

            GUIContent content = new GUIContent(stats);
            Vector2 size = labelStyle.CalcSize(content);
            Rect boxRect = new Rect(statsPosition.x, statsPosition.y, size.x + 20, size.y + 20);

            GUI.Box(boxRect, "", boxStyle);
            GUI.Label(new Rect(statsPosition.x + 10, statsPosition.y + 10, size.x, size.y), stats, labelStyle);
        }

        private void LogInfo(string message)
        {
            Debug.Log($"[SpellTester] {message}");
        }

        private void LogVerbose(string message)
        {
            if (verboseLogging)
            {
                Debug.Log($"[SpellTester] [VERBOSE] {message}");
            }
        }

        private void OnDestroy()
        {
            if (_entityManager != default)
            {
                if (_entityManager.Exists(_testPrefab))
                {
                    _entityManager.DestroyEntity(_testPrefab);
                }
                if (_entityManager.Exists(_caster))
                {
                    _entityManager.DestroyEntity(_caster);
                }
            }

            LogInfo($"Destroyed | Total spawned: {_totalSpawned} | Total destroyed: {_totalDestroyed}");
        }
    }
}

