
#if UNITY_EDITOR
#endif
using Code.Core.Shared.Content;
using Shared.Content.Types.NpcExtensions;
using UnityEngine;

namespace Shared.Content.Types
{
    [ExecuteInEditMode]
    public class NpcPrefab : ContentItem
    {
        /*
         * Data
         */
        public int NPC_ID = -1;

        /*
         * Visuals
         */
        public int ModelID = 1;
        public float Scale = 1f;

        /*
         * Settings
         */
        public bool _enableMovement = false;
        public bool _enableCollisions = false;

#region MOVEMENT

        public float _walkDistance = 5f;
        public float _walkFrequency = 5;

#endregion

        public NpcExtension[] GetExtensions()
        {
            return GetComponents<NpcExtension>();
        }

    }
}

