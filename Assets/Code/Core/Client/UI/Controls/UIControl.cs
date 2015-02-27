using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Client.UI.Controls
{
    public class UIControl : Clickable {

        [SerializeField]
        protected int _index = -1;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public InterfaceType InterfaceId { get; set; }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Is called when server sents new data to this control.
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnSetData(List<float> data)
        {
            Debug.LogError("Unimplemented OnSetData type: "+GetType());
        }
    }
}
