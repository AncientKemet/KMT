using UnityEngine;

namespace Client.Units
{
    public class Face : MonoBehaviour
    {
        public MeshRenderer FaceRenderer;

        [SerializeField] private MeshRenderer Ears;

        private Material _earMaterial;

        public Material EarMaterial
        {
            get { return _earMaterial; }
            set
            {
                _earMaterial = value;
                Ears.material = value;
            }
        }
    }
}
