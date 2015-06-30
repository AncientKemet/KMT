using System;
using Code.Libaries.Generic;
using UnityEngine;

namespace Client.Enviroment
{
    [ExecuteInEditMode]
    public class Weather : MonoSingleton<Weather>
    {
        private float _time = 0;
        [SerializeField]
        private Light _topLight, _bottomLight;

        public float Time = 0;
        public float ratio = 0;

        public Gradient Gradient;

        private void Update()
        {
            if (Time > 24)
            {
                Time -= 24;
            }
            if (Time < 0)
            {
                Time += 24;
            }
            if (_topLight != null && _bottomLight != null)
            {
                if (Math.Abs(Time - _time) > 0.1f)
                {
                    _time = Time;

                    float dayNightRatio = Mathf.Abs((_time - 24f)/24f);
                    ratio = dayNightRatio;

                    Vector3 angle = _topLight.transform.eulerAngles;
                    angle.x = 180*dayNightRatio;
                    angle.y = 0;
                    angle.z = 0;
                    _topLight.transform.eulerAngles = angle;

                    _topLight.color = Gradient.Evaluate(dayNightRatio);
                    _bottomLight.color = Gradient.Evaluate(dayNightRatio)*0.5f;

                    RenderSettings.fogColor = Gradient.Evaluate(dayNightRatio)*1.25f;
                    RenderSettings.ambientLight = Gradient.Evaluate(dayNightRatio)*0.75f;
                    RenderSettings.fogEndDistance = 30 + dayNightRatio*100;
                    RenderSettings.ambientIntensity = dayNightRatio/2f + 0.5f;
                }
            }
        }
    }
}
