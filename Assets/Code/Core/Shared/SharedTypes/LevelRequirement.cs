using System;
using Shared.StructClasses;
using UnityEngine;

namespace Shared.SharedTypes
{
    [Serializable]
    public class LevelRequirement
    {
        public Levels.Skills Skill;
        [Range(0,99)]
        public byte Val = 1;
    }
}
