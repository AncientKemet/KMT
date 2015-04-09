using System;
using System.Threading.Tasks;
using Parse;

namespace Server.IO.File
{
    public class ParseDataProvider : IDataProvider {


        public void GetData(string dataPath, Action<bool, string> onFinish)
        {
            var query = ParseObject.GetQuery("DataServer");
            query.GetAsync(dataPath).ContinueWith(task =>
            {
                try
                {
                    onFinish(true, task.Result.Get<string>("s"));
                }
                catch (Exception e)
                {
                    onFinish(false, "");
                }
            });
        }

        public void SetData(string dataPath, string data, Action<bool> onFinish)
        {
            var query = new ParseObject("DataServer");
            query.SaveAsync().ContinueWith(task =>
            {
                query["s"] = data;
                query.SaveAsync();
            });
        }

        public void AppendData(string dataPath, string data, Action<bool> onFinish)
        {
            GetData(dataPath, (b, s) => SetData(dataPath, s + data, onFinish));
        }

        public void ReplaceData(string dataPath, string oldValue, string newValue, Action<bool> onFinish)
        {
            GetData(dataPath, (b, s) =>
            {
                string ns = s.Replace(oldValue, newValue);
                SetData(dataPath, ns, onFinish);
            });
        }
    }
}
