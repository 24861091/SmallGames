/************************************************
 created : 2018.8
 author : caiming
************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace StaticData
{
    public static class StaticDataInterface
    {

#if RUNTIME_XML
        public static bool ReadConfigXML(string configName)
        {
            bool r = false;
            StaticDataUtility.TranverseElementXML(configName, (element, fileName) => 
            {
                if(element != null)
                {
                    if (_checkError)
                    {
                        if (_firstChild != null)
                        {
                            StaticDataUtility.LogError(string.Format("XML 文件里记录了重复的Template. element = {0} fileName = {1} ", element.InnerText, fileName));
                        }
                    }
                    _firstChild = element;
                    if (_firstChild == null)
                    {
                        StaticDataUtility.LogError(string.Format("XML 文件中数据是不完整的. element = {0}  fileName = {1} ", element.InnerText , fileName));
                        return false;
                    }
                    r = true;
                    return true;
                }
                return false;
            });

            return r;
        }
        public static bool ReadTemplateXML(string templateName, int id)
        {
            bool r = false;
            StaticDataUtility.TranverseElementXML(templateName, (element, fileName) =>
            {
                if (element != null && element.HasChildNodes)
                {
                    int m = element.ChildNodes.Count;
                    for (int j = 0; j < m; j++)
                    {
                        XmlNode node = element.ChildNodes[j];
                        if (node != null)
                        {
                            if (node.Name.Trim().ToLower() == "id")
                            {
                                if (node.InnerText.Trim() == id.ToString())
                                {
                                    if (_firstChild != null)
                                    {
                                        StaticDataUtility.LogError(string.Format("Xml 文件里记录了重复的Template. tempalate = {0} id = {1} file = {2}", templateName, id, fileName));
                                    }

                                    _firstChild = element;
                                    r = true;
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    StaticDataUtility.LogError(string.Format("此文件的一个模版数据为空. tempalate = {0} id = {1} file = {2}", templateName, id, fileName));
                }
                return false;
            });
            return r;
        }
        public static int[] FindTemplateIDsXML(string typeName)
        {
            List<int> ids = new List<int>();
            string file = "";

            try
            {
                StaticData.StaticDataUtility.TranverseElementXML(typeName, (element, fileName) =>
                {
                    file = fileName;
                    Type type = StaticDataSummary.GetType(typeName);
                    IStaticData data = null;
                    string name = element.Attributes["Type"].Value;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = element.Attributes["type"].Value;
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        name = type.Name;
                    }

                    StaticDataSummary.Read(element, out data, name);
                    Template template = data as Template;
                    if (template != null)
                    {
                        ids.Add(template.ID);
                    }
                    return false;
                });

                return ids.ToArray();

            }
            catch (Exception ex)
            {
                StaticData.StaticDataUtility.LogError("File Name = " + file + "\n" + ex.Message + "\n" + ex.StackTrace);
                return ids.ToArray();
            }
        }
        public static void EndReadXML()
        {
            _firstChild = null;
            _checkError = true;
        }
                
        public static void BeginReadXML(bool check = true)
        {
            _checkError = check;
        }
        public static string ReadFieldXML(string fieldName, string self)
        {
            XmlElement sub = GetFieldElement(fieldName, self);
            return sub == null ? string.Empty : sub.InnerText.Trim();
        }

        public static bool ReadObject(string name, string self)
        {
            return GetFieldElement(name, self) != null;
        }
        public static int ReadArrayNum(string fieldName, string self)
        {
            XmlElement sub = GetFieldElement(fieldName, self);
            return (sub == null) ? 0 : ((sub.ChildNodes == null) ? 0 : sub.ChildNodes.Count);
        }
        private static XmlElement GetFieldElement(string fieldName, string self)
        {
            string[] names = fieldName.Split('.');
            XmlElement father = null;
            if (_firstChild == null)
            {
                StaticDataUtility.LogError(string.Format("Error, _firstChild is not correct! fieldname = {0}", fieldName));
                return null;
            }
            if (names != null)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i];
                    XmlElement sub = null;
                    if (name == self)
                    {
                        sub = _firstChild;
                    }
                    else if (father != null)
                    {
                        int t = -1;
                        if (int.TryParse(name, out t))
                        {
                            t--;
                            sub = StaticDataUtility.GetSubElement(father, t);
                        }
                        else
                        {
                            sub = StaticDataUtility.GetSubElement(father, name);
                        }
                    }
                    if (sub == null)
                    {
                        StaticDataUtility.LogWarning(string.Format("Error, field name = {0} is not correct!", name));
                        father = null;
                        break;
                    }

                    father = sub;
                }
                if (father != null)
                {
                    return father;
                }
            }
            return null;

        }

#endif
        public static void BeginRead(bool check = true)
        {
            try
            {
                _checkError = check;
                if (!_reader.BeginRead())
                {
                    StaticDataUtility.LogError("Binary.raw 没有找到， 或者版本号不相同。");
                }
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            
        }
        public static void EndRead()
        {
            StaticDataUtility.EndLogRecord();
            if(_reader != null)
            {
                _reader.EndRead();
            }
            
            _checkError = true;
        }
        public static bool ReadTemplate(string typeName, int id)
        {
            try
            {
                StaticDataUtility.BeginLogRecord(string.Format("类型为{0}, id为{1}.  ", typeName, id));
                if (!_reader.ReadTemplate(typeName, id))
                {
                    if (_checkError)
                    {
                        StaticDataUtility.LogDesinerError("没找到!");
                    }
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }
        public static bool ReadConfig(string typeName)
        {
            try
            {
                StaticDataUtility.BeginLogRecord(string.Format("类型为{0}.  ", typeName));
                if (!_reader.ReadConfig(typeName))
                {
                    if (_checkError)
                    {
                        StaticDataUtility.LogDesinerError("配置文件 config typeName : " + typeName + " 在 binary.raw! 中没有找到");
                    }
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }
        public static bool ReadBoolean()
        {
            try
            {
                return _reader.ReadBoolean();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }
            return false;
        }
        public static byte ReadByte()
        {
            try
            {
                return _reader.ReadByte();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }
            return 0;
             
        }
        public static char ReadChar()
        {
            try
            {
                return _reader.ReadChar();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return '\0';
        }
        public static decimal ReadDecimal()
        {
            try
            {
                return _reader.ReadDecimal();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return 0;
        }
        public static double ReadDouble()
        {
            try
            {
                return _reader.ReadDouble();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }
            return 0;
        }
        public static short ReadInt16()
        {
            try
            {
                return _reader.ReadInt16();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }
            return 0;
        }
        public static int ReadInt32()
        {
            try
            {
                return _reader.ReadInt32();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return 0;
        }
        public static long ReadInt64()
        {
            try
            {
                return _reader.ReadInt64();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return 0;
        }
        public static sbyte ReadSByte()
        {
            try
            {
                return _reader.ReadSByte();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }
            return _reader.ReadSByte();
        }
        public static float ReadSingle()
        {
            try
            {
                return _reader.ReadSingle();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return 0f;
        }
        public static string ReadString()
        {
            try
            {
                return _reader.ReadString();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return string.Empty;
        }
        public static ushort ReadUInt16()
        {
            try
            {
                return _reader.ReadUInt16();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return 0;
        }
        public static uint ReadUInt32()
        {
            try
            {
                return _reader.ReadUInt32();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return 0;
        }
        public static ulong ReadUInt64()
        {
            try
            {
                return _reader.ReadUInt64();
            }
            catch (Exception ex)
            {
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                
            }

            return 0;
        }
        public static void Read(BinaryFileReader reader, out IStaticData obj, string typeName)
        {
            try
            {
                Facade.Instance.Read(reader, out obj, typeName);
            }
            catch (Exception ex)
            {
                obj = null;
                EndRead();
                StaticDataUtility.LogError(ex.Message + "\n" + ex.StackTrace);
            }
        }
        public static bool UseBinary()
        {
            return StaticData.StaticDataUtility.UseBinary;
        }
        public static bool IsLuaWeakReference()
        {
            return Constant.IsLuaWeakReference;
        }
        public static int[] FindTemplateIDs(string typeName)
        {
            return _reader.FindTemplateIDs(typeName);
        }
        public static BinaryFileReader GetReader()
        {
            return _reader;
        }
        public static void Clear()
        {
            if(_reader!= null)
            {
                _reader.Clear();
                _reader = null;
            }
        }
        public static void Init()
        {
            if(_reader == null)
            {
                _reader = new BinaryFileReader();
            }
        }
        private static bool _checkError = true;
        private static XmlElement _firstChild = null;
        private static BinaryFileReader _reader = new BinaryFileReader();
    }

}