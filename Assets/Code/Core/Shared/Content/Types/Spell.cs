using System.Collections.Generic;
using Code.Core.Client.UI;
using Code.Libaries.Generic;
#if SERVER
using Server.Model.Entities;
using Server.Model.Entities.Human;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using Code.Libaries.Generic.Managers;
using UnityEngine;

namespace Shared.Content.Types
{
    public enum SpellType
    {
        Other,
        Physical,
        Magical,
        Mixed,
        Heal,
        Buff,
    }

    public abstract class Spell : ScriptableObject
    {
        private int _inContentManagerId = -1;
        
        public SpellType Type;
        public Texture2D Icon;

        public bool HasEnergyCost { get { return ChargeEnergyCost > 0.01f; } }

        [Range(0,50)]
        public float ChargeEnergyCost = 0f;

        [Range(0,5f)]
        public float MaxDuration = 1f;

        [Range(0,5f)]
        public float ActivableDuration = 0.5f;

        [Multiline(5)]
        public string Description = "";

        public string Subtitle;
        
        public int InContentManagerId
        {
            get
            {
                if (_inContentManagerId == -1)
                {
                    _inContentManagerId = SIAsset<ContentManager>.I.Spells.IndexOf(this);
                }
                return _inContentManagerId;
            }
        }

#if SERVER
        [System.Obsolete("Use Unit.Spells.StartSpell")]
        public void StartCasting(ServerUnit unit)
        {
            var player = unit as Player;

            if (player != null)
            {
                player.ClientUi.ShowControl(InterfaceType.ActionBars, 4);
            }

            OnStartCasting(unit);
        }

        public void FinishCasting(ServerUnit unit, float strenght)
        {
            var player = unit as Player;

            if (player != null)
            {
                player.ClientUi.HideControl(InterfaceType.ActionBars, 4);
            }

            OnFinishCasting(unit, strenght);
        }

        public void StrenghtChanged(ServerUnit unit, float strenght)
        {
            strenght = Mathf.Clamp01(strenght);
            OnStrenghtChanged(unit, strenght);
        }

        public void ForceCancelCasting(ServerUnit unit)
        {
            unit.Spells.CancelCurrentSpell();
        }

        public abstract void OnStartCasting(ServerUnit unit);
        public abstract void OnFinishCasting(ServerUnit unit, float strenght);
        public abstract void OnStrenghtChanged(ServerUnit unit, float strenght);

        /// <summary>
        /// Do not call this by anymeans, instead use Unit.Spells.CancelSpell or
        /// </summary>
        /// <param name="unit"></param>
        [System.Obsolete("Use Unit.Spells.CancelSpell")]
        public abstract void CancelCasting(ServerUnit unit);
#endif
#if UNITY_EDITOR
        public static void CreateSpell<T>() where T : Spell
        {
            var asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, "Assets/Development/Libary/Spells/"+ typeof(T).Name + ".asset");
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
        }
#endif
        public enum HitType
        {
            Melee,
            Range,
            DamageOverTime
        }

        /// <summary>
        /// The numbers are multipliers to the pushaway and stun duration on hit.
        /// Eq weak doesnt push or stun enemy at all cause of the 0.
        /// </summary>
        public enum HitStrenght
        {
            Weak = 0,
            Normal = 1,
            Strong = 2,
            Critical = 3
        }

        public enum DamageType
        {
            Physical,
            Magical,
            True
        }
    }
}
