using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Scripts;
using Shared.SharedTypes;

namespace Client.UI.Interfaces.UpperRight
{
    public class MinimapEventButton : InterfaceButton
    {
        public Icon Icon;
        public tk2dTextMesh Title;
        private MinimapEvent _event;

        public MinimapEvent Event
        {
            get { return _event; }
            set
            {
                _event = value;
                gameObject.SetActive(true);
            }
        }
    }
}
