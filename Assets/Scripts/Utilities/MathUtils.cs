using Unity.Mathematics;

namespace Spellwright.Utilities
{
    public static class MathUtils
    {
        public static float3 GetSafeDirection(in float3 direction)
        {
            float lengthSq = math.lengthsq(direction);
            return lengthSq < 0.0001f ? math.forward() : math.normalize(direction);
        }
    }
}
