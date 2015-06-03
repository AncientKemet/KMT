using System;
using System.IO;
using System.Linq;
using Code.Core.Shared.Content;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEngine;
using UnityEditor;
using System.Collections;

class KemetContentEditor : EditorWindow
{
    [MenuItem("Kemet/Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(KemetContentEditor)).Show();
    }

    private enum TabType
    {
        Items,
        Npcs,
        Models
    }

    private TabType tab;

    private Vector2 _mainScroll = Vector2.zero;

    private ContentItem _selected;
    private Editor _editor;

    void OnGUI()
    {
        tab = (TabType)GUILayout.Toolbar((int)tab, new[] { "Items", "Npcs", "Models" });
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical("box");
            {
                switch (tab)
                {
                    case TabType.Items:
                        if (GUILayout.Button("Remove Nulls"))
                        {
                            ContentManager.I.Items.RemoveAll(item => item == null);
                            EditorUtility.SetDirty(ContentManager.I);
                        }
                        if (GUILayout.Button("Remove Duplicates"))
                        {
                            ContentManager.I.Items = ContentManager.I.Items.Distinct().ToList();
                            EditorUtility.SetDirty(ContentManager.I);
                        }
                        if (GUILayout.Button("Scan"))
                        {
                            ScanItems();
                        }if (GUILayout.Button("All Icon"))
                        {
                            foreach (var item in ContentManager.I.Items)
                            {if(item != null)
                                item.CreateIcon();
                                Repaint();
                            }
                            EditorUtility.SetDirty(ContentManager.I);
                        }
                        break;
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box");
            {
                switch (tab)
                {
                    case TabType.Items:
                        _mainScroll = GUILayout.BeginScrollView(_mainScroll);

                        for (int i = 0; i < ContentManager.I.Items.Count; i++)
                        {
                            GUILayout.BeginHorizontal("box");
                            {
                               GUILayout.Label("[" + i + "]");

                                Item item = ContentManager.I.Items[i];
                                if (item != null)
                                {
                                    GUILayout.Label("" + item.name);

                                    GUILayout.Label(item.Icon, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));
                                    
                                    if (GUILayout.Button("Select", GUILayout.MaxWidth(60)))
                                    {
                                        _selected = item;
                                        Selection.activeGameObject = item.gameObject;
                                    }
                                    if (GUILayout.Button("Icon", GUILayout.MaxWidth(60)))
                                        item.CreateIcon();

                                    if (GUILayout.Button("Clone", GUILayout.MaxWidth(60)))
                                    {
                                        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(item.gameObject),
                                            "Assets/Development/Libary/Items/" + item.gameObject.name + "(clone).prefab");
                                        AssetDatabase.ImportAsset("Assets/Development/Libary/Items/" + item.gameObject.name + "(clone).prefab");
                                        AssetDatabase.Refresh();
                                        ScanItems();
                                    }
                                    if (GUILayout.Button("Recreate", GUILayout.MaxWidth(60)))
                                    {
                                        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(item.gameObject),
                                            "Assets/Development/Libary/Items/" + item.gameObject.name + ".prefab");
                                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.gameObject));
                                        AssetDatabase.Refresh();
                                        ScanItems();
                                    }
                                }
                            }
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                        break;
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    private static void ScanItems()
    {
        string mainPath = "Assets/Development/Libary/Items/";

        var files = Directory.GetFiles(mainPath);

        foreach (var file in files)
        {
            if (file.EndsWith(".prefab"))
            {
                var go = AssetDatabase.LoadAssetAtPath(file, typeof (GameObject)) as GameObject;
                var item = go.GetComponent<Item>();
                if (item.InContentManagerIndex == -1)
                {
                    ContentManager.I.Items.Add(item);
                }
            }
        }
        EditorUtility.SetDirty(ContentManager.I);
    }
}