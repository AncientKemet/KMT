#if SERVER
using System;

namespace Server.IO
{
    public interface IDataProvider
    {
        void GetData(string dataPath, Action<bool, string> onFinish);
        void SetData(string dataPath, string data, Action<bool> onFinish);
        void AppendData(string dataPath, string data, Action<bool> onFinish);
        void ReplaceData(string dataPath, string oldValue, string newValue, Action<bool> onFinish);
    }
}
#endif
