using System;
using Shared.StructClasses;
using UnityEngine;

namespace Shared.SharedTypes
{
    [Serializable]
    public class ExperienceReward
    {
        public Levels.Skills Skill;
        public float Val = 1;

    }
}
