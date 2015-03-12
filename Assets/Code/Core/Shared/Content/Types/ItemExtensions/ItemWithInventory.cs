#if UNITY_EDITOR
#endif
using Shared.Content.Types;

namespace Code.Core.Shared.Content.Types.ItemExtensions
{
    public class ItemWithInventory : ItemExtension
    {
        public int Width = 1, Height = 1;
        public bool HasAnimation;
    }
}
