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
		private static void Read(BinaryFileReader reader ,out StaticData.ConfigTestEditor obj)
		{
			obj = null;
			if(!reader.ReadBoolean())
			{
				return;
			}
			if(obj == null)
			{
				obj = new StaticData.ConfigTestEditor();
			}
			obj.a = reader.ReadInt32();
			obj.b = reader.ReadString();
			obj.c = reader.ReadInt64();
			obj.platform = (UnityEngine.RuntimePlatform)(reader.ReadInt32());
			Read(reader, out obj.v3);
		}
		private static void Write(BinaryWriter writer, StaticData.ConfigTestEditor obj)
		{
			if(obj == null)
			{
				writer.Write(false);
				return;
			}
			else
			{
				writer.Write(true);
			}
			writer.Write(obj.a);
			if(obj.b == null)
			{
				writer.Write("");
			}
			else
			{
				writer.Write(obj.b);
			}
			writer.Write(obj.c);
			writer.Write((int)obj.platform);
			Write(writer, obj.v3);
		}
		private static void Read(XmlNode node ,out StaticData.ConfigTestEditor obj)
		{
			obj = null;
			string innerText = null;
			if (node != null)
			{
				XmlNode child = null;
				if(obj == null)
				{
					obj = new StaticData.ConfigTestEditor();
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "a");
				if(child == null)
				{
					obj.a = 0;
					StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 a 的XML数据不存在. ");
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.a = 0;
						StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 a 的XML数据里面是空的,没有写数据.  ");
					}
					else
					{
						if(!System.Int32.TryParse(innerText, out obj.a))
						{
							StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 a 的数据格式不对. ");
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "b");
				if(child == null)
				{
					obj.b = "";
					StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 b 的XML数据不存在. ");
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.b = "";
					}
					else
					{
						obj.b = innerText;
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "c");
				if(child == null)
				{
					obj.c = 0;
					StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 c 的XML数据不存在. ");
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.c = 0;
						StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 c 的XML数据里面是空的,没有写数据.  ");
					}
					else
					{
						if(!System.Int64.TryParse(innerText, out obj.c))
						{
							StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 c 的数据格式不对. ");
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "platform");
				if(child == null)
				{
					obj.platform = default(UnityEngine.RuntimePlatform);
					StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 platform 的XML数据不存在. ");
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.platform = default(UnityEngine.RuntimePlatform);
						StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 platform 的XML数据里面是空的,没有写数据.  ");
					}
					else
					{
						if (!System.Enum.TryParse<UnityEngine.RuntimePlatform>(XmlSerializerUtility.GetInnerText(child), out obj.platform))
						{
							StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 platform 的数据格式不对.  ");
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "v3");
				if(child == null)
				{
					obj.v3 = default(UnityEngine.Vector3);
					StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 v3 的XML数据不存在. ");
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.v3 = default(UnityEngine.Vector3);
						StaticDataUtility.LogDesinerError("类型为StaticData.ConfigTestEditor 的字段 v3 的XML数据里面是空的,没有写数据.  ");
					}
					else
					{
						Read(child ,out obj.v3);
					}
				}
			}
		}
		private static void Write(XmlDocument doc, XmlNode father,StaticData.ConfigTestEditor obj, string label = "")
		{
			XmlNode element = XmlSerializerUtility.CreateElement(doc, string.IsNullOrEmpty(label)?"Config":label.Replace("[", "").Replace("]", ""));
			XmlSerializerUtility.AddAttribute(doc, element, "Type", StaticDataUtility.GetGenerateClassName(obj.GetType(), Constant.Suffix, false));
			XmlSerializerUtility.AppendChild(father, element);
			XmlNode child = null;
			child = XmlSerializerUtility.CreateElement(doc, "a");
			XmlSerializerUtility.SetInnerText(child, obj.a.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "b");
			XmlSerializerUtility.SetInnerText(child, obj.b);
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "c");
			XmlSerializerUtility.SetInnerText(child, obj.c.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "platform");
			XmlSerializerUtility.SetInnerText(child, obj.platform.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			Write(doc, element, obj.v3, "v3");
		}
	}
}
