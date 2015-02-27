using UnityEngine;

namespace Code.Scripts
{
    public class Fire : MonoBehaviour
    {
        public ParticleSystem ParticleSystem;
        public Light Light;

        public float MinIntensity = 0.1f;
        public float MaxIntensity = 0.2f;
    
        void FixedUpdate ()
        {
            Light.intensity = Random.Range(MinIntensity, MaxIntensity);
        }
    }
}
