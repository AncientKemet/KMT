using UnityEngine;

namespace Client.Units.SpellRadiuses
{
    [ExecuteInEditMode]
    public class AngleConeRadius90 : ASpellRadius
    {
        [SerializeField]
        private float _range = 1f;

        [SerializeField]
        private float _criticalArea;
        
        [SerializeField]
        private Projector Normal, Crit;
        private const float OneSize = 1.0375f, OneRatio = 1.93f;

        public override float Strenght { get; set; }

        public override float Range
        {
            get { return _range; }
            set { _range = Mathf.Clamp(value,0,100); }
        }

        public override float CriticalArea
        {
            get { return _criticalArea; }
            set { _criticalArea = Mathf.Clamp01(value); }
        }

        private void Update()
        {   //range
            Normal.orthographicSize = (_range - (_range * _criticalArea)) * OneSize;
            Crit.orthographicSize = (_range) * OneSize;
            Crit.transform.localPosition = new Vector3(0, 0,0);
        }
    }
}
