using UnityEngine;

namespace Client.UI.Scripts
{
    public class MyCameraAnchor : MonoBehaviour
    {

        public tk2dBaseSprite.Anchor Anchor;

        void Start ()
        {
            var cam = tk2dUIManager.Instance.GetComponent<Camera>();
            transform.position = cam.transform.FindChild("Anchor (" + Anchor + ")").position;
        }
	
    }
}
