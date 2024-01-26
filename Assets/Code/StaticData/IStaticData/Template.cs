/************************************************
 created : 2018.8
 author : caiming
************************************************/

using System.Collections;
using System.Collections.Generic;
using Code.Core.GameConf;
using UnityEngine;
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