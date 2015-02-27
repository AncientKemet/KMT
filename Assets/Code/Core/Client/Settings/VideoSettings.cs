using Code.Libaries.Generic;
using Code.Libaries.IO;

namespace Code.Core.Client.Settings
{
    public class VideoSettings : Singleton<VideoSettings>
    {

        public enum PhysicsQuality
        {
            Low,
            Medium,
            High
        }

        public PPFEnum<PhysicsQuality> Physics = new PPFEnum<PhysicsQuality>("User-Physics"); 

    }
}
