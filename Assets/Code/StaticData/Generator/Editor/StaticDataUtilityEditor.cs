using StaticData;
using System;

namespace StaticDataEditor.Generator
{
    public static partial class StaticDataUtilityEditor
    {
        public static ExportTarget GetExportTarget(Type type)
        {
            object[] atts = type.GetCustomAttributes(typeof(StaticDataAttribute), false);
            bool csharp = false;
            bool lua = false;
            if (atts != null)
            {
                for (int j = 0; j < atts.Length; j++)
                {
                    StaticDataAttribute attribute = atts[j] as StaticDataAttribute;
                    if (attribute.Target == ExportTarget.CSharp)
                    {
                        csharp = true;
                        if (lua)
                        {
                            break;
                        }
                    }

                    else if (attribute.Target == ExportTarget.Lua)
                    {
                        lua = true;
                        if (csharp)
                        {
                            break;
                        }
                    }
                    else if (attribute.Target == ExportTarget.All)
                    {
                        lua = true;
                        csharp = true;
                        break;
                    }
                }
            }
            if (csharp && lua)
            {
                return ExportTarget.All;
            }
            if (csharp)
            {
                return ExportTarget.CSharp;
            }
            if (lua)
            {
                return ExportTarget.Lua;
            }

            return ExportTarget.None;
        }
    }
}