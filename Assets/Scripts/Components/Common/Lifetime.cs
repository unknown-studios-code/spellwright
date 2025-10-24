using Unity.Entities;

namespace Spellwright.Components
{
    /// <summary>
    /// Defines the lifetime of a spell entity, tracking spawn time and duration.
    /// When the elapsed time exceeds the duration, the entity is marked for destruction.
    /// </summary>
    /// <remarks>
    /// Architecture Layer: Common (Used by all spell types)
    /// Updated by: LifetimeSystem
    /// Performance: Blittable type, Burst-compatible
    /// Thread Safety: Read-only in parallel jobs, written by LifetimeSystem
    /// </remarks>
    public struct Lifetime : IComponentData
    {
        /// <summary>
        /// The time when the entity was spawned, in seconds since application start.
        /// </summary>
        /// <value>
        /// Time in seconds from Time.ElapsedTime at spawn.
        /// Used to calculate elapsed time: (currentTime - SpawnTime).
        /// </value>
        public double SpawnTime;

        /// <summary>
        /// The total duration the entity should exist before being destroyed.
        /// </summary>
        /// <value>
        /// Duration in seconds. Valid range: 0.1f to 60.0f.
        /// Set to negative value to indicate unlimited lifetime.
        /// Recommended range: 1.0f to 10.0f for typical spells.
        /// </value>
        public float Duration;
    }
}
