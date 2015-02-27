using System;
using UnityEngine;

namespace Code.Core.Shared.Content
{
    [Serializable]
    public class ContentItem : MonoBehaviour
    {

        [HideInInspector][SerializeField] private string _guid;

        public string GUID
        {
            get
            {
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(_guid))
                {
                    _guid = Guid.NewGuid().ToString();
                }
#endif
                return _guid;
            }
            set { _guid = value; }
        }
    }
}

