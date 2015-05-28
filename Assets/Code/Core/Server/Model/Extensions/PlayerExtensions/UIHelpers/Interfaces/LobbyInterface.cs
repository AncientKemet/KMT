#if SERVER
using Code.Core.Client.UI;
using UnityEngine;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces
{
    public class LobbyInterface : AInterface
    {
        public LobbyInterface(ClientUI ui) : base(ui)
        {
        }

        public override void OnEvent(string action, int controlId)
        {
            Debug.Log("Event: "+action+" id: "+controlId);
        }

        public override InterfaceType GetInterfaceType
        {
            get { return InterfaceType.LobbyInterface;}
        }
    }
}
#endif
