using Code.Libaries.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Shared.NET
{
    public class NetworkConfig : SIAsset<NetworkConfig>
    {
        public int MasterServerPort = 54787;
        public int DataServerPort = 54788;
        public int WorldServerPort = 54785;
        public int LoginServerPort = 54784;
        public int WorldUnprecieseMovmentPort = 54783;

#if UNITY_EDITOR
        [MenuItem("Kemet/Open/Network")]
        private static void SelectAsset()
        {
            Selection.activeObject = I;
        }
#endif
    }
}
