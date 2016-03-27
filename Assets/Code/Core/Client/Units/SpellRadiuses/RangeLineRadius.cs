using Client.Enviroment;
using UnityEngine;

namespace Client.Units.SpellRadiuses
{
    [ExecuteInEditMode]
    public class RangeLineRadius : ASpellRadius
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
            set { _range = Mathf.Clamp(value, 0, 100); }
        }

        public override float CriticalArea
        {
            get { return _criticalArea; }
            set { _criticalArea = Mathf.Clamp01(value); }
        }

        private void Update()
        {
            if (Application.isPlaying)
                if (KemetMap.Instance != null)
                {
                    transform.LookAt(KemetMap.Instance.MouseAt.point);
                    var currentEulers = transform.localEulerAngles;
                    currentEulers.x = 0;
                    currentEulers.z = 0;
                    transform.localEulerAngles = currentEulers;
                }
            if (!Application.isPlaying)
            {
                Strenght = 1f;
            }
            //range
            var normalRange = (_range * (0.5f + Strenght / 2f) - (_range * _criticalArea));
            var critRange = ((_range * (0.5f + Strenght / 2f)) * _criticalArea);
            Normal.orthographicSize = normalRange * OneSize;
            Crit.orthographicSize = critRange * OneSize;
            //width
            Normal.aspectRatio = Width / normalRange;
            Crit.aspectRatio = Width / critRange;

            Normal.transform.localPosition = new Vector3(0, 0, Strenght);
            Crit.transform.localPosition = new Vector3(0, 0, normalRange + Strenght);
        }
    }
}
