using System;

namespace Code.Libaries.Generic
{
    public class GenProperty<T> 
    {
        private T _t;

        public T Get
        {
            get
            {
                if(_t == null)
                {
                    //Debug.Log("Creating instance of: "+typeof(T));
                    _t = Activator.CreateInstance<T>();
                }
                return _t;
            }
        }
    }
}

