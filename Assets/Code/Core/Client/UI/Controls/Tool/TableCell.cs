using UnityEngine;

namespace Client.UI.Controls.Tool
{
    [RequireComponent(typeof(tk2dSlicedSprite))]
    [ExecuteInEditMode]
    public class TableCell : MonoBehaviour
    {
        public Vector3 offset;
        public float width = 2f;
        public float height = 0.5f;

        public bool Build = false;

#if UNITY_EDITOR
        private void Update()
        {
            if (Build)
            {
                Build = false;
                var sprite = GetComponent<tk2dSlicedSprite>();
                sprite.Collection = collection;
                sprite.anchor = tk2dBaseSprite.Anchor.UpperLeft;
                sprite.dimensions = new Vector2(width * 20f, height * 20f);
            }
        }

        public tk2dSpriteCollectionData collection;
#endif
    }
}