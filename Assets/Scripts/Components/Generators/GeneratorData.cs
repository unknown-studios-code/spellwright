using Unity.Entities;

namespace Spellwright.Components.Generators
{
    /// <summary>
    /// Stores configuration parameters that define how a spell generator behaves.
    /// Different generator types use different subsets of these fields based on their delivery mechanism.
    /// </summary>
    /// <remarks>
    /// Architecture Layer: Generator (Mandatory - exactly one generator per spell)
    /// Field Usage by Generator Type:
    /// - Projectile: Speed, Lifetime
    /// - Cone: Radius, Angle, Lifetime
    /// - AreaOfEffect: Radius, Lifetime
    /// - SelfCast: Lifetime (optional)
    /// Systems: Read by all generator systems (ProjectileGeneratorSystem, ConeGeneratorSystem, etc.)
    /// Written by: SpellBakingSystem during JSON-to-Entity conversion
    /// Performance: Blittable component (16 bytes), Burst-compatible, cache-friendly
    /// Memory Layout: 4 float fields = 16 bytes total per entity
    /// JSON Mapping: Maps to <c>generator.*</c> fields in spell JSON schema
    /// Design Decision: Single struct for all generator types reduces code duplication,
    /// unused fields have negligible memory cost compared to benefits of unified interface
    /// </remarks>
    public struct GeneratorData : IComponentData
    {
        /// <summary>
        /// Movement speed for projectile-type spells.
        /// Defines how fast the projectile travels through the world.
        /// </summary>
        /// <value>
        /// Speed in units per second.
        /// Valid Range: 0.1f to 100.0f
        /// Recommended Range: 5.0f to 30.0f for balanced gameplay
        /// Default: 10.0f units/second
        /// Generator Usage: Projectile only (ignored by Cone, AoE, SelfCast)
        /// </value>
        public float Speed;

        /// <summary>
        /// Radius for cone and area-of-effect generators.
        /// Defines the detection range for targets affected by the spell.
        /// </summary>
        /// <value>
        /// Radius in world units.
        /// Valid Range: 0.5f to 50.0f units
        /// Recommended Range: 2.0f to 15.0f for balanced gameplay
        /// Default: 5.0f units
        /// Generator Usage: Cone and AreaOfEffect (ignored by Projectile, SelfCast)
        /// </value>
        public float Radius;

        /// <summary>
        /// Cone angle for directional area spells.
        /// Defines the angular spread of the cone in degrees.
        /// </summary>
        /// <value>
        /// Angle in degrees (e.g., 60 = 60° cone, 90 = 90° cone).
        /// Valid Range: 1.0f to 180.0f degrees
        /// Recommended Range: 30.0f to 120.0f for balanced gameplay
        /// Default: 60.0f degrees
        /// Generator Usage: Cone only (ignored by Projectile, AoE, SelfCast)
        /// Note: Degrees not radians - conversion happens in ConeGeneratorSystem
        /// </value>
        public float Angle;

        /// <summary>
        /// Duration in seconds before the spell entity is automatically destroyed.
        /// Prevents spell entities from persisting indefinitely.
        /// </summary>
        /// <value>
        /// Lifetime in seconds.
        /// Valid Range: 0.1f to 60.0f seconds
        /// Recommended Range: 0.5f to 10.0f for most spells
        /// Default: 5.0f seconds
        /// Generator Usage: All generator types
        /// Note: Duplicated in <see cref="Lifetime"/> component for separation of concerns.
        /// GeneratorData is configuration (what was requested),
        /// Lifetime component is runtime state (when it will expire).
        /// Negative value: Unlimited lifetime (use with caution)
        /// </value>
        public float Lifetime;
    }
}

