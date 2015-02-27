using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.UI.Controls.Tool
{
    [ExecuteInEditMode]
    public class Slider : MonoBehaviour
    {

        public tk2dUIMask UpperMask, LowerMask;
        public tk2dSlicedSprite Background;
        public GameObject ContentGameObject;
        public tk2dSlicedSprite SlidingSprite;
        public float width = 8f;
        public float height = 8f;
        public bool RecalculateBoundsAutomaticaly = true;
        public float _time = 0.5f;

        public Vector3 AdditionUp = Vector3.zero;
        public Vector3 AdditionDown = Vector3.zero;

        public Bounds Bounds { get; set; }

        void LateUpdate ()
        {
            if (RecalculateBoundsAutomaticaly)
            {
                _time -= Time.deltaTime;
                if (_time <= 0)
                {
                    _time = 0.5f;
                    RecalculateBounds();
                }
            }
            SlidingSprite.dimensions = new Vector2(SlidingSprite.dimensions.x, Mathf.Clamp((height / (Bounds.size.y / 15f)) * 20f, 40f, height * 20) * 2f);
                //SlidingSprite.gameObject.SetActive(Bounds.size.y > height);
            SlidingSprite.transform.localPosition = new Vector3(
                width / 2f,
                Mathf.Clamp(
                SlidingSprite.transform.localPosition.y,
                -height + SlidingSprite.dimensions.y / 20f / 2f * SlidingSprite.scale.y,
                -SlidingSprite.dimensions.y / 20f / 2f * SlidingSprite.scale.y));
#if UNITY_EDITOR

            UpperMask.size = new Vector2(width, 30);
            LowerMask.size = new Vector2(width, 30);

            UpperMask.Build();
            LowerMask.Build();
            UpperMask.transform.position = transform.position + Vector3.forward * -1;
            LowerMask.transform.position = transform.position - Vector3.up * height + Vector3.forward * -1;
            Background.dimensions = new Vector2(width * 20, height * 20);
            
#endif
            ContentGameObject.transform.localPosition = new Vector3(0, (-(SlidingSprite.transform.localPosition.y + SlidingSprite.dimensions.y * SlidingSprite.scale.y / 20f / 2f) * Mathf.Clamp(Bounds.size.y - height,0,9999f)) / (height - SlidingSprite.dimensions.y / 20f * SlidingSprite.scale.y +0.0001f), -1f);
        }

        public void RecalculateBounds()
        {
            if (ContentGameObject == null)
            {
                throw new Exception("TargetContainer is null.");
            }

            var renderers = new List<Renderer>(ContentGameObject.GetComponentsInChildren<Renderer>());

            if(renderers.Count <= 0 || renderers[0] == null) 
                return;

            Bounds bounds = new Bounds(renderers[0].transform.position, Vector3.zero);

            foreach (var renderer1 in renderers)
            {
                bounds.Encapsulate(renderer1.bounds);
            }

            bounds.Expand(AdditionUp);
            bounds.Expand(AdditionDown);

            Bounds = bounds;
        }
    }
}
