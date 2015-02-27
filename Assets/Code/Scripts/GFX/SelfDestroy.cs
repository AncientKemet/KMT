using System.Collections;
using UnityEngine;

namespace Code.Scripts.GFX
{
    public class SelfDestroy : MonoBehaviour
    {

        public float Time = 1f;

        void Start ()
        {
            StartCoroutine(DestroyAfter(Time));
        }

        private IEnumerator DestroyAfter(float t)
        {
            yield return new WaitForSeconds(t);
            if(this != null)
            if(gameObject != null)
                Destroy(gameObject);
        }
    }
}
