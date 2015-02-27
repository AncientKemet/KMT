using UnityEngine;

namespace Code.Libaries.Building
{
    public class BuildingNode : MonoBehaviour {


        void DrawGizmos()
        {
            Gizmos.DrawCube(transform.position, Vector3.one / 7f);
        }
    }
}
