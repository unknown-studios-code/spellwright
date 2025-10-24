using Unity.Entities;

namespace Spellwright.Components
{
    /// <summary>
    /// Identifies the entity that owns or cast the spell.
    /// Used for tracking spell ownership, damage attribution, and friendly-fire logic.
    /// </summary>
    /// <remarks>
    /// Architecture Layer: Common (Used by all spell types)
    /// Read by: Collision systems, Damage systems, AI systems
    /// Performance: Blittable type, Burst-compatible
    /// Thread Safety: Read-only after spell creation
    /// </remarks>
    public struct SpellOwner : IComponentData
    {
        /// <summary>
        /// The entity that cast or owns this spell.
        /// </summary>
        /// <value>
        /// Entity reference to the owner (player, NPC, or other entity).
        /// Set to Entity.Null for spells without an owner.
        /// Used for damage attribution and team/faction checks.
        /// </value>
        public Entity OwnerEntity;
    }
}
