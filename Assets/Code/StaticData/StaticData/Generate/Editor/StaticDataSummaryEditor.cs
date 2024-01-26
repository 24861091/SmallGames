#pragma warning disable  0219
/*
This file is Auto-Generated.Please do not try to alter any part.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
namespace StaticData
{
	public static partial class StaticDataSummaryEditor
	{
		public static IStaticData Create(System.Type type)
		{
			switch (type.Name)
			{
				case "ConfigTestEditor":
					return new ConfigTestEditor();
				case "TemplateTestEditor":
					return new TemplateTestEditor();
				default:
					return null;
			}
		}
		public static void Read(BinaryFileReader reader ,out IStaticData obj, string typeName)
		{
			switch (typeName)
			{
				case "ConfigTestEditor":
					ConfigTestEditor _ConfigTestEditor;
					Read(reader,out _ConfigTestEditor);
					obj = _ConfigTestEditor;
					break;
				case "TemplateTestEditor":
					TemplateTestEditor _TemplateTestEditor;
					Read(reader,out _TemplateTestEditor);
					obj = _TemplateTestEditor;
					break;
				default:
					obj = null;
					break;
			}
		}
		public static void Write(BinaryWriter writer, IStaticData obj)
		{
			switch (obj.GetType().Name)
			{
				case "ConfigTestEditor":
					Write(writer, obj as ConfigTestEditor);
					break;
				case "TemplateTestEditor":
					Write(writer, obj as TemplateTestEditor);
					break;
				default:
					break;
			}
		}
		public static void Read(XmlNode node ,out IStaticData obj, string typeName)
		{
			switch (typeName)
			{
				case "ConfigTestEditor":
					ConfigTestEditor nameConfigTestEditor;
					Read(node,out nameConfigTestEditor);
					obj = nameConfigTestEditor;
					break;
				case "TemplateTestEditor":
					TemplateTestEditor nameTemplateTestEditor;
					Read(node,out nameTemplateTestEditor);
					obj = nameTemplateTestEditor;
					break;
				default:
					obj = null;
					break;
			}
		}
		public static void Write(XmlDocument doc, XmlNode father, IStaticData obj, string label = "")
		{
			switch (obj.GetType().Name)
			{
				case "ConfigTestEditor":
					Write(doc, father, obj as ConfigTestEditor, label);
					break;
				case "TemplateTestEditor":
					Write(doc, father, obj as TemplateTestEditor, label);
					break;
				default:
					break;
			}
		}
		public static System.Type GetType(string type)
		{
			switch (type)
			{
				case "ConfigTestEditor":
					return typeof(ConfigTestEditor);
				case "TemplateTestEditor":
					return typeof(TemplateTestEditor);
				default:
					return null;
			}
		}
	}
}
