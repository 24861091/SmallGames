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
	public static partial class StaticDataSummary
	{
		public static string Version = "e09cd333-c42d-4f4e-8f75-7d14f2dbf192";
		public static IStaticData Create(System.Type type)
		{
			switch (type.Name)
			{
				case "ConfigTest":
					return new ConfigTest();
				case "TemplateTest":
					return new TemplateTest();
				default:
					return null;
			}
		}
		public static void Read(BinaryFileReader reader ,out IStaticData obj, string typeName)
		{
			switch (typeName)
			{
				case "ConfigTest":
					ConfigTest _ConfigTest;
					Read(reader,out _ConfigTest);
					obj = _ConfigTest;
					break;
				case "TemplateTest":
					TemplateTest _TemplateTest;
					Read(reader,out _TemplateTest);
					obj = _TemplateTest;
					break;
				default:
					obj = null;
					break;
			}
		}
		public static void Read(XmlNode node ,out IStaticData obj, string typeName)
		{
			switch (typeName)
			{
				case "ConfigTest":
					ConfigTest nameConfigTest;
					Read(node,out nameConfigTest);
					obj = nameConfigTest;
					break;
				case "TemplateTest":
					TemplateTest nameTemplateTest;
					Read(node,out nameTemplateTest);
					obj = nameTemplateTest;
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
				case "ConfigTest":
					Write(doc, father, obj as ConfigTest, label);
					break;
				case "TemplateTest":
					Write(doc, father, obj as TemplateTest, label);
					break;
				default:
					break;
			}
		}
		public static System.Type GetType(string type)
		{
			switch (type)
			{
				case "ConfigTest":
					return typeof(ConfigTest);
				case "TemplateTest":
					return typeof(TemplateTest);
				default:
					return null;
			}
		}
	}
}
