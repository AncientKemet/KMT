#if SERVER
using System.Collections.Generic;
using Libaries.IO;

namespace Server.Model.ContentHandling.Player.AccounteExtensions
{
    public class CharacterCustomalizations : AccountExtension
    {

        public List<int> UnlockedMaleHairs = new List<int>();
        public List<int> UnlockedFemaleHairs = new List<int>();
        public List<int> UnlockedHairColors = new List<int>();
        public List<int> UnlockedEyes = new List<int>();
        public List<int> UnlockedEyeColors = new List<int>();
        public List<int> UnlockedSkinTypes = new List<int>();
        public List<int> UnlockedSkinColors = new List<int>();
        public List<int> UnlockedUnderwearColors = new List<int>();

        public override void LoadFromJson(JSONObject o)
        {
            LoadListFromJson(ref UnlockedMaleHairs, "UnlockedMaleHairs", o);
            LoadListFromJson(ref UnlockedFemaleHairs, "UnlockedFemaleHairs", o);
            LoadListFromJson(ref UnlockedHairColors, "UnlockedHairColors", o);
            LoadListFromJson(ref UnlockedEyes, "UnlockedEyes", o);
            LoadListFromJson(ref UnlockedEyeColors, "UnlockedEyeColors", o);
            LoadListFromJson(ref UnlockedSkinTypes, "UnlockedSkinTypes", o);
            LoadListFromJson(ref UnlockedSkinColors, "UnlockedSkinColors", o);
            LoadListFromJson(ref UnlockedUnderwearColors, "UnlockedUnderwearColors", o);
        }

        public override void SaveToJson(JSONObject o)
        {
            SaveListToJson(ref UnlockedMaleHairs, "UnlockedMaleHairs", o);
            SaveListToJson(ref UnlockedFemaleHairs, "UnlockedFemaleHairs", o);
            SaveListToJson(ref UnlockedHairColors, "UnlockedHairColors", o);
            SaveListToJson(ref UnlockedEyes, "UnlockedEyes", o);
            SaveListToJson(ref UnlockedEyeColors, "UnlockedEyeColors", o);
            SaveListToJson(ref UnlockedSkinTypes, "UnlockedSkinTypes", o);
            SaveListToJson(ref UnlockedSkinColors, "UnlockedSkinColors", o);
            SaveListToJson(ref UnlockedUnderwearColors, "UnlockedUnderwearColors", o);
        }

        public void UnlockDefaults()
        {
            for (int i = 0; i < 3; i++)
            {
                UnlockedMaleHairs.Add(1);
                UnlockedFemaleHairs.Add(1);
                UnlockedHairColors.Add(1);
                UnlockedEyes.Add(1);
                UnlockedEyeColors.Add(1);
                UnlockedSkinTypes.Add(1);
                UnlockedSkinColors.Add(1);
                UnlockedUnderwearColors.Add(1);
            }

        }
    }
}
#endif
