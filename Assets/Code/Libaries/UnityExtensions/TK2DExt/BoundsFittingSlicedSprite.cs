using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Libaries.UnityExtensions
{
    [ExecuteInEditMode]
    [AddComponentMenu("Kemet/UI/Bounds sliced sprite")]
    [RequireComponent(typeof(tk2dSlicedSprite))]
    public class BoundsFittingSlicedSprite : MonoBehaviour
    {
        public GameObject TargetContainer;

        private Bounds _bounds;

        public Vector3 margin;
        public float Scale = 1f;
        public tk2dSlicedSprite SlicedSprite { get; private set; }
        public bool UpdateAutomaticaly = true;

        public Bounds Bounds
        {
            get { return _bounds; }
            private set
            {

                _bounds = value;
                SlicedSprite.transform.position = _bounds.center + new Vector3(0, 0, _bounds.size.z*2 + 1);
                //SlicedSprite.transform.position = Camera.main.WorldToScreenPoint(tk2dUIManager.Instance.UICamera.ScreenToWorldPoint(_bounds.center));
                SlicedSprite.dimensions = margin + _bounds.size*20f *Scale +
                                          new Vector3(SlicedSprite.borderLeft + SlicedSprite.borderRight,
                                                      SlicedSprite.borderBottom + SlicedSprite.borderTop, 0)*8f;
                SlicedSprite.anchor = tk2dBaseSprite.Anchor.MiddleCenter;

                SlicedSprite.ForceBuild();
            }
        }

        private void Awake()
        {
            SlicedSprite = GetComponent<tk2dSlicedSprite>();
        }

        private void Update()
        {
            if (UpdateAutomaticaly)
                RecalculateBounds();
        }

        public void RecalculateBounds()
        {
            if (TargetContainer == null)
            {
                throw new Exception("TargetContainer is null.");
            }

            var renderers = new List<Renderer>(TargetContainer.GetComponentsInChildren<Renderer>());

            if(renderers.Count <= 0 || renderers[0] == null) 
                return;

            Bounds bounds = new Bounds();
            foreach (var renderer1 in renderers)
            {
                if (renderer1.enabled && renderer1.GetComponent<MeshFilter>().sharedMesh != null)
                {
                    bounds = renderer1.bounds;
                    break;
                }
            }

            foreach (var renderer1 in renderers)
            {
                if (renderer1.enabled && renderer1.GetComponent<MeshFilter>().sharedMesh != null)
                        bounds.Encapsulate(renderer1.bounds);
            }

            Bounds = bounds;
        }
    }
}

