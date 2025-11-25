using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Physics.Authoring
{
    [CreateAssetMenu(menuName = "Unity Physics/Custom Physics Material Tag Names", fileName = "Custom Material Tag Names", order = 506)]
    public sealed partial class CustomPhysicsMaterialTagNames : ScriptableObject, ITagNames
    {
        private CustomPhysicsMaterialTagNames() { }

        public IReadOnlyList<string> TagNames => m_TagNames;

        [SerializeField]
        [FormerlySerializedAs("m_FlagNames")]
        private string[] m_TagNames = Enumerable.Range(0, 8).Select(i => string.Empty).ToArray();
    }
}
