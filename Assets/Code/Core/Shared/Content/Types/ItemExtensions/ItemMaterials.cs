#if UNITY_EDITOR
#endif
using Shared.Content.Types;
using UnityEngine;

namespace Code.Core.Shared.Content.Types.ItemExtensions
{
    public class ItemMaterials : ItemExtension
    {
        private MeshRenderer renderer;

        private void Start()
        {
            if (transform.childCount == 0)
            {
                Debug.LogError("Item has no children model to take materials from.");
                return;
            }
            renderer = GetComponentInChildren<MeshRenderer>();
        }

        public Material this[int index]
        {
            get { return GetMaterial(index); }
            set { SetMaterial(index, value); }
        }

        private void SetMaterial(int index, Material value)
        {
            renderer.materials[index] = value;
        }

        private Material GetMaterial(int index)
        {
            return renderer.materials[index];
        }
    }
}
