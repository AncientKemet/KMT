﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO; 

public class ExportTexture : MonoBehaviour {

 
    [MenuItem("Assets/Export Texture") ]
    static void Apply () { 
       var texture = Selection.activeObject as Texture2D; 
       if (texture == null) { 
          EditorUtility.DisplayDialog("Select Texture", "You Must Select a Texture first!", "Ok"); 
          return; 
       } 
 
       var bytes = texture.EncodeToPNG(); 
       File.WriteAllBytes(Application.dataPath + "/exported_texture.png", bytes); 
    } 
}