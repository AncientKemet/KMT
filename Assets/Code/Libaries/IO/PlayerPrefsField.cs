using System;
using UnityEngine;

namespace Code.Libaries.IO
{
    public class PPFInt
    {
        public string Key;

        private int _val = -9999999;

        public int Value
        {
            get
            {
                if (_val == -9999999)
                    _val = PlayerPrefs.GetInt(Key);

                return _val;
            }
            set
            {
                _val = value;
                PlayerPrefs.SetInt(Key, value);
            }
        }

        public PPFInt(string key)
        {
            this.Key = key;
        }
    }

    public class PPFFloat
    {
        public string Key;

        private float _val = -9999999;

        public float Value
        {
            get
            {
                if (_val <= -999999)
                    _val = PlayerPrefs.GetFloat(Key);

                return _val;
            }
            set
            {
                _val = value;
                PlayerPrefs.SetFloat(Key, value);
            }
        }

        public PPFFloat(string key)
        {
            this.Key = key;
        }
    }

    public class PPFString
    {
        public string Key;

        private string _val = null;

        public string Value
        {
            get
            {
                if (_val == null)
                    _val = PlayerPrefs.GetString(Key);

                return _val;
            }
            set
            {
                _val = value;
                PlayerPrefs.SetString(Key, value);
            }
        }

        public PPFString(string key)
        {
            this.Key = key;
        }
    }

    public class PPFEnum<T> where T : struct, IComparable
    {
        public string Key;

        private int initialLoad = -1;
        private T _val;

        public T Value
        {
            get
            {
                if (initialLoad == -1)
                {
                    initialLoad = 0;
                    try
                    {
                        _val = (T) Enum.Parse(typeof (T), PlayerPrefs.GetString(Key));
                    }
                    catch(ArgumentException exception)
                    { }
                }

                return _val;
            }
            set
            {
                _val = value;
                PlayerPrefs.SetString(Key, value.ToString());
            }
        }

        public PPFEnum(string key)
        {
            this.Key = key;
        }
    }
}
