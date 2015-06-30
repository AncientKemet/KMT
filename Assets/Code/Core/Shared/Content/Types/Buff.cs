using System.Collections.Generic;
using Code.Code.Libaries.Net;
#if SERVER
using Server.Model.Entities;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using Code.Libaries.Generic.Managers;

using UnityEngine;

namespace Shared.Content.Types
{
    public class BuffInstance
    {
        public Buff Buff;
        public float Duration;
        public int Stacks = 1;
        public float StartTime = -1;

        public BuffInstance(int index, float time, Buff buff, float duration)
        {
            Duration = duration;
            Buff = buff;
            StartTime = time;
            Index = index;
        }

        public bool Expired
        {
            get { return StartTime + Duration < Time.time; }
        }

        public int Index { get; set; }

        public void Serialize(ByteStream b)
        {
            b.AddShort(Buff.InContentManagerId);
            b.AddFloat2B(Duration);
            b.AddByte(Stacks);
            b.AddFloat4B(StartTime);
        }

        public void Deserialize(ByteStream b)
        {
            Buff = ContentManager.I.Buffs[b.GetUnsignedShort()];
            Duration = b.GetFloat2B();
            Stacks = b.GetUnsignedByte();
            StartTime = b.GetFloat4B();
        }
    }
    public class Buff : ScriptableObject
    {
        public enum Type
        {
            Possitive,Neutral,Negative
        }

        private int _inContentManagerId = -1;

        [Range(1, 10)] public int Priority = 10;
        public Type _type;
        public Texture2D Icon;
        
        [Multiline(5)]
        public string Description = "";
        public string Subtitle = "";

        public List<UnitAttributePropertySerializable> Attributes;

        public bool Stackable = false;
        public bool HaveSet = false;
        public List<Buff> Set;
        public Buff SetBuff;

        public int InContentManagerId
        {
            get
            {
                if (_inContentManagerId == -1)
                {
                    _inContentManagerId = ContentManager.I.Buffs.IndexOf(this);
                }
                return _inContentManagerId;
            }
        }
        
        public string GetDescribtion()
        {
            string s = "";

            foreach (var attribute in Attributes)
            {
                s += UnitAttributePropertySerializable.GetLabeledString(attribute.Property) + " " + UnitAttributePropertySerializable.GetLabeledString(attribute.Property,attribute.Value) + "\n";
            }

            return Description + "\n" + s;
        }
        
#if SERVER
        public virtual void ProgressUnit(ServerUnit unit)
        { }
#endif
#if UNITY_EDITOR
        [MenuItem("Kemet/Create/Buff/New")]
        private static void CreateBuff()
        {
            CreateBuff<Buff>();
        }

        public static void CreateBuff<T>() where T : Buff
        {
            var asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, "Assets/Development/Libary/Buffs/" + typeof(T).Name + ".asset");
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
        }
#endif
    }

}
