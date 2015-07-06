using Client.Enviroment;
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
            set { _range = Mathf.Clamp(value,0,100); }
        }

        public override float CriticalArea
        {
            get { return _criticalArea; }
            set { _criticalArea = Mathf.Clamp01(value); }
        }

        private void Update()
        {
            if (KemetMap.Instance != null)
            {
                transform.LookAt(KemetMap.Instance.MouseAt.point);
                var currentEulers = transform.localEulerAngles;
                currentEulers.x = 0;
                currentEulers.z = 0;
                transform.localEulerAngles = currentEulers;
            }

            //width
            Normal.aspectRatio = OneRatio * (Width) / (_range - (_range * _criticalArea)) * OneSize;
            Crit.aspectRatio = OneRatio * (Width) / (_range * _criticalArea) * OneSize;
            //range
            Normal.orthographicSize = (_range - (_range * _criticalArea)) * OneSize;
            Crit.orthographicSize = (_range * _criticalArea) * OneSize;

            Normal.transform.localPosition = new Vector3(0, 0, Strenght * 2);
            Crit.transform.localPosition = new Vector3(0, 0, _range - (_range * _criticalArea) + Strenght * 2);
        }
    }
}
