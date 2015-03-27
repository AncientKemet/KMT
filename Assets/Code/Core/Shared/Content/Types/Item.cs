﻿#if UNITY_EDITOR
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

        public List<UnitAttributePropertySerializable> Attributes;


        public string[] ActionsStrings;

        [Range(0f, 20f)]
        public float Weight;

        public bool Tradable = true;
        
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

            foreach (var attribute in Attributes)
            {
                s += UnitAttributePropertySerializable.GetLabeledString(attribute) + "\n";
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

        [SerializeField]
        public class ItemInstance
        {
            public Item Item { get; private set; }
            public int Amount { get; set; }

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
        }
#endif
    }

    [RequireComponent(typeof(Item))]
    public class ItemExtension : MonoBehaviour
    {
        public Item Item { get { return GetComponent<Item>(); } }
    }

}
