/************************************************
 created : 2018.8
 author : caiming
************************************************/

using Code.Core.GameConf;
namespace StaticData
{
    public class Template : IStaticData, IConfig
    {
        public int ID;
        public string id;

        public string GetID()
        {
            return id;
        }
    }
}