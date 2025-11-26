using System.Collections.Generic;
using Spellwright.Components.Collision;
using Spellwright.Components.Common;
using Spellwright.Components.Health;
using Spellwright.Components.Payloads;
using Spellwright.Components.Spawning;
using Spellwright.Components.StatusEffect;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spellwright.Tests.Manual
{
    [RequireComponent(typeof(Camera))]
    public class SpellTester : MonoBehaviour
    {
        private const float POSITION_EPSILON = 0.01f;
        private const float CONE_SEGMENTS = 16f;
        private const float ARROW_SIZE = 0.2f;
        private const float COLLISION_EVENT_SPHERE_RADIUS = 0.3f;
        private const float COLLISION_EVENT_CENTER_RADIUS = 0.1f;

        [Header("Spawn Configuration")]
        [SerializeField]
        private Key _spawnKey = Key.Space;

        [SerializeField]
        private int _spawnCount = 1;

        [SerializeField]
        private float _spawnRadius = 1f;

        [SerializeField]
        private float _spellSpeed = 10f;

        [SerializeField]
        private float _spellLifetime = 5f;

        [SerializeField]
        private float3 _spawnPosition = float3.zero;

        [Header("Visualization")]
        [SerializeField]
        private bool _showVelocityVectors = true;

        [SerializeField]
        private bool _showSpawnPoints = true;

        [SerializeField]
        private bool _showLifetimeIndicators = true;

        [SerializeField]
        private bool _showLifetimeText = true;

        [SerializeField]
        private float _vectorScale = 1f;

        [SerializeField]
        private Color _velocityColor = Color.cyan;

        [SerializeField]
        private Color _spawnPointColor = Color.yellow;

        [SerializeField]
        private Color _lifetimeFullColor = Color.green;

        [SerializeField]
        private Color _lifetimeExpiredColor = Color.red;

        [SerializeField]
        private float _sphereRadius = 0.2f;

        [Header("Lifecycle Statistics")]
        [SerializeField]
        private bool _showLifecycleStats = true;

        [SerializeField]
        private Vector2 _statsPosition = new(10, 10);

        [Header("Physics Configuration")]
        [SerializeField]
        private GeneratorType _generatorType = GeneratorType.Projectile;

        [SerializeField]
        private float _collisionRadius = 0.5f;

        [SerializeField]
        private float _coneAngle = 45f;

        [SerializeField]
        private float _coneRadius = 10f;

        [SerializeField]
        private float _aoeRadius = 5f;

        [Header("Target Configuration")]
        [SerializeField]
        private bool _spawnTargets = true;

        [SerializeField]
        private int _targetCount = 5;

        [SerializeField]
        private float _targetSpawnRadius = 5f;

        [SerializeField]
        private float _targetCollisionRadius = 0.5f;

        [SerializeField]
        private float _targetMaxHealth = 100f;

        [SerializeField]
        private Key _spawnTargetKey = Key.T;

        [Header("Payload Configuration")]
        [SerializeField]
        private bool _enablePayloads = true;

        [SerializeField]
        private TestPayloadType _payloadType = TestPayloadType.Damage;

        [SerializeField]
        private ElementalType _elementalType = ElementalType.Fire;

        [SerializeField]
        private float _payloadAmount = 25f;

        [SerializeField]
        private float _payloadDuration = 3f;

        [Header("Collision Visualization")]
        [SerializeField]
        private bool _showCollisionHits = true;

        [SerializeField]
        private bool _showCollisionRadii = true;

        [SerializeField]
        private bool _showTargetHealth = true;

        [SerializeField]
        private Color _collisionEventColor = Color.red;

        [SerializeField]
        private Color _collisionRadiusColor = new(1f, 0.5f, 0f, 0.3f);

        [SerializeField]
        private float _collisionEventDuration = 0.5f;

        [Header("Debug")]
        [SerializeField]
        private bool _autoSpawn = false;

        [SerializeField]
        private float _spawnInterval = 1f;

        [SerializeField]
        private bool _verboseLogging = false;

        private enum GeneratorType
        {
            Projectile,
            Cone,
            AreaOfEffect,
        }

        private enum TestPayloadType
        {
            Damage,
            Heal,
            Slow,
            Burn,
            Stun,
            Poison,
            SpawnEntity,
        }

        private struct CollisionHitData
        {
            public float3 Position;
            public float Time;
        }

        private EntityManager _entityManager;
        private Entity _testPrefab;
        private Entity _caster;

        private float _lastSpellSpeed;
        private float _lastSpellLifetime;
        private GeneratorType _lastGeneratorType;
        private float _lastCollisionRadius;
        private float _lastConeAngle;
        private float _lastConeRadius;
        private float _lastAoeRadius;
        private bool _lastEnablePayloads;
        private TestPayloadType _lastPayloadType;
        private ElementalType _lastElementalType;
        private float _lastPayloadAmount;
        private float _lastPayloadDuration;
        private float _lastTargetCollisionRadius;

        private float _nextSpawnTime;
        private int _totalSpawned;
        private int _totalDestroyed;
        private int _totalCollisions;
        private EntityQuery _spellQuery;
        private EntityQuery _targetQuery;
        private EntityQuery _collisionQuery;
        private readonly List<CollisionHitData> _collisionEvents = new();

        private BlobAssetReference<Unity.Physics.Collider> _projectileColliderBlob;
        private BlobAssetReference<Unity.Physics.Collider> _targetColliderBlob;

        private void Start()
        {
            if (!ValidateSetup())
            {
                return;
            }

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            InitializeQueries();
            InitializeTestEntities();
        }

        private void Update()
        {
            HandleGeneratorTypeSelection();
            HandlePrefabRecreation();
            HandleAutoSpawn();
            HandleManualSpawn();
            HandleTargetSpawning();
            TrackDestroyedEntities();
            TrackCollisionHits();
            CleanupExpiredCollisionHits();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || _entityManager == default || _targetQuery == default)
            {
                return;
            }

            DrawSpellEntities();
            DrawTargetEntities();
            DrawCollisionHits();
            DrawInstantSpellPreview();
        }

        private void OnGUI()
        {
            if (_showLifecycleStats && Application.isPlaying)
            {
                DrawLifecycleStatistics();
            }
        }

        private void OnDestroy()
        {
            CleanupEntities();
            DisposeBlobs();
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
                ComponentType.ReadOnly<PhysicsVelocity>(),
                ComponentType.ReadOnly<SpellOwner>(),
                ComponentType.ReadOnly<Lifetime>()
            );

            _targetQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<Unity.Transforms.LocalTransform>(), ComponentType.ReadOnly<Health>());

            _collisionQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<Components.Collision.CollisionHit>());
        }

        private void InitializeTestEntities()
        {
            DisposeBlobs();

            _testPrefab = CreateTestPrefab();
            _caster = _entityManager.CreateEntity();
            CacheCurrentConfiguration();

            if (_spawnTargets)
            {
                SpawnTargets();
            }

            LogInfo(
                $"Initialized | Prefab: {_testPrefab.Index}:{_testPrefab.Version} | Caster: {_caster.Index}:{_caster.Version} | Press '{_spawnKey}' to spawn spells | Press '{_spawnTargetKey}' to spawn targets"
            );
        }

        private void HandleGeneratorTypeSelection()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            if (Keyboard.current[Key.Digit1].wasPressedThisFrame)
            {
                ChangeGeneratorType(GeneratorType.Projectile);
            }
            else if (Keyboard.current[Key.Digit2].wasPressedThisFrame)
            {
                ChangeGeneratorType(GeneratorType.Cone, $"Angle: {_coneAngle}°, Radius: {_coneRadius}m");
            }
            else if (Keyboard.current[Key.Digit3].wasPressedThisFrame)
            {
                ChangeGeneratorType(GeneratorType.AreaOfEffect, $"Radius: {_aoeRadius}m");
            }
        }

        private void ChangeGeneratorType(GeneratorType newType, string additionalInfo = "")
        {
            _generatorType = newType;
            string info = string.IsNullOrEmpty(additionalInfo) ? newType.ToString() : $"{newType} ({additionalInfo})";
            LogInfo($"Generator type changed to: {info}");
        }

        private void HandlePrefabRecreation()
        {
            if (NeedsPrefabRecreation())
            {
                RecreatePrefab();
            }
        }

        private bool NeedsPrefabRecreation()
        {
            return math.abs(_spellSpeed - _lastSpellSpeed) > POSITION_EPSILON
                || math.abs(_spellLifetime - _lastSpellLifetime) > POSITION_EPSILON
                || _generatorType != _lastGeneratorType
                || math.abs(_collisionRadius - _lastCollisionRadius) > POSITION_EPSILON
                || math.abs(_coneAngle - _lastConeAngle) > POSITION_EPSILON
                || math.abs(_coneRadius - _lastConeRadius) > POSITION_EPSILON
                || math.abs(_aoeRadius - _lastAoeRadius) > POSITION_EPSILON
                || _enablePayloads != _lastEnablePayloads
                || _payloadType != _lastPayloadType
                || _elementalType != _lastElementalType
                || math.abs(_payloadAmount - _lastPayloadAmount) > POSITION_EPSILON
                || math.abs(_payloadDuration - _lastPayloadDuration) > POSITION_EPSILON;
        }

        private void RecreatePrefab()
        {
            if (!_spellQuery.IsEmpty)
            {
                _entityManager.DestroyEntity(_spellQuery);
            }

            if (_entityManager != default && _entityManager.Exists(_testPrefab))
            {
                _entityManager.DestroyEntity(_testPrefab);
                LogInfo("Prefab destroyed");
            }

            if (_projectileColliderBlob.IsCreated)
            {
                _projectileColliderBlob.Dispose();
            }

            _testPrefab = CreateTestPrefab();
            CacheCurrentConfiguration();

            LogInfo($"Prefab recreated | Speed: {_spellSpeed:F2} | Lifetime: {_spellLifetime:F2}s | Type: {_generatorType}");
        }

        private void CacheCurrentConfiguration()
        {
            _lastSpellSpeed = _spellSpeed;
            _lastSpellLifetime = _spellLifetime;
            _lastGeneratorType = _generatorType;
            _lastCollisionRadius = _collisionRadius;
            _lastConeAngle = _coneAngle;
            _lastConeRadius = _coneRadius;
            _lastAoeRadius = _aoeRadius;
            _lastEnablePayloads = _enablePayloads;
            _lastPayloadType = _payloadType;
            _lastElementalType = _elementalType;
            _lastPayloadAmount = _payloadAmount;
            _lastPayloadDuration = _payloadDuration;
        }

        private void HandleAutoSpawn()
        {
            if (_autoSpawn && Time.time >= _nextSpawnTime)
            {
                SpawnTestSpells();
                _nextSpawnTime = Time.time + _spawnInterval;
            }
        }

        private void HandleManualSpawn()
        {
            if (Keyboard.current != null && Keyboard.current[_spawnKey].wasPressedThisFrame)
            {
                SpawnTestSpells();
            }
        }

        private void HandleTargetSpawning()
        {
            if (_spawnTargets && Keyboard.current != null && Keyboard.current[_spawnTargetKey].wasPressedThisFrame)
            {
                SpawnTargets();
            }
        }

        private void TrackDestroyedEntities()
        {
            int currentCount = _spellQuery.CalculateEntityCount();
            int expectedAlive = _totalSpawned - _totalDestroyed;

            if (currentCount < expectedAlive)
            {
                int destroyed = expectedAlive - currentCount;
                _totalDestroyed += destroyed;
                LogVerbose($"Lifecycle destroyed {destroyed} spell(s) | Total destroyed: {_totalDestroyed}");
            }
        }

        private void TrackCollisionHits()
        {
            if (_collisionQuery.IsEmpty)
            {
                return;
            }

            using NativeArray<Entity> entities = _collisionQuery.ToEntityArray(Allocator.Temp);

            foreach (Entity entity in entities)
            {
                if (!_entityManager.HasBuffer<Components.Collision.CollisionHit>(entity))
                {
                    continue;
                }

                DynamicBuffer<Components.Collision.CollisionHit> buffer = _entityManager.GetBuffer<Components.Collision.CollisionHit>(entity);
                foreach (Components.Collision.CollisionHit collision in buffer)
                {
                    _collisionEvents.Add(new CollisionHitData { Position = collision.ImpactPosition, Time = Time.time });
                    _totalCollisions++;
                    LogVerbose($"Collision detected | Spell: {entity.Index} | Target: {collision.TargetEntity.Index}");
                }
            }
        }

        private void CleanupExpiredCollisionHits()
        {
            for (int i = _collisionEvents.Count - 1; i >= 0; i--)
            {
                if (Time.time - _collisionEvents[i].Time > _collisionEventDuration)
                {
                    _collisionEvents.RemoveAt(i);
                }
            }
        }

        private Entity CreateTestPrefab()
        {
            Entity prefab = _entityManager.CreateEntity();

            AddCommonComponents(prefab);

            if (_generatorType == GeneratorType.Projectile)
            {
                AddProjectileComponents(prefab);
            }

            _entityManager.AddComponent<Prefab>(prefab);

            LogVerbose($"Prefab components | SpellOwner: ✓ | Transform: ✓ | Speed: {_spellSpeed:F2} | Lifetime: {_spellLifetime:F2}s | Type: {GetGeneratorInfo()}");

            return prefab;
        }

        private void AddCommonComponents(Entity entity)
        {
            _entityManager.AddComponentData(entity, new SpellOwner { OwnerEntity = Entity.Null });
            _entityManager.AddComponentData(
                entity,
                new Unity.Transforms.LocalTransform
                {
                    Position = float3.zero,
                    Rotation = quaternion.identity,
                    Scale = 1f,
                }
            );
            _entityManager.AddComponentData(entity, new Unity.Transforms.LocalToWorld());
            _entityManager.AddComponentData(entity, new Speed { Value = _spellSpeed });
            _entityManager.AddComponentData(entity, new Lifetime { Duration = _spellLifetime, SpawnTime = 0 });

            if (_enablePayloads)
            {
                AddPayloadBuffer(entity);
            }
        }

        private void AddProjectileComponents(Entity entity)
        {
            _entityManager.AddComponentData(entity, new PhysicsVelocity { Linear = float3.zero, Angular = float3.zero });

            if (!_projectileColliderBlob.IsCreated)
            {
                Unity.Physics.Material material = Unity.Physics.Material.Default;
                material.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;

                _projectileColliderBlob = Unity.Physics.SphereCollider.Create(
                    new SphereGeometry { Center = float3.zero, Radius = _collisionRadius },
                    CollisionLayers.CreateProjectileFilter(),
                    material
                );
                LogVerbose($"Created new Projectile Collider Blob (Radius: {_collisionRadius})");
            }

            _entityManager.AddComponentData(entity, new PhysicsCollider { Value = _projectileColliderBlob });
            _entityManager.AddSharedComponentManaged(entity, new PhysicsWorldIndex { Value = 0 });
            _entityManager.AddComponentData(entity, new ProjectileTag());

            LogVerbose("Physics components added: PhysicsVelocity ✓ | PhysicsCollider (IsTrigger) ✓ | PhysicsWorldIndex=0 ✓");
        }

        private string GetGeneratorInfo()
        {
            return _generatorType switch
            {
                GeneratorType.Cone => $"Cone (Angle: {_coneAngle}°, Radius: {_coneRadius}m)",
                GeneratorType.AreaOfEffect => $"AoE (Radius: {_aoeRadius}m)",
                GeneratorType.Projectile => $"Projectile (Radius: {_collisionRadius}m)",
                _ => throw new System.NotImplementedException($"Generator type {_generatorType} not implemented"),
            };
        }

        private void SpawnTestSpells()
        {
            float3 cameraPosition = transform.position;
            var cameraForward = new float3(transform.forward.x, transform.forward.y, transform.forward.z);
            float3 basePosition = CalculateBasePosition(cameraPosition, cameraForward);
            float3 baseDirection = cameraForward;

            int spawnedThisBatch = 0;
            for (int i = 0; i < _spawnCount; i++)
            {
                float angle = 360f / _spawnCount * i;
                float3 offset = CalculateRadialOffset(angle);
                float3 finalPosition = basePosition + offset;
                float3 finalDirection = math.normalize(baseDirection + (offset * 0.3f));

                CreateSpawnRequest(finalPosition, finalDirection);
                spawnedThisBatch++;
            }

            _totalSpawned += spawnedThisBatch;
            LogInfo($"Spawned {spawnedThisBatch} spell(s) | Total: {_totalSpawned} | Position: {basePosition:F2} | Direction: {baseDirection:F2} | Active: {_spellQuery.CalculateEntityCount()}");
        }

        private float3 CalculateBasePosition(float3 cameraPosition, float3 cameraForward)
        {
            return math.lengthsq(_spawnPosition) > POSITION_EPSILON ? cameraPosition + _spawnPosition : cameraPosition + (cameraForward * 2f);
        }

        private float3 CalculateRadialOffset(float angleDegrees)
        {
            if (_spawnCount == 1)
            {
                return float3.zero;
            }

            float angleRad = math.radians(angleDegrees);
            return new float3(math.cos(angleRad) * _spawnRadius, math.sin(angleRad) * _spawnRadius, 0);
        }

        private void CreateSpawnRequest(float3 position, float3 direction)
        {
            switch (_generatorType)
            {
                case GeneratorType.Projectile:
                    CreateProjectileRequest(position, direction);
                    break;

                case GeneratorType.Cone:
                    CreateConeRequest(position, direction);
                    break;

                case GeneratorType.AreaOfEffect:
                    CreateAoeRequest(position);
                    break;
            }
        }

        private void CreateProjectileRequest(float3 position, float3 direction)
        {
            Entity request = _entityManager.CreateEntity();
            _entityManager.AddComponentData(
                request,
                new SpawnRequest
                {
                    PrefabEntity = _testPrefab,
                    CasterEntity = _caster,
                    SpawnPosition = position,
                    SpawnDirection = direction,
                }
            );
            LogVerbose($"Projectile SpawnRequest created | Position: {position:F2} | Direction: {direction:F2}");
        }

        private void CreateConeRequest(float3 position, float3 direction)
        {
            Entity request = _entityManager.CreateEntity();
            _entityManager.AddComponentData(
                request,
                new ConeRequest
                {
                    Position = position,
                    Direction = direction,
                    AngleDegrees = _coneAngle,
                    Radius = _coneRadius,
                    SourceEntity = _caster,
                    CheckLineOfSight = true,
                }
            );

            if (_enablePayloads)
            {
                AddPayloadBuffer(request);
            }

            LogVerbose($"Cone request created | Position: {position:F2} | Direction: {direction:F2} | Angle: {_coneAngle}° | Radius: {_coneRadius}m");
        }

        private void CreateAoeRequest(float3 position)
        {
            Entity request = _entityManager.CreateEntity();
            _entityManager.AddComponentData(
                request,
                new AoeRequest
                {
                    Position = position,
                    Radius = _aoeRadius,
                    SourceEntity = _caster,
                    CheckLineOfSight = true,
                }
            );

            if (_enablePayloads)
            {
                AddPayloadBuffer(request);
            }

            LogVerbose($"AoE request created | Position: {position:F2} | Radius: {_aoeRadius}m");
        }

        private void AddPayloadBuffer(Entity entity)
        {
            switch (_payloadType)
            {
                case TestPayloadType.Damage:
                    DynamicBuffer<DamagePayloadRequest> damageBuffer = _entityManager.AddBuffer<DamagePayloadRequest>(entity);
                    damageBuffer.Add(new DamagePayloadRequest { Element = _elementalType, Amount = _payloadAmount });
                    break;

                case TestPayloadType.Heal:
                    DynamicBuffer<HealPayloadRequest> healBuffer = _entityManager.AddBuffer<HealPayloadRequest>(entity);
                    healBuffer.Add(new HealPayloadRequest { Amount = _payloadAmount });
                    break;

                case TestPayloadType.Slow:
                    DynamicBuffer<SlowPayloadRequest> slowBuffer = _entityManager.AddBuffer<SlowPayloadRequest>(entity);
                    slowBuffer.Add(new SlowPayloadRequest { Amount = _payloadAmount, Duration = _payloadDuration });
                    break;

                case TestPayloadType.Burn:
                    DynamicBuffer<BurnPayloadRequest> burnBuffer = _entityManager.AddBuffer<BurnPayloadRequest>(entity);
                    burnBuffer.Add(new BurnPayloadRequest { Amount = _payloadAmount, Duration = _payloadDuration });
                    break;

                case TestPayloadType.Stun:
                    DynamicBuffer<StunPayloadRequest> stunBuffer = _entityManager.AddBuffer<StunPayloadRequest>(entity);
                    stunBuffer.Add(new StunPayloadRequest { Duration = _payloadDuration });
                    break;

                case TestPayloadType.Poison:
                    DynamicBuffer<PoisonPayloadRequest> poisonBuffer = _entityManager.AddBuffer<PoisonPayloadRequest>(entity);
                    poisonBuffer.Add(new PoisonPayloadRequest { Amount = _payloadAmount, Duration = _payloadDuration });
                    break;

                case TestPayloadType.SpawnEntity:
                    DynamicBuffer<SpawnEntityRequest> spawnBuffer = _entityManager.AddBuffer<SpawnEntityRequest>(entity);
                    spawnBuffer.Add(new SpawnEntityRequest { Prefab = Entity.Null });
                    break;
            }
        }

        private bool IsStatusEffectPayload()
        {
            return _payloadType == TestPayloadType.Slow || _payloadType == TestPayloadType.Burn || _payloadType == TestPayloadType.Stun || _payloadType == TestPayloadType.Poison;
        }

        private void SpawnTargets()
        {
            bool configChanged = math.abs(_targetCollisionRadius - _lastTargetCollisionRadius) > POSITION_EPSILON;
            bool targetsExist = !_targetQuery.IsEmpty;

            if (configChanged && targetsExist)
            {
                _entityManager.DestroyEntity(_targetQuery);
                LogInfo("Destroyed existing targets due to configuration change");
            }

            if (!_targetColliderBlob.IsCreated || configChanged)
            {
                if (_targetColliderBlob.IsCreated)
                {
                    _targetColliderBlob.Dispose();
                }

                _targetColliderBlob = Unity.Physics.SphereCollider.Create(new SphereGeometry { Center = float3.zero, Radius = _targetCollisionRadius }, CollisionLayers.CreateEnemyFilter());
                _lastTargetCollisionRadius = _targetCollisionRadius;
                LogVerbose($"Created new Target Collider Blob (Radius: {_targetCollisionRadius})");
            }

            float3 cameraPosition = transform.position;
            float3 cameraForward = transform.forward;
            float3 basePosition = cameraPosition + (cameraForward * 5f);

            int spawned = 0;
            for (int i = 0; i < _targetCount; i++)
            {
                float3 position = CalculateTargetPosition(basePosition, i);
                CreateTargetEntity(position);
                spawned++;
            }

            LogInfo($"Spawned {spawned} target(s) | Total targets: {_targetQuery.CalculateEntityCount()}");
        }

        private float3 CalculateTargetPosition(float3 basePosition, int index)
        {
            float angle = 360f / _targetCount * index;
            float angleRad = math.radians(angle);
            var offset = new float3(math.cos(angleRad) * _targetSpawnRadius, 0f, math.sin(angleRad) * _targetSpawnRadius);
            return basePosition + offset;
        }

        private void CreateTargetEntity(float3 position)
        {
            Entity target = _entityManager.CreateEntity();
            _entityManager.AddComponentData(
                target,
                new Unity.Transforms.LocalTransform
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1f,
                }
            );
            _entityManager.AddComponentData(target, new Unity.Transforms.LocalToWorld());

            _entityManager.AddComponentData(target, new PhysicsCollider { Value = _targetColliderBlob });
            _entityManager.AddSharedComponentManaged(target, new PhysicsWorldIndex { Value = 0 });
            _entityManager.AddComponentData(target, new Health { Current = _targetMaxHealth, Maximum = _targetMaxHealth });
            _entityManager.AddBuffer<HealthUpdateRequest>(target);
            _entityManager.AddBuffer<StatusEffectStack>(target);
        }

        private void DrawSpellEntities()
        {
            if (_spellQuery.IsEmpty)
            {
                return;
            }

            using NativeArray<Entity> entities = _spellQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<Unity.Transforms.LocalTransform> transforms = _spellQuery.ToComponentDataArray<Unity.Transforms.LocalTransform>(Allocator.Temp);
            using NativeArray<PhysicsVelocity> velocities = _spellQuery.ToComponentDataArray<PhysicsVelocity>(Allocator.Temp);
            using NativeArray<Lifetime> lifetimes = _spellQuery.ToComponentDataArray<Lifetime>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                float3 position = transforms[i].Position;
                float3 velocity = velocities[i].Linear;
                Lifetime lifetime = lifetimes[i];

                DrawSpawnPoint(position);
                DrawVelocityVector(position, velocity);
                DrawLifetimeIndicator(position, lifetime);

                if (_generatorType == GeneratorType.Projectile)
                {
                    DrawCollisionRadius(position);
                }
            }
        }

        private void DrawTargetEntities()
        {
            if (_targetQuery.IsEmpty)
            {
                return;
            }

            using NativeArray<Entity> entities = _targetQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<Unity.Transforms.LocalTransform> transforms = _targetQuery.ToComponentDataArray<Unity.Transforms.LocalTransform>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                float3 position = transforms[i].Position;

                Color healthColor = Color.green;
                if (_showTargetHealth && _entityManager.HasComponent<Health>(entities[i]))
                {
                    Health health = _entityManager.GetComponentData<Health>(entities[i]);
                    float healthPercent = health.Current / health.Maximum;
                    healthColor = Color.Lerp(Color.red, Color.green, healthPercent);

                    Gizmos.color = healthColor;
                    Gizmos.DrawWireSphere(position, _targetCollisionRadius);
                    Gizmos.color = new Color(healthColor.r, healthColor.g, healthColor.b, 0.2f);
                    Gizmos.DrawSphere(position, _targetCollisionRadius);

                    DrawTargetHealthBar(position, health);
                }
                else
                {
                    Gizmos.color = healthColor;
                    Gizmos.DrawWireSphere(position, _targetCollisionRadius);
                    Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
                    Gizmos.DrawSphere(position, _targetCollisionRadius);
                }
            }
        }

        private void DrawTargetHealthBar(float3 position, Health health)
        {
            float healthPercent = math.clamp(health.Current / health.Maximum, 0f, 1f);
            float3 barPosition = position + new float3(0f, _targetCollisionRadius + 0.3f, 0f);
            float barWidth = _targetCollisionRadius * 2f;

            float3 barStart = barPosition - new float3(barWidth * 0.5f, 0f, 0f);
            float3 barEnd = barStart + new float3(barWidth, 0f, 0f);
            float3 healthEnd = barStart + new float3(barWidth * healthPercent, 0f, 0f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(barStart, barEnd);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(barStart, healthEnd);
        }

        private void DrawInstantSpellPreview()
        {
            if (_generatorType == GeneratorType.Projectile)
            {
                return;
            }

            float3 cameraPosition = transform.position;
            var cameraForward = new float3(transform.forward.x, transform.forward.y, transform.forward.z);
            float3 previewPosition = cameraPosition + (cameraForward * 2f);

            switch (_generatorType)
            {
                case GeneratorType.Cone:
                    DrawConeGizmo(previewPosition, cameraForward);
                    break;

                case GeneratorType.AreaOfEffect:
                    DrawAoeGizmo(previewPosition);
                    break;
            }
        }

        private void DrawAoeGizmo(float3 position)
        {
            Gizmos.color = _collisionRadiusColor;
            Gizmos.DrawWireSphere(position, _aoeRadius);
            Gizmos.color = new Color(_collisionRadiusColor.r, _collisionRadiusColor.g, _collisionRadiusColor.b, _collisionRadiusColor.a * 0.3f);
            Gizmos.DrawSphere(position, _aoeRadius);
        }

        private void DrawConeGizmo(float3 position, float3 direction)
        {
            float3 coneForward = math.normalize(direction);
            float3 right = CalculateConeRight(coneForward);
            float3 up = math.normalize(math.cross(coneForward, right));

            float halfAngleRad = math.radians(_coneAngle * 0.5f);
            float3 origin = position;
            float3 baseCenter = origin + (coneForward * _coneRadius);
            float baseRadius = _coneRadius * math.tan(halfAngleRad);

            Gizmos.color = _collisionRadiusColor;

            float angleStep = 360f / CONE_SEGMENTS;

            for (int i = 0; i <= CONE_SEGMENTS; i++)
            {
                float currentAngle = angleStep * i;
                float angleRad = math.radians(currentAngle);
                float3 basePoint = baseCenter + (((right * math.cos(angleRad)) + (up * math.sin(angleRad))) * baseRadius);

                Gizmos.DrawLine(new Vector3(origin.x, origin.y, origin.z), new Vector3(basePoint.x, basePoint.y, basePoint.z));

                if (i > 0)
                {
                    float3 prevBasePoint = CalculatePreviousConePoint(baseCenter, right, up, baseRadius, angleStep, i);
                    Gizmos.DrawLine(new Vector3(prevBasePoint.x, prevBasePoint.y, prevBasePoint.z), new Vector3(basePoint.x, basePoint.y, basePoint.z));
                }
            }
        }

        private static float3 CalculateConeRight(float3 forward)
        {
            return math.abs(math.dot(forward, math.up())) < 0.999f ? math.normalize(math.cross(math.up(), forward)) : math.normalize(math.cross(math.right(), forward));
        }

        private static float3 CalculatePreviousConePoint(float3 baseCenter, float3 right, float3 up, float baseRadius, float angleStep, int currentIndex)
        {
            float prevAngle = angleStep * (currentIndex - 1);
            float prevAngleRad = math.radians(prevAngle);
            return baseCenter + (((right * math.cos(prevAngleRad)) + (up * math.sin(prevAngleRad))) * baseRadius);
        }

        private void DrawCollisionHits()
        {
            if (!_showCollisionHits)
            {
                return;
            }

            Gizmos.color = _collisionEventColor;
            foreach (CollisionHitData collisionEvent in _collisionEvents)
            {
                float alpha = 1f - ((Time.time - collisionEvent.Time) / _collisionEventDuration);
                Gizmos.color = new Color(_collisionEventColor.r, _collisionEventColor.g, _collisionEventColor.b, alpha);
                Gizmos.DrawWireSphere(collisionEvent.Position, COLLISION_EVENT_SPHERE_RADIUS);
                Gizmos.DrawSphere(collisionEvent.Position, COLLISION_EVENT_CENTER_RADIUS);
            }
        }

        private void DrawSpawnPoint(float3 position)
        {
            if (_showSpawnPoints)
            {
                Gizmos.color = _spawnPointColor;
                Gizmos.DrawWireSphere(position, _sphereRadius);
            }
        }

        private void DrawVelocityVector(float3 position, float3 velocity)
        {
            if (!_showVelocityVectors || math.lengthsq(velocity) < 0.001f)
            {
                return;
            }

            Gizmos.color = _velocityColor;
            float3 normalized = math.normalize(velocity);
            float3 endPoint = position + (normalized * _vectorScale);

            Gizmos.DrawLine(position, endPoint);
            DrawArrowHead(position, endPoint);
        }

        private void DrawLifetimeIndicator(float3 position, Lifetime lifetime)
        {
            if (!_showLifetimeIndicators)
            {
                return;
            }

            double elapsedTime = World.DefaultGameObjectInjectionWorld.Time.ElapsedTime;
            float remainingTime = lifetime.Duration - (float)(elapsedTime - lifetime.SpawnTime);
            float normalizedLifetime = math.clamp(remainingTime / lifetime.Duration, 0f, 1f);

            Gizmos.color = Color.Lerp(_lifetimeExpiredColor, _lifetimeFullColor, normalizedLifetime);
            Gizmos.DrawWireSphere(position, _sphereRadius * 0.5f);

            if (_showLifetimeText && Camera.current != null)
            {
                DrawLifetimeLabel(position, remainingTime);
            }
        }

        private void DrawLifetimeLabel(float3 position, float remainingTime)
        {
#if UNITY_EDITOR
            Vector3 screenPos = Camera.current.WorldToScreenPoint(position);
            if (screenPos.z <= 0)
            {
                return;
            }

            var guiPos = new Vector3(screenPos.x, Screen.height - screenPos.y, 0);
            var style = new GUIStyle
            {
                normal = { textColor = remainingTime > 1f ? _lifetimeFullColor : _lifetimeExpiredColor },
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter,
            };

            string label = remainingTime > 0 ? $"{remainingTime:F1}s" : "EXPIRED";
            Vector2 size = style.CalcSize(new GUIContent(label));
            var rect = new Rect(guiPos.x - (size.x / 2), guiPos.y - (size.y / 2), size.x, size.y);

            UnityEditor.Handles.BeginGUI();
            GUI.Label(rect, label, style);
            UnityEditor.Handles.EndGUI();
#endif
        }

        private void DrawCollisionRadius(float3 position)
        {
            if (!_showCollisionRadii)
            {
                return;
            }

            Gizmos.color = _collisionRadiusColor;
            Gizmos.DrawWireSphere(position, _collisionRadius);
            Gizmos.color = new Color(_collisionRadiusColor.r, _collisionRadiusColor.g, _collisionRadiusColor.b, _collisionRadiusColor.a * 0.3f);
            Gizmos.DrawSphere(position, _collisionRadius);
        }

        private static void DrawArrowHead(float3 start, float3 end)
        {
            float3 direction = math.normalize(end - start);
            float3 right = CalculateArrowRight(direction);

            float3 arrowTip1 = end - (direction * ARROW_SIZE) + (right * ARROW_SIZE * 0.5f);
            float3 arrowTip2 = end - (direction * ARROW_SIZE) - (right * ARROW_SIZE * 0.5f);

            Gizmos.DrawLine(end, arrowTip1);
            Gizmos.DrawLine(end, arrowTip2);
        }

        private static float3 CalculateArrowRight(float3 direction)
        {
            float3 right = math.cross(direction, math.up());

            if (math.lengthsq(right) < 0.001f)
            {
                right = math.cross(direction, math.forward());
            }

            return right;
        }

        private void DrawLifecycleStatistics()
        {
            int activeCount = _spellQuery.CalculateEntityCount();
            int targetCount = _targetQuery != null ? _targetQuery.CalculateEntityCount() : 0;

            var boxStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, padding = new RectOffset(10, 10, 10, 10) };
            var labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, normal = { textColor = Color.white } };

            string stats = BuildStatisticsText(activeCount, targetCount);
            var content = new GUIContent(stats);
            Vector2 size = labelStyle.CalcSize(content);
            var boxRect = new Rect(_statsPosition.x, _statsPosition.y, size.x + 20, size.y + 20);

            GUI.Box(boxRect, "", boxStyle);
            GUI.Label(new Rect(_statsPosition.x + 10, _statsPosition.y + 10, size.x, size.y), stats, labelStyle);
        }

        private string BuildStatisticsText(int activeCount, int targetCount)
        {
            string payloadInfo = _enablePayloads
                ? $"\n<b>Payload Configuration</b>\n"
                    + $"Type: {_payloadType}\n"
                    + $"Element: {_elementalType}\n"
                    + $"Amount: {_payloadAmount:F1}\n"
                    + $"Duration: {_payloadDuration:F1}s\n"
                    + (IsStatusEffectPayload() ? $"Effect: {_payloadType}\n" : "")
                : "";

            return $"<b>Lifecycle Statistics</b>\n"
                + $"Active Spells: {activeCount}\n"
                + $"Total Spawned: {_totalSpawned}\n"
                + $"Total Destroyed: {_totalDestroyed}\n"
                + $"Lifetime: {_spellLifetime:F1}s\n"
                + $"Speed: {_spellSpeed:F1} u/s\n"
                + $"\n<b>Collision Statistics</b>\n"
                + $"Total Collisions: {_totalCollisions}\n"
                + $"Active Events: {_collisionEvents.Count}\n"
                + $"Targets: {targetCount}\n"
                + $"Type: {_generatorType}\n"
                + payloadInfo
                + $"\n<b>Controls</b>\n"
                + $"Spawn Spell: {_spawnKey}\n"
                + $"Spawn Targets: {_spawnTargetKey}\n"
                + $"Select Type: 1-Projectile | 2-Cone | 3-AoE\n"
                + $"Auto Spawn: {(_autoSpawn ? "ON" : "OFF")}";
        }

        private void CleanupEntities()
        {
            if (_entityManager == default || World.DefaultGameObjectInjectionWorld == null)
            {
                return;
            }

            if (_entityManager.Exists(_testPrefab))
            {
                _entityManager.DestroyEntity(_testPrefab);
            }

            if (_entityManager.Exists(_caster))
            {
                _entityManager.DestroyEntity(_caster);
            }

            if (!_spellQuery.IsEmpty)
            {
                _entityManager.DestroyEntity(_spellQuery);
            }

            if (!_targetQuery.IsEmpty)
            {
                _entityManager.DestroyEntity(_targetQuery);
            }

            LogInfo($"Destroyed | Total spawned: {_totalSpawned} | Total destroyed: {_totalDestroyed}");
        }

        private void DisposeBlobs()
        {
            if (_projectileColliderBlob.IsCreated)
            {
                _projectileColliderBlob.Dispose();
            }

            if (_targetColliderBlob.IsCreated)
            {
                _targetColliderBlob.Dispose();
            }
        }

        private void LogInfo(string message)
        {
            Debug.Log($"[SpellTester] {message}");
        }

        private void LogVerbose(string message)
        {
            if (_verboseLogging)
            {
                Debug.Log($"[SpellTester] [VERBOSE] {message}");
            }
        }
    }
}
