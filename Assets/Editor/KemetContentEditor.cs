using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
                                GUILayout.BeginVertical(GUILayout.MaxWidth(120));
                                GUILayout.Label("[" + item.InContentManagerIndex + "]");
                                GUILayout.BeginHorizontal();
                                if (GUILayout.Button("Sel", GUILayout.MaxWidth(80)))
                                {
                                    _selected = item;
                                    Selection.activeGameObject = item.gameObject;
                                }
                                if (GUILayout.Button("Dup", GUILayout.MaxWidth(80)))
                                {
                                    Duplicate(tab, item);
                                }
                                if (GUILayout.Button("Del", GUILayout.MaxWidth(80)))
                                {
                                    Delete(tab, item);
                                    GUILayout.EndHorizontal();
                                    GUILayout.EndVertical();
                                    continue;
                                }
                                GUILayout.EndHorizontal();
                                item.name = GUILayout.TextField(item.name, GUILayout.MaxWidth(120));
                                GUILayout.EndVertical();
                                #region Items
                                if (tab == TabType.Items)
                                {
                                    var it = (Item) item;
                                    GUILayout.BeginVertical();
                                    GUILayout.Label(((Item) item).Icon, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));
                                    if (GUILayout.Button("Icon", GUILayout.MaxWidth(64)))
                                        ((Item)item).CreateIcon();
                                    GUILayout.EndVertical();

                                    GUILayout.BeginVertical(GUILayout.MaxWidth(90));
                                    {
                                        GUILayout.Label("Stacks");
                                        it.MaxStacks = EditorGUILayout.IntField(it.MaxStacks);
                                    }
                                    GUILayout.EndVertical();

                                    if (it.EQ != null)
                                    {
                                        GUILayout.BeginVertical(GUILayout.MaxWidth(90));
                                        {
                                            it.EQ.Class = (Class)EditorGUILayout.EnumPopup(it.EQ.Class);
                                            it.EQ.Role = (Role)EditorGUILayout.EnumPopup(it.EQ.Role);
                                       
                                            GUILayout.Label("Required level");
                                            it.EQ.RequiredLevel = EditorGUILayout.IntField(it.EQ.RequiredLevel);
                                        }
                                        GUILayout.EndVertical();
                                    }
                                }
                                #endregion
                                #region RECIPES
                                if (tab == TabType.Recipes)
                                {
                                    var recipe = ((ItemRecipe)item);
                                    GUILayout.BeginVertical("box", GUILayout.MaxWidth(128));
                                    {
                                        //level requirements
                                        {
                                            recipe._foldOutRequirements = EditorGUILayout.Foldout(recipe._foldOutRequirements, "Requirements");
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
                                                        (Levels.Skills)EditorGUILayout.EnumPopup(requirement.Skill);
                                                    requirement.Val =
                                                        (byte)Mathf.Max(EditorGUILayout.IntField(requirement.Val), 1);
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
                                            recipe._foldOutRewards = EditorGUILayout.Foldout(recipe._foldOutRewards, "Experience");
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
                                                            (Levels.Skills)EditorGUILayout.EnumPopup(reward.Skill);
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

                                    

                                    int x = 0;

                                    GUILayout.BeginVertical("box");
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label("Craft time: ");
                                        recipe.CraftTime = EditorGUILayout.Slider(recipe.CraftTime, 0.3f, 60f);
                                        GUILayout.Label("seconds");
                                    }
                                    GUILayout.EndHorizontal();
                                    recipe._foldOutItemRequirements = EditorGUILayout.Foldout(recipe._foldOutItemRequirements, "Items required");
                                    if (recipe._foldOutItemRequirements)
                                    {
                                        GUILayout.BeginHorizontal();
                                        if (GUILayout.Button("Add"))
                                        {
                                            recipe.ItemRequirements.Add(new ItemRecipe.ItemRequirement()
                                            {
                                                IsConsumed = false,
                                                Item = new Item.ItemInstance(ContentManager.I.Items[0], 1)
                                            });
                                        }
                                        GUILayout.EndHorizontal();
                                        for (int id = 0; id < recipe.ItemRequirements.Count; id ++)
                                        {
                                            var itemRequirement = recipe.ItemRequirements[id];
                                            if (x == 0)
                                                GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.BeginVertical("box", GUILayout.MaxWidth(150));
                                                {
                                                    if (itemRequirement.Item.Item == null)
                                                        itemRequirement.Item =
                                                            new Item.ItemInstance(ContentManager.I.Items[0], 1);
                                                    GUILayout.BeginHorizontal();
                                                    if (GUILayout.Button("X"))
                                                    {
                                                        recipe.ItemRequirements.Remove(itemRequirement);
                                                        if (id == 0)
                                                        {
                                                            GUILayout.EndHorizontal();
                                                            GUILayout.EndVertical();
                                                            continue;
                                                        }
                                                        id --;
                                                        itemRequirement = recipe.ItemRequirements[id];
                                                    }
                                                    GUILayout.Label(itemRequirement.Item.Item.name);
                                                    GUILayout.EndHorizontal();
                                                    GUILayout.BeginHorizontal();
                                                    itemRequirement.Item.Amount =
                                                        Mathf.Clamp(
                                                            EditorGUILayout.IntField(itemRequirement.Item.Amount), 0,
                                                            999);
                                                    GUILayout.EndHorizontal();
                                                    GUILayout.BeginHorizontal();
                                                    GUILayout.Label("C.");
                                                    itemRequirement.IsConsumed =
                                                        EditorGUILayout.Toggle(itemRequirement.IsConsumed);
                                                    if (GUILayout.Button(itemRequirement.Item.Item.Icon,
                                                                         GUILayout.MaxWidth(50),
                                                                         GUILayout.MaxHeight(50)))
                                                    {
                                                        ItemRecipe.ItemRequirement requirement = itemRequirement;
                                                        KemetContentItemPopup.DoPickItem(ContentManager.I.Items,
                                                                                         contentItem =>
                                                                                         {
                                                                                             requirement.Item =
                                                                                                 new Item.ItemInstance(
                                                                                                     contentItem as Item,
                                                                                                     requirement.Item
                                                                                                         .Amount);
                                                                                         });
                                                    }
                                                    GUILayout.EndHorizontal();
                                                }
                                                GUILayout.EndVertical();
                                            }
                                            if (x == 3)
                                            {
                                                GUILayout.EndHorizontal();
                                                x = 0;
                                            }
                                            else
                                                x++;
                                        }
                                        if (x != 0)
                                            GUILayout.EndHorizontal();
                                    }
                                    GUILayout.EndVertical();

                                    ///result
                                    GUILayout.BeginVertical("box", GUILayout.MaxWidth(90));
                                    {
                                        if (recipe.Result.Item == null)
                                            recipe.Result = new Item.ItemInstance(ContentManager.I.Items[0], 1);
                                        //item result
                                        GUILayout.Label("Result");
                                        GUILayout.Label(recipe.Result.Item.name);
                                        recipe.Result.Amount = Mathf.Clamp(EditorGUILayout.IntField(recipe.Result.Amount), 1, 999);
                                        if (GUILayout.Button(recipe.Result.Item.Icon, GUILayout.MaxWidth(50),
                                                                 GUILayout.MaxHeight(50)))
                                        {
                                            KemetContentItemPopup.DoPickItem(ContentManager.I.Items,
                                                                             contentItem =>
                                                                             {
                                                                                 recipe.Result =
                                                                                     new Item.ItemInstance(
                                                                                         contentItem as Item,
                                                                                         recipe.Result.Amount);
                                                                             });
                                        }
                                    }
                                    GUILayout.EndVertical();
                                    //Side products
                                    GUILayout.BeginVertical("box");
                                    x = 0;
                                    recipe._foldOutSideProducts = EditorGUILayout.Foldout(recipe._foldOutSideProducts, "Side products");
                                    if (recipe._foldOutSideProducts)
                                    {
                                        GUILayout.BeginHorizontal();
                                        if (GUILayout.Button("Add"))
                                        {
                                            recipe.SideProducts.Add(new Item.ItemInstance(ContentManager.I.Items[0], 1));
                                        }
                                        GUILayout.EndHorizontal();
                                        for (int id = 0; id < recipe.SideProducts.Count; id++)
                                        {
                                            var sideProduct = recipe.SideProducts[id];
                                            if (x == 0)
                                                GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.BeginVertical("box", GUILayout.MaxWidth(150));
                                                {
                                                    GUILayout.BeginHorizontal();
                                                    if (GUILayout.Button("X"))
                                                    {
                                                        recipe.SideProducts.Remove(sideProduct);
                                                        if (id == 0)
                                                        {
                                                            GUILayout.EndHorizontal();
                                                            GUILayout.EndVertical();
                                                            continue;
                                                        }
                                                        id--;
                                                        sideProduct = recipe.SideProducts[id];
                                                    }
                                                    GUILayout.Label(sideProduct.Item.name);
                                                    GUILayout.EndHorizontal();
                                                    GUILayout.BeginHorizontal();
                                                    sideProduct.Amount =
                                                        Mathf.Clamp(
                                                            EditorGUILayout.IntField(sideProduct.Amount), 0,
                                                            999);
                                                    GUILayout.EndHorizontal();
                                                    
                                                    if (GUILayout.Button(sideProduct.Item.Icon,
                                                                         GUILayout.MaxWidth(50),
                                                                         GUILayout.MaxHeight(50)))
                                                    {
                                                        Item.ItemInstance instance = sideProduct;
                                                        KemetContentItemPopup.DoPickItem(ContentManager.I.Items,
                                                                                         contentItem =>
                                                                                         {
                                                                                             instance.Item = contentItem as Item;
                                                                                         });
                                                    }
                                                }
                                                GUILayout.EndVertical();
                                            }
                                            if (x == 3)
                                            {
                                                GUILayout.EndHorizontal();
                                                x = 0;
                                            }
                                            else
                                                x++;
                                        }
                                        if (x != 0)
                                            GUILayout.EndHorizontal();
                                    }
                                    GUILayout.EndVertical();
                                }
#endregion
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

        private static void Delete(TabType tabType, ContentItem item)
        {
            string mainPath = "Assets/Development/Libary/" + tabType.ToString() + "/";
            AssetDatabase.DeleteAsset(mainPath + item.name + ".prefab");
            var list = GetContentList(tabType);
            for (int i = 0; i < 100; i++)
            {
                list.Remove(null);
            }
            EditorUtility.SetDirty(ContentManager.I);
        }

        private static void Duplicate(TabType tabType, ContentItem item)
        {
            string mainPath = "Assets/Development/Libary/" + tabType.ToString() + "/";
            PrefabUtility.CreatePrefab(mainPath + item.name + "(2).prefab", item.gameObject);
            Scan(tabType);
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