#if SERVER
using Code.Core.Client.UI;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces
{
    public abstract class AInterface
    {
        protected Player player
        {
            get { return ui.Player; }
        }

        protected ClientUI ui;
        private bool _opened;

        public AInterface(ClientUI ui)
        {
            this.ui = ui;
        }

        public abstract InterfaceType GetInterfaceType { get; }

        public virtual void OnOpen() { }
        public virtual void OnClose() { }
        public virtual void OnEvent(string action, int controlId) { }

        public bool Opened
        {
            get { return _opened; }
            set
            {

                if (value)
                {
                    if (ui.IsClosed(GetInterfaceType))
                    {
                        ui.Open(GetInterfaceType);
                        OnOpen();
                    }
                }
                else
                {
                    if (ui.IsOpened(GetInterfaceType))
                    {
                        ui.Close(GetInterfaceType);
                        OnClose();
                    }
                }

                _opened = value;
            }
        }
    }
}
#endif
