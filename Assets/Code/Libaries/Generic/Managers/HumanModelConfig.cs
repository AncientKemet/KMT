using System.Collections.Generic;
using Code.Libaries.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Libaries.Generic.Managers
{
    public class HumanModelConfig : SIAsset<HumanModelConfig>
    {

        public List<Texture2D> MaleFaceTextures;
        public List<Texture2D> FemaleFaceTextures;
        public List<Color> FaceColors;

        public List<MeshRenderer> MaleHairs;
        public List<bool> MaleHairsEars;
        public List<MeshRenderer> FemaleHairs;
        public List<bool> FemaleHairsEars;
        public List<Color> HairsColors;
        
        public List<Texture2D> SkinTextures;
        public List<Color> SkinColors;
        public List<Color> UnderWearColors;

#if UNITY_EDITOR
        [MenuItem("Kemet/Open/Human Config")]
        private static void SelectAsset()
        {
            Selection.activeObject = I;
        }
#endif
    }
}
