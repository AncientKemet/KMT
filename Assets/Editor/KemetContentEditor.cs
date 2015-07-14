using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Core.Shared.Content;
using Code.Libaries.Generic.Managers;
using Server.Model.Content.Spawns;
using Shared.Content;
using Shared.Content.Types;
using Shared.Content.Types.ItemExtensions;
using Shared.SharedTypes;
using Shared.StructClasses;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class KemetContentEditor : EditorWindow
    {
        [MenuItem("Kemet/Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(KemetContentEditor)).Show();
        }

        public enum TabType
        {
            Items,
            Recipes,
            Npcs
        }

        private TabType tab;

        private Vector2 _mainScroll = Vector2.zero;

        private ContentItem _selected;
        private UnityEditor.Editor _editor;

        void OnGUI()
        {
            tab = (TabType)GUILayout.Toolbar((int)tab, new[] { "Items", "Recipes", "Npcs" });
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical("box");
                {
                    var list = GetContentList(tab);
                    if (GUILayout.Button("New"))
                    {
                        string path = "Assets/Development/Libary/" + tab.ToString() + "/new.prefab";
                        GameObject go = (GameObject)PrefabUtility.CreatePrefab(path, new GameObject());
                        switch (tab)
                        {
                            case TabType.Items:
                                go.AddComponent<Item>();
                                go.AddComponent<ItemRigid>();
                                break;
                            case TabType.Recipes:
                                go.AddComponent<ItemRecipe>();
                                break;
                            case TabType.Npcs:
                                go.AddComponent<NPCSpawn>();
                                break;
                        }
                        Scan(tab);
                        Selection.activeGameObject = go;
                    }
                    if (GUILayout.Button("Remove Nulls"))
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            list.Remove(null);
                        }
                        EditorUtility.SetDirty(ContentManager.I);
                    }
                    if (GUILayout.Button("Remove Duplicates"))
                    {
                        List<object> toRemove = new List<object>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            var o = list[i];
                            for (int i2 = 0; i2 < list.Count; i2++)
                            {
                                if (list[i2] == o && i != i2)
                                {
                                    list[i2] = null;
                                    list.Remove(null);
                                }
                            }
                        }
                        EditorUtility.SetDirty(ContentManager.I);
                    }
                    if (GUILayout.Button("Scan"))
                    {
                        Scan(tab);
                    }
                    if (tab == TabType.Items)
                    {
                        if (GUILayout.Button("All Icon"))
                        {
                            foreach (var item in ContentManager.I.Items)
                            {
                                if (item != null)
                                    item.CreateIcon();
                                Repaint();
                            }
                            EditorUtility.SetDirty(ContentManager.I);
                        }
                    }

                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical("box");
                {

                    _mainScroll = GUILayout.BeginScrollView(_mainScroll);
                    var list = GetContentList(tab);
                    for (int i = 0; i < list.Count; i++)
                    {
                        GUILayout.BeginHorizontal("box");
                        {
                            ContentItem item = (ContentItem)list[i];
                            if (item != null)
                            {
                                EditorUtility.SetDirty(item);
                                GUILayout.Label("[" + item.InContentManagerIndex + "]");

                                if (GUILayout.Button("Select", GUILayout.MaxWidth(60)))
                                {
                                    _selected = item;
                                    Selection.activeGameObject = item.gameObject;
                                }
                                item.name = GUILayout.TextField(item.name);

                                if (tab == TabType.Items)
                                    GUILayout.Label(((Item)item).Icon, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));

                                if (tab == TabType.Recipes)
                                {
                                    var recipe = ((ItemRecipe)item);
                                    GUILayout.BeginVertical("box", GUILayout.MaxWidth(128));
                                    {
                                        //level requirements
                                        {
                                            recipe._foldOutRequirements = EditorGUILayout.Foldout(recipe._foldOutRequirements,"Requirements");
                                            if (recipe._foldOutRequirements)
                                            {
                                                GUILayout.BeginHorizontal();
                                                if (GUILayout.Button("Add"))
                                                {
                                                    recipe.Requirements.Add(new LevelRequirement());
                                                }
                                                GUILayout.EndHorizontal();


                                                for (int req = 0; req < recipe.Requirements.Count; req++)
                                                {
                                                    var requirement = recipe.Requirements[req];
                                                    GUILayout.BeginHorizontal();
                                                    requirement.Skill =
                                                        (Levels.Skills) EditorGUILayout.EnumPopup(requirement.Skill);
                                                    requirement.Val =
                                                        (byte) Mathf.Max(EditorGUILayout.IntField(requirement.Val), 1);
                                                    if (GUILayout.Button("-"))
                                                    {
                                                        recipe.Requirements.Remove(requirement);
                                                    }
                                                    GUILayout.EndHorizontal();
                                                }
                                            }
                                        }
                                        //xp rewards
                                        {
                                            recipe._foldOutRewards = EditorGUILayout.Foldout(recipe._foldOutRewards, "Experience gained");
                                            if (recipe._foldOutRewards)
                                            {
                                                if (GUILayout.Button("Add"))
                                                {
                                                    recipe.Rewards.Add(new ExperienceReward());
                                                }
                                                for (int rew = 0; rew < recipe.Rewards.Count; rew++)
                                                {
                                                    var reward = recipe.Rewards[rew];
                                                    GUILayout.BeginHorizontal();
                                                    {
                                                        reward.Skill =
                                                            (Levels.Skills) EditorGUILayout.EnumPopup(reward.Skill);
                                                        reward.Val = Mathf.Max(EditorGUILayout.FloatField(reward.Val),
                                                                               1f);
                                                        if (GUILayout.Button("-"))
                                                        {
                                                            recipe.Rewards.Remove(reward);
                                                        }
                                                    }
                                                    GUILayout.EndHorizontal();
                                                }
                                            }
                                        }
                                    }
                                    GUILayout.EndVertical();

                                    GUILayout.BeginVertical("box", GUILayout.MaxWidth(128));
                                    {
                                        if (recipe.Item1.Item == null)
                                            recipe.Item1 = new Item.ItemInstance(ContentManager.I.Items[0], 1);
                                        GUILayout.Label(recipe.Item1.Item.name);
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("X");
                                        recipe.Item1.Amount = EditorGUILayout.IntField(recipe.Item1.Amount);
                                        GUILayout.EndHorizontal();
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("con.");
                                        recipe.isConsumed1 = EditorGUILayout.Toggle(recipe.isConsumed1);
                                        GUILayout.EndHorizontal();
                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.Label(recipe.Item1.Item.Icon, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));
                                    GUILayout.Label("+");
                                    GUILayout.BeginVertical("box", GUILayout.MaxWidth(128));
                                    {
                                        if (recipe.Item2.Item == null)
                                            recipe.Item2 = new Item.ItemInstance(ContentManager.I.Items[0], 1);
                                        GUILayout.Label(recipe.Item2.Item.name);
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("X");
                                        recipe.Item2.Amount = EditorGUILayout.IntField(recipe.Item2.Amount);
                                        GUILayout.EndHorizontal();
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("con.");
                                        recipe.isConsumed2 = EditorGUILayout.Toggle(recipe.isConsumed2);
                                        GUILayout.EndHorizontal();
                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.Label(recipe.Item2.Item.Icon, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));
                                    GUILayout.Label("=");
                                    GUILayout.BeginVertical("box", GUILayout.MaxWidth(128));
                                    {
                                        if (recipe.Result.Item == null)
                                            recipe.Result = new Item.ItemInstance(ContentManager.I.Items[0], 1);
                                        //item result
                                        GUILayout.Label(recipe.Result.Item.name);
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("X");
                                        recipe.Result.Amount = EditorGUILayout.IntField(recipe.Result.Amount);
                                        GUILayout.EndHorizontal();
                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.Label(recipe.Result.Item.Icon, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));
                                }

                                if (tab == TabType.Items)
                                    if (GUILayout.Button("Icon", GUILayout.MaxWidth(60)))
                                        ((Item)item).CreateIcon();

                                /*if (GUILayout.Button("Clone", GUILayout.MaxWidth(60)))
                                {
                                    AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(item.gameObject),
                                                            "Assets/Development/Libary/" + tab + "/" + item.gameObject.name + "(clone).prefab");
                                    AssetDatabase.ImportAsset("Assets/Development/Libary/" + tab + "/" + item.gameObject.name + "(clone).prefab");
                                    AssetDatabase.Refresh();
                                    Scan(tab);
                                }*/
                                /*if (GUILayout.Button("Recreate", GUILayout.MaxWidth(60)))
                                {
                                    AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(item.gameObject),
                                                            "Assets/Development/Libary/"+tab+"/" + item.gameObject.name + ".prefab");
                                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.gameObject));
                                    AssetDatabase.Refresh();
                                    Scan(tab);
                                }*/
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndScrollView();
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            EditorUtility.SetDirty(ContentManager.I);
        }

        private static void Scan(TabType tabType)
        {
            string mainPath = "Assets/Development/Libary/" + tabType.ToString() + "/";

            var files = Directory.GetFiles(mainPath);
            var list = GetContentList(tabType);
            if (list != null)
            {
                foreach (var file in files)
                {
                    if (file.EndsWith(".prefab"))
                    {
                        var go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                        var item = go.GetComponent<ContentItem>();
                        if (item.InContentManagerIndex == -1 || list[item.InContentManagerIndex] != item)
                        {
                            list.Add(item);
                            item.InContentManagerIndex = list.IndexOf(item);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Couldnt find good list for " + tabType);
            }
            EditorUtility.SetDirty(ContentManager.I);
        }

        private static System.Collections.IList GetContentList(TabType tabType)
        {
            switch (tabType)
            {
                case TabType.Items:
                    return ContentManager.I.Items;
                case TabType.Recipes:
                    return ContentManager.I.Recipes;
                case TabType.Npcs:
                    return ContentManager.I.Npcs;
                default:
                    Debug.LogError("Unknown tabtype " + tabType);
                    return null;
            }
        }
    }
}