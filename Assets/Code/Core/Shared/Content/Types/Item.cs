#if UNITY_EDITOR
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Client.Units;
using Code.Core.Shared.Content;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.Generic.Managers;
using UnityEditor;
using UnityEngine;

namespace Shared.Content.Types
{
    [Serializable]
    [ExecuteInEditMode]
    public class Item : ContentItem

    {
        [Multiline(5)]
        public string Description = "";
        public string Subtitle = "";
        
        public string[] ActionsStrings;

        [Range(0f, 20f)]
        public float Weight;

        public bool Tradable = true;
        [Range(1, 1000000)] public int Value = 1;
        
        public bool Stackable = false;

        public int MaxStacks = 1;

        public Texture2D Icon;
        
        private int _inContentManagerIndex = -1;
        private EquipmentItem _eq;

        public int InContentManagerIndex
        {
            get
            {
                if (_inContentManagerIndex == -1)
                {
                    _inContentManagerIndex = ContentManager.I.Items.IndexOf(this);
                }
                /*if (_inContentManagerIndex == -1)
                {
                    for (int i = 0; i < ContentManager.I.Items.Count; i++)
                    {
                        if (ContentManager.I.Items[i] != null)
                        if (GUID == ContentManager.I.Items[i].GUID)
                        {
                            _inContentManagerIndex = i;
                            break;
                        }
                    }
                }*/
                return _inContentManagerIndex;
            }
        }

        private void Start()
        {
            name = name.Replace("(Clone)", "");
        }

        public void EnterUnit(PlayerUnit unit)
        {
            StartCoroutine(_enterUnit(unit.transform,Vector3.up * 1));
        }

        private IEnumerator _enterUnit(Transform u, Vector3 offset)
        {
            float startTime = Time.realtimeSinceStartup;
            float time = 0.5f;

            Vector3 start = transform.position;
            Vector3 startScale = transform.localScale;

            float t = 0.001f;

            while (Time.realtimeSinceStartup - startTime < time)
            {
                yield return new WaitForEndOfFrame();

                t += Time.deltaTime;

                if (Time.realtimeSinceStartup - startTime < time)
                {
                    transform.position = start*(1f - t/time) + (u.transform.position+offset) * (t/time);
                    transform.localScale = startScale * (1f - t / time) + (Vector3.zero) * (t / time);
                }
            }

            ContentManager.I.CreateEffect(0, transform.position);

            Destroy(gameObject);
        }

        public string GetDescribtion()
        {
            string s = "";

            if(EQ != null)
            foreach (var attribute in EQ.Attributes)
            {
                s += UnitAttributePropertySerializable.GetLabeledString(attribute.Property, attribute.Value) + "\n";
            }

            return Description + "\n" + s;
        }

        public EquipmentItem EQ
        {
            get
            {
                if(_eq == null)
                    _eq = GetComponent<EquipmentItem>();
                return _eq;
            }
        }

        [Serializable]
        public class ItemInstance
        {
            [SerializeField]
            private Item _item;
            [SerializeField]
            private int _amount;

            public Item Item
            {
                get { return _item; }
                private set { _item = value; }
            }

            public int Amount
            {
                get { return _amount; }
                set { _amount = value; }
            }

            public ItemInstance(Item _item, int _amount)
            {
                Item = _item;
                Amount = _amount;
            }

            public ItemInstance(Item item)
            {
                Item = item;
                Amount = 1;
            }
        }

        public static ItemInstance CopperCoins(int amount)
        {
            return new ItemInstance(ContentManager.I.Items[0], amount);
        }

        public static ItemInstance BronzeCoins(int amount)
        {
            return new ItemInstance(ContentManager.I.Items[1], amount);
        }

        public static ItemInstance MildBronzeCoins(int amount)
        {
            return new ItemInstance(ContentManager.I.Items[2], amount);
        }

        public static ItemInstance TinCoins(int amount)
        {
            return new ItemInstance(ContentManager.I.Items[3], amount);
        }

        public static ItemInstance SilverCoins(int amount)
        {
            return new ItemInstance(ContentManager.I.Items[4], amount);
        }

        public enum CoinValue
        {
            Copper = 1,
            Bronze = 2,
            MildBronze = 5,
            Tin = 10,
            Silver = 200,
            Electrum = 5000,
            Gold = 10000
        }

#if UNITY_EDITOR
        public void CreateIcon()
        {
            var tex = AssetPreview.GetAssetPreview(gameObject);
            Thread.Sleep(50);
            for (int i = 0; i < 20; i++)
            {
                if(AssetPreview.IsLoadingAssetPreview(gameObject.GetInstanceID()))
                    Thread.Sleep(100);
            }
            if (tex != null)
            {
                byte[] data = tex.EncodeToPNG();
                string pathAndName = Application.dataPath + "/Development/Libary/Items/Icons/" + name + ".png";
                File.WriteAllBytes(pathAndName, data);
                AssetDatabase.ImportAsset("Assets/Development/Libary/Items/Icons/" + name + ".png");
                Icon =
                    (Texture2D)
                        AssetDatabase.LoadAssetAtPath("Assets/Development/Libary/Items/Icons/" + name + ".png",
                            typeof (Texture2D));
            }
            EditorUtility.SetDirty(gameObject);
        }
#endif
    }

    [RequireComponent(typeof(Item))]
    public class ItemExtension : MonoBehaviour
    {
        public Item Item { get { return GetComponent<Item>(); } }
    }

}
