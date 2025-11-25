using UnityEngine;

namespace Unity.Physics.Authoring
{
    internal sealed class EnumFlagsAttribute : PropertyAttribute { }

    internal sealed class ExpandChildrenAttribute : PropertyAttribute { }

    internal sealed class SoftRangeAttribute : PropertyAttribute
    {
        public readonly float SliderMin;
        public readonly float SliderMax;
        public float TextFieldMin { get; set; }
        public float TextFieldMax { get; set; }

        public SoftRangeAttribute(float min, float max)
        {
            SliderMin = TextFieldMin = min;
            SliderMax = TextFieldMax = max;
        }
    }
}
