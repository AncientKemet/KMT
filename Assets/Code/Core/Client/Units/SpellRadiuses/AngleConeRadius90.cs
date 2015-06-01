using UnityEngine;

namespace Client.Units.SpellRadiuses
{
    public class AngleConeRadius90 : ASpellRadius
    {

        [SerializeField] private Projector NormalRange, CriticalStart;
        private float _range;
        private float _criticalChance;

        public override float Strenght { get; set; }

        public override float Range
        {
            get { return _range; }
            set
            {
                _range = value;
                NormalRange.orthographicSize = 2 + Range;
                CriticalStart.orthographicSize = 2 + Range - 0.2f * CriticalChance;
            }
        }

        public override float CriticalChance
        {
            get { return _criticalChance; }
            set
            {
                _criticalChance = value;
                CriticalStart.orthographicSize = 2 + Range - 0.2f * CriticalChance;
            }
        }
    }
}
