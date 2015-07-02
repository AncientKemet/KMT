using System;
using System.Collections;
using System.Collections.Generic;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.Generic.Managers;
using UnityEngine;

namespace Code.Code.Libaries.Net
{
    public class ByteStream
    {
        private List<byte> stream;
        private int _offset = 0;

        public int Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public ByteStream()
        {
            stream = new List<byte>();
        }

        public ByteStream(int lenght)
        {
            stream = new List<byte>(lenght);
        }

        public ByteStream(byte[] bytes)
        {
            stream = new List<byte>(bytes.Length);
            foreach (var _b in bytes)
            {
                AddByte(_b);
            }
        }

        public void AddByte(int value)
        {
            AddByte(value, this._offset++);
        }

        public void AddByte(int value, int pos)
        {
            if (pos < stream.Count)
            {
                stream[pos] = (byte) value;
            }
            else
            {
                stream.Add((byte) value);
            }
        }

        public sbyte GetByte()
        {
            if (stream.Count > _offset)
            {
                return (sbyte) stream[_offset++];
            }
            else
            {
                //Debug.LogError("Readigngdsg streamsize: " + stream.Count + " off: " + Offset);
                Application.Quit();
                throw new Exception("Broken bytestream.");
            }
        }

        public void AddShort(int i)
        {
            AddByte(i >> 8);
            AddByte(i);
        }

        public void AddInt(int i)
        {
            AddByte(i >> 24);
            AddByte(i >> 16);
            AddByte(i >> 8);
            AddByte(i);
        }

        public void AddLong(long l)
        {
            AddByte((int) (l >> 56));
            AddByte((int) (l >> 48));
            AddByte((int) (l >> 40));
            AddByte((int) (l >> 32));
            AddByte((int) (l >> 24));
            AddByte((int) (l >> 16));
            AddByte((int) (l >> 8));
            AddByte((int) l);
        }

        public void AddString(String s)
        {
            if (s == null)
                s = "null";
            AddShort(s.Length);
            foreach (char c in s.ToCharArray())
            {
                AddByte((byte) c);
            }
        }


        public void AddFloat4B(float f)
        {
            AddInt((int) (f*1000));
        }

        public void AddFloat2B(float f)
        {
            AddShort((int) (f*10));
        }


        public byte GetUnsignedByte()
        {
            return (byte) GetByte();
        }

        public short GetShort()
        {
            short i = (short) ((GetUnsignedByte() << 8) + GetUnsignedByte());
            return i;
        }

        public int GetShortBe()
        {
            int i = GetUnsignedShortBe();
            if (i > 32767)
            {
                i -= 65536;
            }
            return i;
        }

        public ushort GetUnsignedShort()
        {
            return (ushort) ((GetUnsignedByte() << 8) + GetUnsignedByte());
        }

        public int GetUnsignedShortBe()
        {
            return GetUnsignedByte() + (GetUnsignedByte() << 8);
        }

        public int GetInt()
        {
            return (GetUnsignedByte() << 24) + (GetUnsignedByte() << 16) + (GetUnsignedByte() << 8) + GetUnsignedByte();
        }

        public int GetIntBe()
        {
            return GetUnsignedByte() + (GetUnsignedByte() << 8) + (GetUnsignedByte() << 16) + (GetUnsignedByte() << 24);
        }

        public long GetLong()
        {
            long l = GetInt() & 0xFFFFFFFF;
            long l1 = GetInt() & 0xFFFFFFFF;
            return (l << 32) + l1;
        }

        public String GetString()
        {
            String s = "";
            int size = GetUnsignedShort();
            for (int i = 0; i < size; i++)
            {
                s += (char) GetByte();
            }
            return s;
        }

        /// <summary>
        /// Max is 2147483647 / 2000 = 1073741,8235
        /// </summary>
        /// <returns></returns>
        public float GetFloat4B()
        {
            return (float) GetInt()/1000f;
        }

        /// <summary>
        /// Max is 65536 / 200 = 325.68
        /// </summary>
        /// <returns></returns>
        public float GetFloat2B()
        {
            return (float) GetShort()/10f;
        }

        public byte[] GetBuffer()
        {
            return stream.ToArray();
        }

        public int Length
        {
            get { return stream.Count; }
            set { stream.Capacity = value; }
        }

        private static byte[] GetBytesOfString(string str)
        {
            byte[] bytes = new byte[str.Length*sizeof (char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public void AddFlag(params bool[] updates)
        {
            var i = CreateBitMask(updates);
            AddByte(i);
        }

        private static int CreateBitMask(bool[] updates)
        {
            int i = 0;
            if (updates.Length >= 1)
            {
                i = (i | (updates[0] ? 0x01 : 0x00));
            }
            if (updates.Length >= 2)
            {
                i = (i | (updates[1] ? 0x02 : 0x00));
            }
            if (updates.Length >= 3)
            {
                i = (i | (updates[2] ? 0x04 : 0x00));
            }
            if (updates.Length >= 4)
            {
                i = (i | (updates[3] ? 0x08 : 0x00));
            }
            if (updates.Length >= 5)
            {
                i = (i | (updates[4] ? 0x10 : 0x00));
            }
            if (updates.Length >= 6)
            {
                i = (i | (updates[5] ? 0x20 : 0x00));
            }
            if (updates.Length >= 7)
            {
                i = (i | (updates[6] ? 0x40 : 0x00));
            }
            if (updates.Length >= 8)
            {
                i = (i | (updates[7] ? 0x80 : 0x00));
            }
            return i;
        }
        private static int CreateBitMask(BitArray updates)
        {
            int i = 0;
            if (updates.Length >= 1)
            {
                i = (i | (updates[0] ? 0x01 : 0x00));
            }
            if (updates.Length >= 2)
            {
                i = (i | (updates[1] ? 0x02 : 0x00));
            }
            if (updates.Length >= 3)
            {
                i = (i | (updates[2] ? 0x04 : 0x00));
            }
            if (updates.Length >= 4)
            {
                i = (i | (updates[3] ? 0x08 : 0x00));
            }
            if (updates.Length >= 5)
            {
                i = (i | (updates[4] ? 0x10 : 0x00));
            }
            if (updates.Length >= 6)
            {
                i = (i | (updates[5] ? 0x20 : 0x00));
            }
            if (updates.Length >= 7)
            {
                i = (i | (updates[6] ? 0x40 : 0x00));
            }
            if (updates.Length >= 8)
            {
                i = (i | (updates[7] ? 0x80 : 0x00));
            }
            return i;
        }

        public void AddBytes(byte[] p)
        {
            stream.AddRange(p);
        }

        public byte[] GetSubBuffer(int lenght)
        {
            byte[] bytes = new byte[lenght];
            for (int i = 0; i < lenght; i++)
            {
                bytes[i] = (byte) GetByte();
            }
            return bytes;
        }

        public void AddPosition6B(Vector3 Position)
        {
            AddFloat2B(Position.x);
            AddFloat2B(Position.y);
            AddFloat2B(Position.z);
        }

        public Vector3 GetPosition6B()
        {
            return new Vector3(GetFloat2B(), GetFloat2B(), GetFloat2B());
        }

        public void AddBitArray(BitArray bitArray)
        {
            var bytes = new byte[1];
            bitArray.CopyTo(bytes, 0);
            AddByte(bytes[0]);
        }

        public BitArray GetBitArray()
        {
            return new BitArray(new[] {(int)GetByte()});
        }

        public int GetSize()
        {
            return stream.Count;
        }

        public void AddPosition12B(Vector3 Position)
        {
            AddFloat4B(Position.x);
            AddFloat4B(Position.y);
            AddFloat4B(Position.z);
        }

        public Vector3 GetPosition12B()
        {
            return new Vector3(GetFloat4B(), GetFloat4B(), GetFloat4B());
        }

        public void AddEqItem(EquipmentItem item)
        {
            if (item == null)
                AddShort(-1);
            else
                AddShort(item.Item.InContentManagerIndex);
        }

        public void GetEqItem(ref EquipmentItem item)
        {
            int id = GetShort();
            item = id == -1 ? null : ContentManager.I.Items[id].GetComponent<EquipmentItem>();
        }

        public void AddAngle2B(float angleInDegrees)
        {
            AddShort((int) (angleInDegrees*10));
        }

        public float GetAngle2B()
        {
            return GetShort()/10f;
        }

        public void AddIdMask2B(int id, BitArray mask)
        {
            int s = CreateBitMask(mask) << (12);
            AddShort(s | id);
        }

        public BitArray GetIdMask2BMASK(int shortValue)
        {
            return new BitArray(new[] {(shortValue & 61440) >> 12});
        }

        public int GetIdMask2BID(int shortValue)
        {
            return shortValue & 4095;
        }
    }
}

