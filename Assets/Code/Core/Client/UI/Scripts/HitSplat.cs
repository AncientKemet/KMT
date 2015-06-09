using System.Collections;
using System.Collections.Generic;
using Client.Units;
using Code.Core.Client.Units.Managed;
using Code.Libaries.Generic.Managers;
using Libaries.Net.Packets.ForClient;
using Libaries.UnityExtensions.Independent;
using Shared.Content.Types;
using UnityEngine;

namespace Client.UI.Scripts
{
    [RequireComponent(typeof(tk2dTextMesh))]
    public class HitSplat : MonoBehaviour {

        private static List<HitSplat> HitSplats = new List<HitSplat>();

        public static HitSplat Show(PlayerUnit unit, int damage,Spell.DamageType dmgType, Spell.HitType hitType)
        {
            float factor = 0.85f + damage/50f;

            var splash = HitSplats.Find(splat => !splat.Active);
            foreach (var splat in HitSplats)
            {
                if (splat.Unit == unit)
                {
                    splat._bonusYCoord += factor;
                }
            }

            if (splash == null)
                splash = Instantiate((GameObject) Resources.Load("Hitsplats/Hitsplat")).GetComponent<HitSplat>();

            splash.Sprite.SetSprite(damage > 9 ? "Long" : "Short");
            splash.Sprite.color = UIContentManager.I.DamageColors.Find(color => color.Type == dmgType).Color;
            splash.TextMesh.color = UIContentManager.I.HitColors.Find(color => color.Type == hitType).Color;

            splash.TextMesh.text = "" + damage;
            splash.Unit = unit;
            splash._bonusYCoord = 0;
            splash.gameObject.SetActive(true);
            splash.transform.localScale = Vector3.zero;
            
            splash.StartCoroutine(splash.Animation(factor));
            splash.StartCoroutine(Ease.Vector(Vector3.zero, Vector3.one*factor,
                                              vector3 => splash.transform.localScale = vector3));
            
            return splash;
        }
        
        public static HitSplat Show(DamagePacket packet)
        {
            var unit = UnitManager.Instance[packet.UnitId];
            if (unit != null)
            {
                return Show(unit, packet.Damage, packet.DamageType, packet.HitType);
            }
            return null;
        }

        public PlayerUnit Unit { get; set; }
        public tk2dSprite Sprite;
        public bool Active { get { return gameObject.activeSelf; } }

        private tk2dTextMesh TextMesh;
        private float _bonusYCoord;

        void Awake()
        {
            HitSplats.Add(this);
            TextMesh = GetComponent<tk2dTextMesh>();
        }

        void Update()
        {
            Vector3 screenPos = UnscaledCamera.Instance.GetComponent<Camera>().ScreenToWorldPoint(Camera.main.WorldToScreenPoint(Unit.transform.position + Vector3.up));
            transform.position = screenPos + new Vector3(0, _bonusYCoord * 20);
        }

        private IEnumerator Animation(float factor)
        {
            yield return Ease.Vector(Vector3.zero, Vector3.one*factor, vector3 => transform.localScale = vector3);
            yield return new WaitForSeconds(1+factor);
            yield return Ease.Vector(transform.localScale, Vector3.zero, vector3 => transform.localScale = vector3);
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);
            Unit = null;
        }

        
    }
}
