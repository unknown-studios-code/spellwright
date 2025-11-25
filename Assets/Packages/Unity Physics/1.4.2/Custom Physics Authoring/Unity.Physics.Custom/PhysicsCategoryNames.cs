using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.Physics.Authoring
{
    [CreateAssetMenu(menuName = "Unity Physics/Physics Category Names", fileName = "Physics Category Names", order = 507)]
    public sealed class PhysicsCategoryNames : ScriptableObject, ITagNames
    {
        private PhysicsCategoryNames() { }

        IReadOnlyList<string> ITagNames.TagNames => CategoryNames;

        public IReadOnlyList<string> CategoryNames => m_CategoryNames;

        [SerializeField]
        private string[] m_CategoryNames = Enumerable.Range(0, 32).Select(i => string.Empty).ToArray();
    }
}
