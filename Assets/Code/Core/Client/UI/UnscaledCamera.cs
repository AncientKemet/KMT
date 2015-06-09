using UnityEngine;

namespace Client.UI
{
    public class UnscaledCamera : MonoBehaviour {

        public static UnscaledCamera Instance { get; private set; }
        
        void Awake ()
        {
            Instance = this;
        }
	
    }
}
