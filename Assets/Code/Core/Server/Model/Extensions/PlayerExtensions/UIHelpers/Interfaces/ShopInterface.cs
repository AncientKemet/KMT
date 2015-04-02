using Libaries.Net.Packets.ForClient;
using Server.Model.Content.Spawns.NpcSpawns;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces
{
    public class ShopInterface {

        public ShopInterface(ClientUI _ui)
        {
            this.UI = _ui;
        }

        private NpcShop _openedShop;
        private ClientUI UI;

        public NpcShop OpenedShop
        {
            get { return _openedShop; }
            set
            {
                
                if (value == null)
                {
                    if (_openedShop != null)
                        _openedShop.DeattachPlayer(UI.Player);
                    if (UI.ProfileInterface.Opened)
                        if (UI.ProfileInterface.Tab == ProfileInterfaceUpdatePacket.PacketTab.Vendor)
                        {
                            UI.ProfileInterface.Opened = false;
                        }
                }
                else
                {
                    value.AttachPlayer(UI.Player);
                    value.SendFullStockTo(UI.Player);
                    if (!UI.ProfileInterface.Opened || UI.ProfileInterface.Tab != ProfileInterfaceUpdatePacket.PacketTab.Vendor)
                        UI.ProfileInterface.Open(value.GetComponent<NPC>(), ProfileInterfaceUpdatePacket.PacketTab.Vendor);
                }
                
                _openedShop = value;
            }
        }
    }
}
