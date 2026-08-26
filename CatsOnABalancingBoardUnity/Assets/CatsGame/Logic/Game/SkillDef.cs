using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using static OMC.WeightBehaviour;

namespace OMC {
    [CreateAssetMenu(menuName = "OMC/SkillDef")]
    public class SkillDef : ScriptableObject {
        public string skillName;
        public string description;
        public int maxPoints;
    }
}

