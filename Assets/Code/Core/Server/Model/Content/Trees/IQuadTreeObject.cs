#if SERVER
using UnityEngine;

namespace Code.Libaries.Generic.Trees
{
    public interface IQuadTreeObject
    {
        QuadTree CurrentBranch { get; set; }

        Vector2 GetPosition();
        Vector2 PositionChange();
        void ObjectBecameVisible(IQuadTreeObject o);

        bool IsStatic();
    }
}
#endif
