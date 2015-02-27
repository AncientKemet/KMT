using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Libaries.IO
{
    public static class Prefabs
    {

        public static T GetComponent<T>(string assetPath) where T : MonoBehaviour
        {
            Object o = Resources.Load(assetPath);
            if (o is GameObject)
            {
                return (o as GameObject).GetComponent<T>();
            }
            else if(o == null)
            {
                throw new Exception("Nonexisiting asset: " + assetPath);
            }
            else
                throw new Exception("Invalid asset: "+assetPath);
            return null;
        }

#if UNITY_EDITOR

        public static T GetAsset<T>(string assetPath) where T : ScriptableObject
        {
            Object o = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
            if (o is ScriptableObject)
            {
                return o as T;
            }
            else if (o == null)
            {
                throw new Exception("Nonexisiting asset: " + assetPath);
            }
            else
                throw new Exception("Invalid asset: " + assetPath);
            return null;
        }

#endif
    }
}

