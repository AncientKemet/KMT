using System.Collections.Generic;
using System.Linq;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Scripts;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEngine;

namespace Client.UI.Interfaces.StatsBar
{
    public class BuffControl : Clickable
    {
        public tk2dSlicedSprite BuffColor;
        public Icon Icon;
        private BuffInstance _buff;

        public BuffInstance Buff
        {
            get { return _buff; }
            set
            {
                _buff = value;
                Icon.Texture = value.Buff.Icon;
                BuffColor.color = UIContentManager.I.BuffColors.Find(color => color.Type == value.Buff._type).Color;
            }
        }

        protected override void Start()
        {
            base.Start();
            OnMouseIn += () => DescriptionInterface.I.Show(Buff.Buff.name,
                string.IsNullOrEmpty(Buff.Buff.Subtitle) ? null : Buff.Buff.Subtitle,
                Buff.Buff.GetDescribtion(),
                Buff.Buff.Icon);
            OnMouseOff += () => DescriptionInterface.I.Hide();
        }

        private void FixedUpdate()
        {
            
        }

        public static void RealignBuffs(BuffControl buffInstance, List<BuffControl> buffControls)
        {
            List<BuffControl> priorityOrder = buffControls.OrderBy(control => control.Buff.Buff.Priority).ToList();

            /*int highestPriority = priorityOrder.First().Buff.Buff.Priority;
            int lowestPriority = priorityOrder.Last().Buff.Buff.Priority;*/

            Vector3 pos = buffInstance.transform.position;

            float scale = 1f;

            foreach (var buffControl in priorityOrder)
            {
                buffControl.transform.position = pos;
                pos += new Vector3(1.25f*scale,0,0);
                buffControl.transform.localScale = Vector3.one*scale;
                scale *= 0.9f;
            }
        }
    }
}
