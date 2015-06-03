using UnityEngine;

namespace Client.Units.SpellRadiuses
{
    [ExecuteInEditMode]
    public class MeleeLineRadius : ASpellRadius
    {
        [SerializeField]
        private float _range = 1f;
        public float Width = 1f;

        [SerializeField]
        private float _criticalArea;


        [SerializeField]
        private Projector Normal, Crit;
        private const float OneSize = 1.0375f, OneRatio = 1.93f;

        public override float Strenght { get; set; }

        public override float Range
        {
            get { return _range; }
            set { _range = value; }
        }

        public override float CriticalArea
        {
            get { return _criticalArea; }
            set { _criticalArea = value; }
        }

        private void Update()
        {
            //width
            Normal.aspectRatio = OneRatio * (Width) / (_range - (_range * _criticalArea)) * OneSize;
            Crit.aspectRatio = OneRatio * (Width) / (_range * _criticalArea) * OneSize;
            //range
            Normal.orthographicSize = (_range - (_range * _criticalArea)) * OneSize;
            Crit.orthographicSize = (_range * _criticalArea) * OneSize;
            Crit.transform.localPosition = new Vector3(0, 0, _range - (_range * _criticalArea));
        }
    }
}
