using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Code.Libaries.Generic
{
    /// <summary>
    /// Single Instance Asset.
    /// </summary>
    public class SIAsset<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _asset;

        /// <summary>
        /// Instance of asset.
        /// </summary>
        public static T I
        {
            get
            {
                if (_asset == null)
                {
                    _asset = Resources.Load<T>(GetAssetPath(typeof(T)));

                    #region AssetCreation

#if UNITY_EDITOR
                    if (_asset == null)
                    {
                        _asset = CreateInstance<T>();
                        AssetDatabase.CreateAsset(_asset, "Assets/Resources/"+GetAssetPath(typeof (T))+".asset");
                        AssetDatabase.SaveAssets();
                    }
#endif

                    #endregion
                }
                return _asset;
            }
        }

        /// <summary>
        /// Returns SIAsset path in resources folder.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetAssetPath(Type type)
        {
            return "SIAssets/" + type.Name;
        }
    }
}
