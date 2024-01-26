using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StaticData
{
    public interface ITemplateLoader
    {
        Template LoadTemplate(string key, int id, Func<Template, bool> Add, bool checkError = true);
        Template[] LoadTemplates(string key, Func<Template, bool> Add, bool checkError = true);
    }
}