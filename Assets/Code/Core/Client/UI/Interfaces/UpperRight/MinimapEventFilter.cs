using System.Collections.Generic;
using Code.Core.Client.UI.Controls;
using Shared.SharedTypes;
using UnityEngine;

namespace Client.UI.Interfaces.UpperRight
{
    public class MinimapEventFilter : InterfaceButton
    {
        [SerializeField]
        private tk2dTextMesh _notificationLabel;

        [SerializeField] private List<MinimapEventButton> Buttons;

        public MinimapEventType EventType;

        private LinkedList<MinimapEvent> _events = new LinkedList<MinimapEvent>();


        public void Add(MinimapEvent e)
        {
            _events.AddFirst(e);
            if(_events.Count > 10)
                _events.RemoveLast();
            ReloadButtons();
        }

        private void ReloadButtons()
        {
            foreach (var b in Buttons)
            {
                b.gameObject.SetActive(false);
            }

            int index = 0;
            foreach (var e in _events)
            {
                Buttons[index].Event = e;
                index++;
            }
        }
    }
}
