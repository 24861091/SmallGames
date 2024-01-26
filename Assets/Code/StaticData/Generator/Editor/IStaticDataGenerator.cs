using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace StaticDataEditor.Generator
{
    public static class IStaticDataGenerator
    {
        private static Type GetBaseClass(Type type)
        {
            return type.BaseType;
        }
        private static string TabString(int num)
        {
            return StaticData.Generator.StaticDataUtility.TabString(num);
        }
        private static string GetClassName(Type editorTypeDefinationClass, bool nameSpace = true)
        {
            return StaticData.Generator.StaticDataUtility.GetGenerateClassName(editorTypeDefinationClass, StaticData.Generator.Constant.Suffix, nameSpace);
        }
        private static string GetCSDefaultValueString(Type type)
        {
            return StaticData.Generator.StaticDataUtility.GetCSDefaultValueString(type);
        }
        private static bool CheckFieldType(Type type)
        {
            if (type.Name.IndexOf(StaticData.Generator.Constant.Suffix) < 0)
            {
                return false;
            }

            object[] atts = type.GetCustomAttributes(typeof(StaticDataAttribute), false);
            if (atts != null)
            {
                for (int j = 0; j < atts.Length; j++)
                {
                    StaticDataAttribute attribute = atts[j] as StaticDataAttribute;
                    if(attribute != null)
                    {
                        if (attribute.Target != ExportTarget.All && attribute.Target != ExportTarget.CSharp && attribute.Target != ExportTarget.Lua)
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }

            }
            else
            {
                return false;
            }

            if(type.Namespace != "StaticData")
            {
                return false;
            }

            return true;
        }

        private static void GenerateCSFields(Type fieldType, string fieldName, object defaultValue, string suffix, StreamWriter writer, int tabNum)
        {
            suffix = suffix.Trim();
            string tab = TabString(tabNum);
            if (fieldType.IsEnum)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    writer.WriteLine(tab + "public {0}{2} {1} = null;", fieldType.FullName, fieldName, suffix);
                }
                else
                {
                    if (defaultValue == null)
                    {
                        writer.WriteLine(tab + "public {0} {1} = default({2});", fieldType.FullName, fieldName, (fieldType.FullName));
                    }
                    else
                    {
                        writer.WriteLine(tab + "public {0} {1} = {0}.{2};", fieldType.FullName, fieldName, defaultValue.ToString());
                    }
                }
            }
            else if (fieldType.IsPrimitive || fieldType == typeof(decimal))
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    writer.WriteLine(tab + "public {0}{2} {1} = null;", fieldType.FullName, fieldName, suffix);
                }
                else
                {
                    if (defaultValue == null)
                    {
                        writer.WriteLine(tab + "public {0} {1} = {2};", fieldType.FullName, fieldName, GetCSDefaultValueString(fieldType));
                    }
                    else
                    {
                        if (fieldType == typeof(float) || fieldType == typeof(double))
                        {
                            writer.WriteLine(tab + "public {0} {1} = {2};", fieldType.FullName, fieldName, defaultValue.ToString() + "f");
                        }
                        else if(fieldType == typeof(Boolean))
                        {
                            writer.WriteLine(tab + "public {0} {1} = {2};", fieldType.FullName, fieldName, defaultValue.ToString().ToLower());
                        }
                        else
                        {
                            writer.WriteLine(tab + "public {0} {1} = {2};", fieldType.FullName, fieldName, defaultValue.ToString());
                        }
                    }
                }
            }
            else if (fieldType.IsValueType)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    writer.WriteLine(tab + "public {0}{2} {1} = null;", fieldType.FullName, fieldName, suffix);
                }
                else
                {
                    writer.WriteLine(tab + "public {0} {1} = new {0}();", fieldType.FullName, fieldName);
                }
            }
            else if (fieldType.IsSubclassOf(typeof(IStaticData)))
            {
                if (!CheckFieldType(fieldType))
                {
                    StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("Error: type = {0} can not be defined as field for it ends with no 'Editor' Or output nothing ", fieldType.Name));
                }
                else
                {
                    if (!string.IsNullOrEmpty(suffix))
                    {
                        writer.WriteLine(tab + "public {0}{2} {1} = null;", GetClassName(fieldType), fieldName, suffix);
                    }
                    else
                    {
                        writer.WriteLine(tab + "public {0} {1} = null;", GetClassName(fieldType), fieldName);
                    }
                }


            }
            else if (fieldType.IsArray)
            {
                Type elementType = fieldType.GetElementType();
                GenerateCSFields(elementType, fieldName, defaultValue, suffix + "[]", writer, tabNum);
            }
            else if (fieldType == typeof(string))
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    writer.WriteLine(tab + "public {0}{2} {1} = null;", fieldType.FullName, fieldName, suffix);
                }
                else
                {
                    if (defaultValue == null)
                    {
                        writer.WriteLine(tab + "public {0} {1} = {2};", fieldType.FullName, fieldName, GetCSDefaultValueString(fieldType));
                    }
                    else
                    {
                        writer.WriteLine(tab + "public {0} {1} = {2};", fieldType.FullName, fieldName, "\"" + defaultValue.ToString() + "\"");
                    }
                }
            }
        }
        private static void GenerateCSFields(Type type, StreamWriter writer, int tabNum)
        {
            FieldInfo[] fields = null;
            fields = StaticData.Generator.StaticDataUtility.GetFieldsSelf(type); /*type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);*/
            if (fields != null)
            {

                string tab = TabString(tabNum);
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    DefaultValueAttribute attribute = StaticData.Generator.StaticDataUtility.GetAttribute<DefaultValueAttribute>(field);
                    object defaultValue = null;
                    if (attribute != null)
                    {
                        defaultValue = attribute.GetValue();
                    }
                    GenerateCSFields(field.FieldType, field.Name, defaultValue, "", writer, tabNum);
                }
            }
        }
        public static void GenerateCSClass(Type type, StreamWriter writer, int tabNum)
        {
            string className = GetClassName(type, false);
            string tab = TabString(tabNum);
            writer.WriteLine(tab + "public class " + className + (type == GetBaseClass(type) ? "" : " : " + GetClassName(GetBaseClass(type), true)));
            writer.WriteLine(tab + "{");
            tabNum++;
            GenerateCSFields(type, writer, tabNum);
            writer.WriteLine(tab + "}");
        }
        public static void GenerateCSFile(Type editorType, StreamWriter writer, int tabNum)
        {
            GenerateTitles(writer);
            writer.WriteLine("namespace StaticData");
            writer.WriteLine("{");
            tabNum++;
            IStaticDataGenerator.GenerateCSClass(editorType, writer, tabNum);
            writer.WriteLine("}");
        }
        private static void GenerateTitles(StreamWriter writer)
        {
            writer.WriteLine(@"/*");
            writer.WriteLine(@"This file is Auto-Generated.Please do not try to alter any part.");
            writer.WriteLine(@"*/");

        }
    }

}