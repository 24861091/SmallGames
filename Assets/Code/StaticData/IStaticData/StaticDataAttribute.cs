using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace StaticData
{
    public class StaticDataAttribute : Attribute
    {
        public StaticDataAttribute(ExportTarget target)
        {
            Target = target;
        }
        public ExportTarget Target { get; set; }
    }
    public class AllowEmptyAttribute : Attribute
    {

    }
    public class DefaultValueAttribute : Attribute
    {
        private object _value;
        public DefaultValueAttribute(object v)
        {
            _value = v;
        }
        public object GetValue()
        {
            return _value;
        }
    }

    public enum ExportTarget
    {
        None = 0,
        Lua = 1,
        CSharp = 2,
        All = 99,
    }

}