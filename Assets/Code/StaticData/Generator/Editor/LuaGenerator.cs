using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace StaticDataEditor
{
    public static class LuaGenerator
    {
        private static string TabString(int num)
        {
            return StaticData.Generator.StaticDataUtility.TabString(num);
        }
        private static string GetClassName(Type editorTypeDefinationClass)
        {
            return StaticData.Generator.StaticDataUtility.GetGenerateClassName(editorTypeDefinationClass, StaticData.Generator.Constant.Suffix, false);
        }

        public static void GenerateLuaFile(Type editorType, StreamWriter writer, int tabNum)
        {
            try
            {
                if (!editorType.IsSubclassOf(typeof(Template)) && !editorType.IsSubclassOf(typeof(Config)) && !editorType.IsSubclassOf(typeof(IStaticData)))
                {
                    return;
                }
                string tab = TabString(tabNum);
                string name = GetClassName(editorType);
                GenerateLuaDefination(editorType, writer, tabNum, "local ", name);
                if(Constant.canGenerateLuaBinaryRead)
                {
                    GenerateLuaCreateMethod(editorType, writer, tabNum);
                }
                if (Constant.canGenerateLuaXmlRead)
                {
                    GenerateLuaCreateXMLMethod(editorType, writer, tabNum);
                }
                writer.WriteLine(tab + string.Format("return {0}", name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                writer.Close();

            }
        }
        private static void GenerateLuaCreateXMLMethod(Type editorType, StreamWriter writer, int tabNum)
        {
            string className = GetClassName(editorType);
            string tab = TabString(tabNum);
            string tab1 = TabString(tabNum + 1);
            string tab2 = TabString(tabNum + 2);
            //string objName = string.Empty;
            if (editorType.IsSubclassOf(typeof(Template)))
            {
                writer.WriteLine(tab + string.Format("function {0}:CreateXML(reader, id, check)", className));
                writer.WriteLine(tab1 + "utility = require(\"StaticData.StaticDataUtility\")");
                writer.WriteLine(tab1 + (@"if check then"));
                writer.WriteLine(tab2 + (@"check = true"));
                writer.WriteLine(tab1 + (@"else"));
                writer.WriteLine(tab2 + (@"check = false"));
                writer.WriteLine(tab1 + (@"end"));

                writer.WriteLine(tab1 + (@"reader.BeginReadXML(check)"));
                writer.WriteLine(tab1 + @"if not reader.ReadTemplateXML('{0}', {1}) then", className, "id");
                writer.WriteLine(tab2 + (@"reader.EndReadXML()"));
                writer.WriteLine(tab2 + (@"return false"));
                writer.WriteLine(tab1 + (@"end"));

                //objName = "Template";
            }
            else if (editorType.IsSubclassOf(typeof(Config)))
            {
                writer.WriteLine(tab + string.Format("function {0}:CreateXML(reader,check)", className));
                writer.WriteLine(tab1 + "utility = require(\"StaticData.StaticDataUtility\")");
                writer.WriteLine(tab1 + (@"if check then"));
                writer.WriteLine(tab2 + (@"check = true"));
                writer.WriteLine(tab1 + (@"else"));
                writer.WriteLine(tab2 + (@"check = false"));
                writer.WriteLine(tab1 + (@"end"));

                writer.WriteLine(tab1 + (@"reader.BeginReadXML(check)"));
                writer.WriteLine(tab1 + @"if not reader.ReadConfigXML('{0}') then", className);
                writer.WriteLine(tab2 + (@"reader.EndReadXML()"));
                writer.WriteLine(tab2 + (@"return false"));
                writer.WriteLine(tab1 + (@"end"));
            }
            else if(editorType.IsSubclassOf(typeof(IStaticData)))
            {
                writer.WriteLine(tab + string.Format("function {0}:CreateXML(reader,check)", className));
                writer.WriteLine(tab1 + "utility = require(\"StaticData.StaticDataUtility\")");
                writer.WriteLine(tab1 + (@"if check then"));
                writer.WriteLine(tab2 + (@"check = true"));
                writer.WriteLine(tab1 + (@"else"));
                writer.WriteLine(tab2 + (@"check = false"));
                writer.WriteLine(tab1 + (@"end"));
            }
            else
            {
                StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("Error! base class not supported! {0}", editorType.Name));
            }
            writer.WriteLine(tab1 + "if not reader.ReadObject(\"self\", \"self\") then");
            writer.WriteLine(tab2 + (@"reader.EndReadXML()"));
            writer.WriteLine(tab2 + @"return false", className);
            writer.WriteLine(tab1 + @"end", className);

            _GenerateLuaCreateMethodFieldValueXML("self", editorType, writer, tabNum + 1);
            writer.WriteLine(tab1 + (@"reader.EndReadXML()"));
            writer.WriteLine(tab1 + (@"return true"));
            writer.WriteLine(tab + ("end"));

        }
        private static void _GenerateLuaCreateMethodFieldValueXML(string fieldName, Type editorType, StreamWriter writer, int tabNum, bool create = false)
        {
            string tab = TabString(tabNum);
            //string tab1 = TabString(tabNum + 1);
            if (create)
            {
                string v = "{}";

                switch (editorType.Name.Trim())
                {
                    case "Vector2":
                        v = "Vector2.New()";
                        break;
                    case "Vector3":
                        v = "Vector3.New()";
                        break;
                    case "Vector4":
                        v = "Vector4.New()";
                        break;

                    case "Color":
                        v = "Color.New()";
                        break;
                    case "Quaternion":
                        v = "Quaternion.New()";
                        break;
                    case "InternationalText":
                        v = "StaticData.InternationalText.New()";
                        break;


                    default:
                        break;
                }

                writer.WriteLine(tab + string.Format("{0} = {1}", fieldName, v));
            }
            //FieldInfo[] fields = StaticDataUtility.GetFieldsHierarchy(editorType);
            //StaticDataUtility.ResortID("ID", fields);
            //for (int i = 0; i < fields.Length; i++)
            //{
            //    FieldInfo field = fields[i];
            //    if (field != null)
            //    {
            //        _GenerateLuaCreateMethodFieldValueXML(field.FieldType, fieldName + "." + field.Name, writer, tabNum);
            //    }
            //}
            StaticData.Generator.StaticDataUtility.TranverseFields(editorType, (field) =>
            {
                _GenerateLuaCreateMethodFieldValueXML(field.FieldType, fieldName + "." + field.Name, writer, tabNum);
            });

        }
        private static void _GenerateLuaCreateMethodFieldValueXML(Type type, string fieldName, StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);
            //string tab1 = TabString(tabNum + 1);
            //string tab2 = TabString(tabNum + 2);

            string luaParam = fieldName.Replace("[", ".\"..tostring(").Replace("]", ")..\"");
            if (type.IsSubclassOf(typeof(IStaticData)))
            {
                writer.WriteLine(tab + "if reader.ReadObject(\"{0}\", \"self\") then", fieldName);
                _GenerateLuaCreateMethodFieldValueXML(fieldName, type, writer, tabNum + 1, true);
                writer.WriteLine(tab + "end");
            }
            else if (type.IsEnum)
            {
                writer.WriteLine(tab + string.Format("{0} = tonumber(reader.ReadFieldXML(\"{1}\" , \"self\"))", fieldName, luaParam));
            }
            else if (type.IsPrimitive || type == typeof(decimal))
            {
                string transFunction = "";
                string prefix = "";
                string suffix = "";

                switch (type.FullName)
                {
                    case "System.Boolean":
                        transFunction = "utility.ToBoolean";
                        prefix = "(";
                        suffix = ")";
                        break;
                    case "System.Char":
                    case "System.String":
                        transFunction = "utility.ToString";
                        prefix = "(";
                        suffix = ")";
                        break;
                    case "System.Decimal":

                    case "System.Byte":

                    case "System.Double":

                    case "System.Single":

                    case "System.Int32":

                    case "System.Int64":

                    case "System.SByte":

                    case "System.Int16":

                    case "System.UInt32":

                    case "System.UInt64":

                    case "System.UInt16":
                        transFunction = "tonumber";
                        prefix = "(";
                        suffix = ")";
                        break;


                    default:
                        transFunction = "utility.ToString";
                        prefix = "(";
                        suffix = ")";
                        break;

                }
                writer.WriteLine(tab + string.Format("{0} = {1}{2}reader.ReadFieldXML(\"{4}\" , \"self\"){3}", fieldName, transFunction, prefix, suffix, luaParam));

            }
            else if (type.IsValueType)
            {
                writer.WriteLine(tab + "if reader.ReadObject(\"{0}\", \"self\") then", fieldName);
                _GenerateLuaCreateMethodFieldValueXML(fieldName, type, writer, tabNum + 1, true);
                writer.WriteLine(tab + "end");
            }
            else if (type == typeof(string))
            {
                writer.WriteLine(tab + string.Format("{0} = reader.ReadFieldXML(\"{1}\" , \"self\")", fieldName, luaParam));
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                writer.WriteLine(tab + string.Format("local {0}num = reader.ReadArrayNum(\"{1}\" , \"self\")", fieldName.Replace(".", "").Replace("[", "").Replace("]", ""), luaParam));
                writer.WriteLine(tab + string.Format("{0} = {1}", fieldName, "{}"));

                writer.WriteLine(tab + string.Format("for {0}_i = 1, {0}num , 1 do", fieldName.Replace(".", "").Replace("[", "").Replace("]", "")));
                if (elementType.IsSubclassOf(typeof(IStaticData)))
                {
                    _GenerateLuaCreateMethodFieldValueXML(string.Format("{0}[{1}_i]", fieldName, fieldName.Replace(".", "").Replace("[", "").Replace("]", "")), elementType, writer, tabNum + 1, true);
                }
                else
                {
                    _GenerateLuaCreateMethodFieldValueXML(elementType, string.Format("{0}[{1}_i]", fieldName, fieldName.Replace(".", "").Replace("[", "").Replace("]", "")), writer, tabNum + 1);
                }
                writer.WriteLine(tab + ("end"));
            }
        }

        private static void GenerateLuaCreateMethod(Type editorType, StreamWriter writer, int tabNum)
        {
            string className = GetClassName(editorType);
            string tab = TabString(tabNum);
            string tab1 = TabString(tabNum + 1);
            string tab2 = TabString(tabNum + 2);
            string tab3 = TabString(tabNum + 3);
            //bool canEndread = false;
            if (editorType.IsSubclassOf(typeof(Template)))
            {
                writer.WriteLine(tab + string.Format("function {0}:Create(reader, id, check, isEnd)", className));
                writer.WriteLine(tab1 + "local utility = require(\"StaticData.StaticDataUtility\")");
                writer.WriteLine(tab1 + (@"if check then"));
                writer.WriteLine(tab2 + (@"check = true"));
                writer.WriteLine(tab1 + (@"else"));
                writer.WriteLine(tab2 + (@"check = false"));
                writer.WriteLine(tab1 + (@"end"));

                writer.WriteLine(tab1 + (@"if isEnd == nil then"));
                writer.WriteLine(tab2 + (@"isEnd = true"));
                writer.WriteLine(tab1 + (@"else"));
                writer.WriteLine(tab2 + (@"isEnd = false"));
                writer.WriteLine(tab1 + (@"end"));

                writer.WriteLine(tab1 + (@"if isEnd then"));

                writer.WriteLine(tab2 + (@"reader.BeginRead(check)"));
                writer.WriteLine(tab2 + @"if not reader.ReadTemplate('{0}', {1}) then", className, "id");
                writer.WriteLine(tab3 + (@"reader.EndRead()"));
                writer.WriteLine(tab3 + (@"return false"));
                writer.WriteLine(tab2 + (@"end"));

                writer.WriteLine(tab1 + (@"end"));

                //canEndread = true;
            }
            else if (editorType.IsSubclassOf(typeof(Config)))
            {
                writer.WriteLine(tab + string.Format("function {0}:Create(reader, check, isEnd)", className));
                writer.WriteLine(tab1 + "local utility = require(\"StaticData.StaticDataUtility\")");
                writer.WriteLine(tab1 + (@"if check then"));
                writer.WriteLine(tab2 + (@"check = true"));
                writer.WriteLine(tab1 + (@"else"));
                writer.WriteLine(tab2 + (@"check = false"));
                writer.WriteLine(tab1 + (@"end"));

                writer.WriteLine(tab1 + (@"if isEnd == nil then"));
                writer.WriteLine(tab2 + (@"isEnd = true"));
                writer.WriteLine(tab1 + (@"else"));
                writer.WriteLine(tab2 + (@"isEnd = false"));
                writer.WriteLine(tab1 + (@"end"));

                writer.WriteLine(tab1 + (@"if isEnd then"));

                writer.WriteLine(tab2 + (@"reader.BeginRead(check)"));
                writer.WriteLine(tab2 + @"if not reader.ReadConfig('{0}') then", className);
                writer.WriteLine(tab3 + (@"reader.EndRead()"));
                writer.WriteLine(tab3 + (@"return false"));
                writer.WriteLine(tab2 + (@"end"));

                writer.WriteLine(tab1 + (@"end"));

                //canEndread = true;
            }
            else if(editorType.IsSubclassOf(typeof(IStaticData)))
            {
                writer.WriteLine(tab + string.Format("function {0}:Create(reader, check, isEnd)", className));
                writer.WriteLine(tab1 + "local utility = require(\"StaticData.StaticDataUtility\")");
                writer.WriteLine(tab1 + @"if check then");
                writer.WriteLine(tab2 + @"check = true");
                writer.WriteLine(tab1 + @"else");
                writer.WriteLine(tab2 + @"check = false");
                writer.WriteLine(tab1 + @"end");

                writer.WriteLine(tab1 + (@"if isEnd == nil then"));
                writer.WriteLine(tab2 + (@"isEnd = true"));
                writer.WriteLine(tab1 + (@"else"));
                writer.WriteLine(tab2 + (@"isEnd = false"));
                writer.WriteLine(tab1 + (@"end"));

            }
            else
            {
                StaticData.Generator.StaticDataUtility.LogErrorAndThrowException(string.Format("Error! base class not supported! {0}", editorType.Name));
            }
            writer.WriteLine(tab1 + @"if not reader.ReadBoolean() then");
            writer.WriteLine(tab2 + @"reader.EndRead()");
            writer.WriteLine(tab2 + @"return false", className);
            writer.WriteLine(tab1 + @"end", className);


            _GenerateLuaCreateMethodFieldValue("self", editorType, writer, tabNum + 1);


            writer.WriteLine(tab1 + (@"if isEnd then"));
            writer.WriteLine(tab2 + (@"reader.EndRead()"));
            writer.WriteLine(tab1 + (@"end"));

            writer.WriteLine(tab1 + (@"return true"));
            writer.WriteLine(tab + ("end"));
        }
        private static void _GenerateLuaCreateMethodFieldValue(string fieldName, Type editorType,StreamWriter writer, int tabNum, bool create = false)
        {
            string tab = TabString(tabNum);
            if (create)
            {
                writer.WriteLine(tab + string.Format("{0} = {1}", fieldName, "{}"));
            }
            StaticData.Generator.StaticDataUtility.TranverseFields(editorType, (field) =>
            {
                _GenerateLuaCreateMethodFieldValue(field.FieldType, fieldName + "." + field.Name, writer, tabNum);
            });
        }
        private static void _GenerateLuaCreateMethodFieldValue(Type type, string fieldName,StreamWriter writer, int tabNum)
        {
            string tab = TabString(tabNum);
            string tab1 = TabString(tabNum + 1);
            string tab2 = TabString(tabNum + 2);

            if (type.IsSubclassOf(typeof(IStaticData)))
            {
                writer.WriteLine(tab + "local {0}name = reader.ReadString()", fieldName.Replace(".","").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab + "local {0}type = nil", fieldName.Replace(".", "").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab + "if {0}name and {0}name ~= \'\' then", fieldName.Replace(".", "").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab1 + "{0}type = require(\"StaticData.Generate.\" .. {0}name)", fieldName.Replace(".", "").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab + "end");
                writer.WriteLine(tab + "if {0}type then", fieldName.Replace(".", "").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab1 + "{0} = utility.Clone({1}type)", fieldName, fieldName.Replace(".", "").Replace("[", "").Replace("]", ""));
                writer.WriteLine(tab1 + "{0}:Create(reader, check, false, false)", fieldName);
                writer.WriteLine(tab + "else");
                writer.WriteLine(tab1 + "if reader.ReadBoolean() then");
                writer.WriteLine(tab2 + "{0} = {1}", fieldName, "{}");
                _GenerateLuaCreateMethodFieldValue(fieldName, type, writer, tabNum + 2);
                writer.WriteLine(tab1 + "end");
                writer.WriteLine(tab + "end");
            }
            else if (type.IsEnum)
            {
                writer.WriteLine(tab + string.Format("{0} = reader.ReadInt32()", fieldName));
            }
            else if (type.IsPrimitive || type == typeof(decimal))
            {
                string transFunction = "";
                string prefix = "";
                string suffix = "";

                switch (type.FullName)
                {
                    case "System.Boolean":
                        transFunction = "ReadBoolean";
                        prefix = "(";
                        suffix = ")";
                        break;
                    case "System.Char":
                        transFunction = "ReadChar";
                        prefix = "(";
                        suffix = ")";
                        break;
                    case "System.String":
                        transFunction = "ReadString";
                        prefix = "(";
                        suffix = ")";
                        break;
                    case "System.Decimal":
                        transFunction = "ReadDecimal";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.Byte":
                        transFunction = "ReadByte";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.Double":
                        transFunction = "ReadDouble";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.Single":
                        transFunction = "ReadSingle";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.Int32":
                        transFunction = "ReadInt32";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.Int64":
                        transFunction = "ReadInt64";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.SByte":
                        transFunction = "ReadSByte";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.Int16":
                        transFunction = "ReadInt16";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.UInt32":
                        transFunction = "ReadUInt32";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.UInt64":
                        transFunction = "ReadUInt64";
                        prefix = "(";
                        suffix = ")";
                        break;

                    case "System.UInt16":
                        transFunction = "ReadUInt16";
                        prefix = "(";
                        suffix = ")";
                        break;


                    default:
                        transFunction = "ReadString";
                        prefix = "(";
                        suffix = ")";
                        break;

                }
                writer.WriteLine(tab + string.Format("{0} = reader.{1}{2}{3}", fieldName, transFunction, prefix, suffix));

            }
            else if (type.IsValueType)
            {
                string v = "{}";

                switch(type.Name.Trim())
                {
                    case "Vector2":
                        v = "Vector2.New()";
                        break;
                    case "Vector3":
                        v = "Vector3.New()";
                        break;
                    case "Vector4":
                        v = "Vector4.New()";
                        break;

                    case "Color":
                        v = "Color.New()";
                        break;
                    case "Quaternion":
                        v = "Quaternion.New()";
                        break;
                    case "InternationalText":
                        v = "StaticData.InternationalText.New()";
                        break;



                    default:
                        break;
                }

                writer.WriteLine(tab + "{0} = {1}", fieldName, v);
                _GenerateLuaCreateMethodFieldValue(fieldName, type, writer, tabNum);
            }
            else if (type == typeof(string))
            {
                writer.WriteLine(tab + string.Format("{0} = reader.ReadString()", fieldName));
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                writer.WriteLine(tab + string.Format("local {0}num = reader.ReadInt32()", fieldName.Replace(".", "").Replace("[", "").Replace("]", "")));
                writer.WriteLine(tab + string.Format("{0} = {1}", fieldName, "{}"));
                //writer.WriteLine(tab + string.Format("if {0}num < {1} then", fieldName.Replace(".", "").Replace("[", "").Replace("]", ""), StaticData.Generator.Constant.StaticDataUpLimit.ToString()));

                writer.WriteLine(tab + string.Format("for {0}_i = 1, {0}num , 1 do", fieldName.Replace(".", "").Replace("[", "").Replace("]", "")));
                if (elementType.IsSubclassOf(typeof(IStaticData)))
                {
                    _GenerateLuaCreateMethodFieldValue(elementType, string.Format("{0}[{1}_i]", fieldName, fieldName.Replace(".", "").Replace("[", "").Replace("]", "")), writer, tabNum + 1);

                }
                else
                {
                    _GenerateLuaCreateMethodFieldValue(elementType, string.Format("{0}[{1}_i]", fieldName, fieldName.Replace(".", "").Replace("[", "").Replace("]", "")), writer, tabNum + 1);
                }

                //"StaticDataUtility.LogError(\"binary is bad!!! please rebuild it! 二进制文件已损坏,请重新生成!\")"
                writer.WriteLine(tab + ("end"));
                //writer.WriteLine(tab + ("elseif {0}num >= {1} then"), fieldName.Replace(".", "").Replace("[", "").Replace("]", ""), StaticData.Generator.Constant.StaticDataUpLimit.ToString());
                //writer.WriteLine(tab1 + ("error(\"binary is bad!!! please rebuild it! 二进制文件已损坏,请重新生成!\")"));
                //writer.WriteLine(tab + ("end"));

            }
        }

        private static string GetLuaDefaultValueString(Type type)
        {
            return StaticData.Generator.StaticDataUtility.GetLuaDefaultValueString(type);
        }

        private static void GenerateLuaDefination(Type editorType, StreamWriter writer, int tabNum, string prefix, string name, string suffix = "")
        {
            string tab = TabString(tabNum);
            string tab2 = TabString(tabNum + 1);
            //Assembly assembly = Assembly.GetAssembly(typeof(IStaticData));
            writer.WriteLine(tab + string.Format("{0}{1}=", prefix, name));
            writer.WriteLine(tab + ("{"));

            FieldInfo[] fields = StaticData.Generator.StaticDataUtility.GetFieldsHierarchy(editorType);
            StaticData.Generator.StaticDataUtility.ResortID("ID", fields);
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    if (field != null)
                    {
                        if (field.FieldType.IsEnum)
                        {
                            writer.WriteLine(tab2 + string.Format("{0}={2}{1}", field.Name, i == fields.Length - 1 ? "" : ",", GetLuaDefaultValueString(field.FieldType)));
                        }
                        else if (field.FieldType.IsPrimitive || field.FieldType == typeof(decimal))
                        {
                            writer.WriteLine(tab2 + string.Format("{0}={2}{1}", field.Name, i == fields.Length - 1 ? "" : ",", GetLuaDefaultValueString(field.FieldType)));
                        }
                        else if (field.FieldType.IsValueType)
                        {
                            writer.WriteLine(tab2 + string.Format("{0}={2}{1}", field.Name, i == fields.Length - 1 ? "" : ",", GetLuaDefaultValueString(field.FieldType)));
                        }
                        else if (field.FieldType == typeof(string))
                        {
                            writer.WriteLine(tab2 + string.Format("{0}={2}{1}", field.Name, i == fields.Length - 1 ? "" : ",", GetLuaDefaultValueString(field.FieldType)));
                        }
                        else if (field.FieldType.IsSubclassOf(typeof(IStaticData)))
                        {
                            GenerateLuaDefination(field.FieldType, writer, tabNum + 1, "", field.Name, i == fields.Length - 1 ? "" : ",");
                        }
                        else if (field.FieldType.IsArray)
                        {
                            writer.WriteLine(tab2 + string.Format("{0}={2}{1}", field.Name, i == fields.Length - 1 ? "" : ",", "{}"));
                        }
                    }
                }
            }


            writer.WriteLine(tab + ("}" + suffix));
        }



    }
}