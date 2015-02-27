using UnityEngine;

namespace Code.Core.Client.UI.Scripts
{
    [ExecuteInEditMode]
    public class KeepRelativePosition : MonoBehaviour
    {

        [SerializeField]
        private tk2dSlicedSprite parent;
        [SerializeField]
        private float configX = 0;
        [SerializeField]
        private float configY = 0;

        void Start()
        {

        }
    
        void Update()
        {
#if UNITY_EDITOR
            if(parent == null){
                parent = FindParent<tk2dSlicedSprite>();
                configX = Mathf.Abs((parent.dimensions.x + parent.borderLeft + parent.borderRight) / (parent.transform.position.x - transform.position.x));
                configY = Mathf.Abs((parent.dimensions.y + parent.borderTop + parent.borderBottom) / (parent.transform.position.y - transform.position.y));
            }
#endif

            if(parent != null)
            {
                if(Application.isPlaying)
                {
                    transform.position = new Vector3(parent.transform.position.x + (parent.dimensions.x+ parent.borderLeft + parent.borderRight) / configX, parent.transform.position.y + (parent.dimensions.y+ parent.borderTop + parent.borderBottom) / configY, transform.position.z);
                }
            }
        }

        private T FindParent<T>() where T : Component
        {
            Transform p = transform.parent;
            for (int i = 0; i < 10; i++)
            {
                if(p == null){
                    return null;
                }
                if (p.GetComponent<T>() == null)
                {
                    p = p.parent;
                }
            }
            return p.GetComponent<T>();
        }
    }
}
