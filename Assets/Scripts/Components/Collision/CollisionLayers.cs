using Unity.Physics;

namespace Spellwright.Components.Collision
{
    public static class CollisionLayers
    {
        public const uint NOTHING = 0u;
        public const uint EVERYTHING = ~0u;
        public const uint PROJECTILE = 1u << 0;
        public const uint ENEMY = 1u << 1;
        public const uint ENVIRONMENT = 1u << 2;
        public const uint PLAYER = 1u << 3;

        public static CollisionFilter CreateProjectileFilter()
        {
            return new CollisionFilter
            {
                BelongsTo = PROJECTILE,
                CollidesWith = ENEMY | ENVIRONMENT,
                GroupIndex = 0,
            };
        }

        public static CollisionFilter CreateEnemyFilter()
        {
            return new CollisionFilter
            {
                BelongsTo = ENEMY,
                CollidesWith = PROJECTILE | ENVIRONMENT,
                GroupIndex = 0,
            };
        }

        public static CollisionFilter CreateEnvironmentFilter()
        {
            return new CollisionFilter
            {
                BelongsTo = ENVIRONMENT,
                CollidesWith = EVERYTHING,
                GroupIndex = 0,
            };
        }
    }
}
