#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace Unity.Physics.Authoring
{
    [InitializeOnLoad]
    internal class EditorInitialization
    {
        private static readonly string k_CustomDefine = "UNITY_PHYSICS_CUSTOM";

        static EditorInitialization()
        {
            var fromBuildTargetGroup = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string definesStr = PlayerSettings.GetScriptingDefineSymbols(fromBuildTargetGroup);
            List<string> defines = definesStr.Split(';').ToList();
            string found = defines.Find(define => define.Equals(k_CustomDefine));
            if (found == null)
            {
                defines.Add(k_CustomDefine);
                PlayerSettings.SetScriptingDefineSymbols(fromBuildTargetGroup, string.Join(";", defines.ToArray()));
            }
        }
    }
}
#endif
