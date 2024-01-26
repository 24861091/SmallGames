using System;
using System.Collections.Generic;
using System.IO;
using Code.Core.Utils;
using GameLib.Main;

namespace Code.Core.GameConf
{
    public static class GameConf
    {
        private static Dictionary<Type, object> _entries = new();
        private static Dictionary<Type, object> _datas = new();
        
        public static T Get<T>(string id) where T : StaticData.Template
        {
            var dic = Get<T>();
            if (dic.TryGetValue(id, out var value))
            {
                return value;
            }

            return null;
        }
        
        public static Dictionary<string, T> Get<T>() where T : StaticData.Template
        {
            var type = typeof(T);
            if (_datas.TryGetValue(type, out var value))
            {
                if (value is Dictionary<string, T> list)
                    return list;
        
                throw new Exception($"List of ({type.FullName}) expected, got {value.GetType().FullName}");
            }

            var entry = GetEntries<T>();
            var ret = new Dictionary<string, T>();
            foreach (var template in entry)
            {
                if (template is IConfig config)
                {
                    ret.Add(config.GetID(), template);
                }
            }
            _datas.Add(type, ret);
            return ret;
        }

        public static List<T> GetEntries<T>() where T : StaticData.Template
        {
            
            var type = typeof(T);
            if (_entries.TryGetValue(type, out var value))
            {
                if(value is List<T> entry)
                    return entry;
                throw new Exception($"GetEntries List of ({type.FullName}) expected, got {value.GetType().FullName}");
            }
            
            var array = StaticData.TemplateManager.Instance.GetTemplates(type);
            var list = new List<T>();
            foreach (var element in array)
            {
                list.Add(element as T);
            }
            _entries.Add(type, list);
            return list;
        }

        // public static bool IsDropReward(string rewardID)
        // {
        //     if (!Get<RewardConfig>().TryGetValue(rewardID, out var rewardConfig))
        //         throw new Exception($"RewardConfig {rewardID} not found");
        //
        //     return rewardConfig.res_ids == null || rewardConfig.res_ids.Length == 0;
        // }
        //
        // public static ResourceItem[] GetRewards(string rewardID)
        // {
        //     if (!Get<RewardConfig>().TryGetValue(rewardID, out var rewardConfig))
        //         throw new Exception($"RewardConfig {rewardID} not found");
        //
        //     if (rewardConfig.res_ids == null || rewardConfig.res_ids.Length == 0)
        //     {
        //         CoreLog.LogWarning($"RewardConfig {rewardConfig} is drop reward");
        //         return null;
        //     }
        //
        //     var ret = new ResourceItem[rewardConfig.res_ids.Length];
        //     for (int i = 0; i < rewardConfig.res_ids.Length; i++)
        //     {
        //         ret[i] = new ResourceItem(rewardConfig.res_ids[i], rewardConfig.res_amounts[i],0);
        //     }
        //
        //     return ret;
        // }
        //
        // public static pb.reward.Reward GetReward(string rewardID)
        // {
        //     ResourceItem[] resourceItems = GetRewards(rewardID);
        //     pb.reward.Reward reward = new pb.reward.Reward();
        //     reward.ID = rewardID;
        //     reward.Resources = ResourceItem.ToServer(resourceItems);
        //     return reward;
        // }
    }
}