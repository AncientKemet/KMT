#if SERVER
using System;
using Code.Core.Client.UI;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces
{
    public class LoginInterface : AInterface
    {
        public LoginInterface(ClientUI ui) : base(ui)
        {
        }

        public override InterfaceType GetInterfaceType
        {
            get { return InterfaceType.LoginInterface;}
        }
    }
}
#endif
