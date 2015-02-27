using System.Collections.Generic;
using Code.Core.Client.UI.Controls;
using UnityEngine;

namespace Code.Core.Shared.Content.Types
{
    [RequireComponent(typeof(Clickable))]
    public class ColorChanger : MonoBehaviour
    {
        public Color ChangeToColor = Color.green;

        private Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();

        protected virtual void OnMouseEnter()
        {
            if (renderer != null)
            {
                foreach (Material material in renderer.materials)
                {
                    originalColors[material] = material.color;
                    material.color = ChangeToColor*2;
                }
            }
        }

        protected virtual void OnMouseExit()
        {
            if (renderer != null)
            {
                foreach (Material material in renderer.materials)
                {
                    material.color = originalColors[material];
                }
            }
        }
    }
}
