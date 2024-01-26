using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace StaticDataEditor
{
    public class CreateXmlWindow : EditorWindow
    {
        private string _xmlPath = "";
        private Type _staticType;
        private string _className = "";
        private string _beforeClassName = "";
        private int _createButtonNumberPerLine = 3;
        private string _namespace = "StaticData";
        List<string> _list = new List<string>();

        public CreateXmlWindow()
        {
            this.titleContent.text = "Create XML";
        }

        [MenuItem("StaticData/Create XML")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CreateXmlWindow));
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            if (GUILayout.Button("Select Target Xml Path"))
            {
                _xmlPath = EditorUtility.OpenFolderPanel("Select Target Xml Path", _xmlPath, "");
            }
            if(string.IsNullOrEmpty(_xmlPath))
            {
                _xmlPath = StaticDataEditor.FacadeEditor.Instance.XMLStaticDataPaths[0];
            }
            _xmlPath = _xmlPath.Replace(@"/", @"\");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Xml Path", GUILayout.MaxWidth(144));
            EditorGUILayout.LabelField(_xmlPath);
            GUILayout.EndHorizontal();

            _className = EditorGUILayout.TextField("Class Name", _beforeClassName);
            if(_className != _beforeClassName)
            {
                _beforeClassName = _className;
                CreateTips();
            }
            int n = _list.Count;

            for (int i = 0; i < n; i++)
            {
                if (i % _createButtonNumberPerLine == 0)
                {
                    GUILayout.BeginHorizontal();
                }


                if (GUILayout.Button(_list[i]))
                {
                    CreateXML(_list[i]);
                }


                if (i % _createButtonNumberPerLine == _createButtonNumberPerLine - 1 || i == n - 1)
                {
                    GUILayout.EndHorizontal();
                }

            }
            
            GUILayout.EndVertical();
        }
        
        private void CreateTips()
        {
            _list.Clear();
            Tranverse(Assembly.GetExecutingAssembly().GetTypes(), (type, attribute) =>
            {
                string temp = StaticDataUtility.GetGenerateClassName(type, StaticData.Constant.Suffix, false);

                if (temp.ToLower().IndexOf(_className.Trim().ToLower()) > -1 && _className.Trim().Length > 3)
                {
                    _list.Add(temp);
                }
                return false;
            });

        }
        private void Tranverse(Type[] types, Func<Type, StaticData.StaticDataAttribute, bool> func)
        {
            if (func == null)
            {
                return;
            }
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                object[] obs = type.GetCustomAttributes(typeof(StaticData.StaticDataAttribute), false);
                if (obs == null)
                {
                    continue;
                }
                for (int j = 0; j < obs.Length; j++)
                {
                    StaticData.StaticDataAttribute attribute = obs[j] as StaticDataAttribute;
                    if (attribute == null)
                    {
                        continue;
                    }
                    if (func(type, attribute))
                    {
                        break;
                    }
                }
            }
        }
        private void CreateXML(string className)
        {
            Application.OpenURL(_xmlPath);
            string fileName = Path.Combine(_xmlPath, className + ".xml");
            if (!File.Exists(fileName))
            {
                File.Open(fileName, FileMode.CreateNew).Close();
            }
            Type editorType = Assembly.GetExecutingAssembly().GetType(_namespace + "." + className + StaticData.Constant.Suffix);
            IStaticData instance = Activator.CreateInstance(editorType) as IStaticData;
            InitInstance(instance);

            XmlDocument docwrite;
            XmlElement rootWrite;
            XmlDeclaration declaration;
            StaticDataUtilityEditor.CreateXmlFileHead(out docwrite, out declaration, out rootWrite);
            StaticDataEditor.FacadeEditor.Instance.Write(docwrite, rootWrite, instance);
            docwrite.Save(fileName);
        }
        private static void InitInstance(object obj)
        {
            if(obj != null)
            {
                Type type = obj.GetType();
                FieldInfo[] fields = type.GetFields();
                if(fields != null)
                {
                    for(int i = 0; i < fields.Length; i ++)
                    {
                        FieldInfo field = fields[i];
                        if (field != null)
                        {
                            if (field.FieldType.IsValueType)
                            {
                                
                            }
                            else if(field.FieldType.IsArray)
                            {
                                if(field.FieldType.GetElementType().IsSubclassOf(typeof(StaticData.IStaticData)))
                                {
                                    Array array = Activator.CreateInstance(field.FieldType, 1) as Array;
                                    IStaticData instance = Activator.CreateInstance(field.FieldType.GetElementType()) as IStaticData;
                                    InitInstance(instance);
                                    array.SetValue(instance, 0);
                                    field.SetValue(obj, array);
                                }
                                else
                                {
                                    field.SetValue(obj, Activator.CreateInstance(field.FieldType, 1));
                                }
                                
                            }
                            else if (field.FieldType == typeof(string))
                            {

                            }
                            else if (field.FieldType.IsSubclassOf(typeof(IStaticData)))
                            {
                                object o = Activator.CreateInstance(field.FieldType);
                                InitInstance(o);
                                field.SetValue(obj, o);
                            }
                            else
                            {
                                field.SetValue(obj, Activator.CreateInstance(field.FieldType));
                            }
                        }
                    }
                }
            }
        }
    }
}