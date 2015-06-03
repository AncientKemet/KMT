using System;
using UnityEngine;

namespace UnityStandardAssets.Water
{
    [RequireComponent(typeof (WaterBase))]
    [ExecuteInEditMode]
    public class SpecularLighting : MonoBehaviour
    {
        private WaterBase m_WaterBase;
        public Transform specularLight;


        public void Start()
        {
            m_WaterBase = (WaterBase) gameObject.GetComponent(typeof (WaterBase));
        }


        public void Update()
        {
            if (!m_WaterBase)
            {
                m_WaterBase = (WaterBase) gameObject.GetComponent(typeof (WaterBase));
            }

            if (specularLight && m_WaterBase.sharedMaterial)
            {
                m_WaterBase.sharedMaterial.SetVector("_WorldLightDir", specularLight.transform.forward);
            }
        }
    }
}