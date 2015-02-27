using UnityEngine;

namespace Libaries.TK2DROOT.tk2d.Code.Sprites
{
    [ExecuteInEditMode]
    public class tk2dForceBuild : MonoBehaviour {

        void Update () {
	        GetComponent<tk2dBaseSprite>().ForceBuild();
        }
    }
}
