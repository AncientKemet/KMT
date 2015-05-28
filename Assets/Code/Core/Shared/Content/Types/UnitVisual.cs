
#if UNITY_EDITOR
#endif

using UnityEngine;

namespace Code.Core.Shared.Content.Types
{
    public class UnitVisual : MonoBehaviour
    {
        [SerializeField] private Animation animation;

        public Animation CreateVisual()
        {
            GameObject o = ((GameObject) Instantiate(animation.gameObject));
            return o.GetComponent<Animation>();
        }
    }
}

