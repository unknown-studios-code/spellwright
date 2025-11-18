using Spellwright.Components;
using Unity.Entities;
using UnityEngine;

namespace Spellwright.Authorings
{
    public class TargetAuthoring : MonoBehaviour
    {
        [Header("Health Configuration")]
        [Tooltip("The maximum health of the target")]
        [Range(1f, 10000f)]
        public float MaxHealth = 100f;

        [Tooltip("The starting health percentage (0-1)")]
        [Range(0f, 1f)]
        public float StartingHealthPercent = 1f;

        private class TargetBaker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new Health { Maximum = authoring.MaxHealth, Current = authoring.MaxHealth * authoring.StartingHealthPercent });
            }
        }
    }
}
