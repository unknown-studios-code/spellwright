using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Spellwright.Components.Common;

namespace Spellwright.Jobs.Movement
{
    [BurstCompile]
    public partial struct ApplyVelocityJob : IJobEntity
    {
        public float DeltaTime;

        [BurstCompile]
        public void Execute(ref LocalTransform transform, in Velocity velocity)
        {
            transform.Position += velocity.Value * DeltaTime;
        }
    }
}

