#if SERVER
using Code.Core.Client.UI;

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
