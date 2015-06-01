using UnityEngine;

namespace Client.Units
{
    public abstract class ASpellRadius : MonoBehaviour
    {
        private PlayerUnit _unit;

        public PlayerUnit Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                transform.parent = value.transform;
                transform.localEulerAngles = Vector3.zero;
                transform.localPosition = new Vector3(0, 2, 0);
            }
        }

        public abstract float Strenght { get; set; }
        public abstract float Range { get; set; }
        public abstract float CriticalChance { get; set; }

    }
}
