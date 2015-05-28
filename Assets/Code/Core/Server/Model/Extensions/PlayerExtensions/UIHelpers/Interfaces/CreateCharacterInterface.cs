#if SERVER
using Code.Core.Client.UI;
using Libaries.IO;
using Libaries.Net.Packets.ForClient;
using Server.Model.ContentHandling.Player.AccounteExtensions;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces
{
    public class CreateCharacterInterface : AInterface {

        private CharacterCustomalizations _characterCustomalizations;

        public string RequestedUsername { get; set; }

        public CreateCharacterInterface(ClientUI ui) : base(ui)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            _characterCustomalizations = ui.Player.Client.UserAccount.CharacterCustomalizations.Get;

            JSONObject o = new JSONObject();

            _characterCustomalizations.SaveToJson(o);

            CharacterCustomalizationDataPacket packet = new CharacterCustomalizationDataPacket();
            packet.JsonObject = o;

            player.Client.ConnectionHandler.SendPacket(packet);
            
            ui.Open(InterfaceType.Chat);
            ui.Open(InterfaceType.ChatPanel);
            ui.Open(InterfaceType.ActionBars);
            ui.Open(InterfaceType.LowerLeftMenu);
            ui.Open(InterfaceType.StatsBars);
            ui.Open(InterfaceType.MyCharacterInventory);
            ui.Open(InterfaceType.UnitSelectionInterface);

            ui.Close(InterfaceType.Chat);
            ui.Close(InterfaceType.ChatPanel);
            ui.Close(InterfaceType.ActionBars);
            ui.Close(InterfaceType.LowerLeftMenu);
            ui.Close(InterfaceType.MyCharacterInventory);
            ui.Close(InterfaceType.ProfileInterface);
            ui.Close(InterfaceType.StatsBars);
            ui.Close(InterfaceType.UnitSelectionInterface);
        }

        public override void OnClose()
        {
            ui.Open(InterfaceType.Chat);
            ui.Open(InterfaceType.ChatPanel);
            ui.Open(InterfaceType.ActionBars);
            ui.Open(InterfaceType.LowerLeftMenu);
            ui.Open(InterfaceType.MyCharacterInventory);
            ui.Open(InterfaceType.StatsBars);
            ui.Open(InterfaceType.UnitSelectionInterface);
        }
        
        public override void OnEvent(string action, int controlId)
        {
            if (action == "Finish")
            {
                player.name = RequestedUsername;
                Opened = false;
            }
        }
        
        public override InterfaceType GetInterfaceType
        {
            get { return InterfaceType.CreateCharacterInterface;}
        }
    }
}
#endif
