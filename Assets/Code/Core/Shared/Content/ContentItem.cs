using System;
using UnityEngine;

namespace Shared.Content
{
    [Serializable]
    public class ContentItem : MonoBehaviour
    {
        [HideInInspector][SerializeField]
        private int _inContentManagerIndex = -1;

        /// <summary>
        /// Returns a cached value of ContentManager.I.?List.IndexOf(this).
        /// Is set in KemetContentEditor.
        /// </summary>
        public int InContentManagerIndex
        {
            get { return _inContentManagerIndex; }
#if UNITY_EDITOR
            set { _inContentManagerIndex = value; }
#endif
        }
    }
}