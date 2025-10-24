using Unity.Entities;
using Unity.Mathematics;

namespace Spellwright.Components
{
    /// <summary>
    /// Defines the movement velocity of a spell entity in world space.
    /// Updated by movement systems and used to calculate position changes per frame.
    /// </summary>
    /// <remarks>
    /// Architecture Layer: Common (Used by projectile and moving spell types)
    /// Read by: Movement systems, Physics systems
    /// Written by: Homing systems, Acceleration systems, Movement initialization
    /// Performance: Blittable type, Burst-compatible, cache-friendly
    /// Thread Safety: Read/Write in parallel jobs (no shared writes)
    /// </remarks>
    public struct Velocity : IComponentData
    {
        /// <summary>
        /// The velocity vector in world space units per second.
        /// </summary>
        /// <value>
        /// Velocity as a 3D vector (x, y, z) in units per second.
        /// Magnitude represents speed, direction represents movement heading.
        /// Typical range: magnitude 0.0f to 100.0f units/second.
        /// Use <c>math.length(Value)</c> to get current speed.
        /// </value>
        public float3 Value;
    }
}
