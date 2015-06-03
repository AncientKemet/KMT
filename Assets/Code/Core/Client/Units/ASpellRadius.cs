using UnityEngine;

namespace Client.Units
{
    public abstract class ASpellRadius : MonoBehaviour
    {
        public abstract float Strenght { get; set; }
        public abstract float Range { get; set; }
        public abstract float CriticalArea { get; set; }

    }
}
