using System;
using System.Collections.Generic;
using System.Text;
using Core.Serialization;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public partial class ScriptMono 
{
    // [FormerlySerializedAs("scriptMonoTypeFullName")]
    // public string ilTypeFullName;

    public CompBindingDict compBindings;

    public T GetCompBindingValue<T>(string key) where T : UnityEngine.Object
    {
        if (compBindings.TryGetValue(key, out var comp))
        {
            if (comp != null)
                return (T)comp;
        }

        return null;
    }

    #region 基础类型引用绑定

#if USE_PRIMITIVE_BINDING
        public PrimitiveBindingDict primitiveBindings;

        public int GetIntBindingValue(string key)
        {
            if (primitiveBindings.TryGetValue(key, out var value))
            {
                return int.Parse(value.Value);
            }

            return 0;
        }
        public float GetFloatBindingValue(string key)
        {
            if (primitiveBindings.TryGetValue(key, out var value))
            {
                return float.Parse(value.Value);
            }

            return 0f;
        }

        public bool GetBooleanBindingValue(string key)
        {
            if (primitiveBindings.TryGetValue(key, out var value))
            {
                return Boolean.Parse(value.Value);
            }

            return false;
        }
        
        public string GetStringBindingValue(string key)
        {
            if (primitiveBindings.TryGetValue(key, out var value))
            {
                return value.Value;
            }

            return null;
        }

        public Vector3 GetVector3BindingValue(string key)
        {
            if (primitiveBindings.TryGetValue(key, out var value))
            {
                var splits = value.Value.Split(',');
                var vec = new Vector3(float.Parse(splits[0]), float.Parse(splits[1]),
                    splits.Length < 3 ? 0f : float.Parse(splits[2]));
                return vec;
            }

            return Vector3.zero;
        }
#endif

    #endregion

    #region Editor代码

#if UNITY_EDITOR || true

#if USE_PRIMITIVE_BINDING


        private string GetFuncName(PrimitiveType valueType)
        {
            switch (valueType)
            {
                case PrimitiveType.@int:
                    return nameof(GetIntBindingValue);
                // case PrimitiveType.@string:
                    // return nameof(GetStringBindingValue);
                case PrimitiveType.@float:
                    return nameof(GetFloatBindingValue);
                // case PrimitiveType.Vector3:
                    // return nameof(GetVector3BindingValue);
                case PrimitiveType.@bool:
                    return nameof(GetBooleanBindingValue);
                default:
                    throw new Exception();
            }
        }
#endif
    [NonSerialized] [ShowInInspector] public bool IsChildClass;

    [Button()]
    public void GenerateFieldsByKey()
    {
        GenerateFields(true);
    }

    [Button()]
    public void GenerateFieldsByFind()
    {
        GenerateFields(false);
    }

    public void GenerateFields(bool useKeys)
    {
        var type = this.GetType();
        var className = this.GetType().Name;
        var @namespace = type.Namespace;
        var folderPath = "Assets/Code/UI/Generated";
        var tab = "    ";
        var sbFieldDeclare = new StringBuilder();
        var sbFieldGetCalls = new StringBuilder();

        if (compBindings != null && compBindings.Count > 0)
        {
            foreach (var item in compBindings)
            {
                sbFieldDeclare.AppendLine(
                    $"{tab}{tab}public {(IsChildClass ? "override" : "virtual")} {item.Value.GetType().FullName} {item.Key} {{ get; set; }}");
                if (useKeys)
                {
                    sbFieldGetCalls.AppendLine(
                        $"{tab}{tab}{tab}{item.Key} = GetCompBindingValue<{item.Value.GetType().FullName}>(\"{item.Key}\");");
                }
                else
                {
                    var method = "";
                    if (item.Value.GetType() == typeof(GameObject))
                    {
                        method = ".gameObject";
                    }
                    else if (item.Value.GetType() == typeof(Transform))
                    {
                        method = "";
                    }
                    else
                    {
                        method = $".GetComponent<{item.Value.GetType().FullName}>()";
                    }

                    sbFieldGetCalls.AppendLine(
                        $"{tab}{tab}{tab}{item.Key} = transform.Find(\"{GetObjectPath(item.Value)}\"){method};");
                }

            }
        }
#if USE_PRIMITIVE_BINDING
            if (primitiveBindings != null && primitiveBindings.Count > 0)
            {
                foreach (var item in primitiveBindings)
                {
                    sbFieldDeclare.AppendLine($"        public {nameof(item.Value.Type)} {item.Key} {{ get; set; }}");
                    sbFuncCalls.AppendLine($"            {item.Key} = {GetFuncName(item.Value.Type)}(\"{item.Key}\");");
                }
            }
#endif

        var output = $@"
namespace {@namespace}
{{
    public partial class {className}
    {{
{sbFieldDeclare.ToString()}
        public {(IsChildClass ? "override" : "virtual")} void InitializeBindingFields()
        {{
{sbFieldGetCalls.ToString()}
        }}
    }}  
}}
";
        Debug.Log(folderPath);
        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);
        var filePath = System.IO.Path.Combine(folderPath, $"{className}.Fields.cs");
        System.IO.File.WriteAllText(filePath, output);
        Debug.Log($"Wrote C# file to {filePath}");
        
        AssetDatabase.Refresh();
    }

    string GetObjectPath(UnityEngine.Object o)
    {
        GameObject go = null;
        if (o is UnityEngine.Component c)
        {
            go = c.gameObject;
        }
        else if (o is GameObject)
        {
            go = o as GameObject;
        }

        if (!go) return "";

        var strings = new List<string>();
        strings.Add(go.name);
        while (go.transform.parent)
        {
            go = go.transform.parent.gameObject;

            if (go == gameObject)
                break;

            strings.Add(go.name);
        }

        strings.Reverse();
        return string.Join("/", strings);
    }

#endif

    #endregion
}

}
