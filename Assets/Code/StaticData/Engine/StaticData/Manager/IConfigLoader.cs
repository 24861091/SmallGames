using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
    public interface IConfigLoader
    {
        Config LoadConfig(string key, Func<Config, bool> Add, bool checkError = true);
    }
}