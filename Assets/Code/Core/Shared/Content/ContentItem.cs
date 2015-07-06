using System;
using Code.Libaries.Generic.Managers;
using UnityEngine;

namespace Code.Core.Shared.Content
{
    [Serializable]
    public class ContentItem : MonoBehaviour
    {

        [HideInInspector][SerializeField]
        private int _inContentManagerIndex = -1;
        public int InContentManagerIndex
        {
            get
            {
                return _inContentManagerIndex;
            }
#if UNITY_EDITOR
            set { _inContentManagerIndex = value; }
#endif
        }
    }
}

