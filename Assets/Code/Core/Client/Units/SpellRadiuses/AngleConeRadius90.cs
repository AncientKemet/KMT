using UnityEngine;

namespace Client.Units.SpellRadiuses
{
    public class AngleConeRadius90 : ASpellRadius
    {

        [SerializeField] private Projector NormalRange, CriticalStart;
        private float _range;
        private float _criticalArea;

        public override float Strenght { get; set; }

        public override float Range
        {
            get { return _range; }
            set
            {
                _range = value;
                NormalRange.orthographicSize = 2 + Range;
                CriticalStart.orthographicSize = 2 + Range - 0.2f * CriticalArea;
            }
        }

        public override float CriticalArea
        {
            get { return _criticalArea; }
            set
            {
                _criticalArea = value;
                CriticalStart.orthographicSize = 2 + Range - 0.2f * CriticalArea;
            }
        }
    }
}
