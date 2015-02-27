//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Code.Libaries.GameObjects
{
    public static class TransformHelper
    {
        public static Transform FindTraverseChildren(string _objectName, Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if(child != null){
                    if(child.name == _objectName){
                        return child;
                    }else{
                        Transform subChild = FindTraverseChildren(_objectName, child);
                        if(subChild != null)
                        {
                            return subChild;
                        }
                    }
                }
            }
            return null;
        }

        public static List<Transform> GetChildren(Transform transform)
        {
            List<Transform> list = new List<Transform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                list.Add(transform.GetChild(i));
                list.AddRange(GetChildren(transform.GetChild(i)));
            }

            return list;
        }

        
    }
}

