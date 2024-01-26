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
		private static void Read(BinaryFileReader reader ,out StaticData.TemplateTestEditor obj)
		{
			obj = null;
			if(!reader.ReadBoolean())
			{
				return;
			}
			if(obj == null)
			{
				obj = new StaticData.TemplateTestEditor();
			}
			obj.ID = reader.ReadInt32();
			obj.a3 = reader.ReadInt32();
			obj.b3 = reader.ReadString();
			obj.c3 = reader.ReadInt64();
			obj.platform3 = (UnityEngine.RuntimePlatform)(reader.ReadInt32());
			Read(reader, out obj.v33);
			obj.id = reader.ReadString();
		}
		private static void Write(BinaryWriter writer, StaticData.TemplateTestEditor obj)
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
			writer.Write(obj.ID);
			writer.Write(obj.a3);
			if(obj.b3 == null)
			{
				writer.Write("");
			}
			else
			{
				writer.Write(obj.b3);
			}
			writer.Write(obj.c3);
			writer.Write((int)obj.platform3);
			Write(writer, obj.v33);
			if(obj.id == null)
			{
				writer.Write("");
			}
			else
			{
				writer.Write(obj.id);
			}
		}
		private static void Read(XmlNode node ,out StaticData.TemplateTestEditor obj)
		{
			obj = null;
			string innerText = null;
			if (node != null)
			{
				XmlNode child = null;
				if(obj == null)
				{
					obj = new StaticData.TemplateTestEditor();
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "ID");
				if(child == null)
				{
					obj.ID = 0;
					StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 ID 的XML数据不存在. ID = " + obj.ID);
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.ID = 0;
						StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 ID 的XML数据里面是空的,没有写数据.  ID = " + obj.ID);
					}
					else
					{
						if(!System.Int32.TryParse(innerText, out obj.ID))
						{
							StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 ID 的数据格式不对. ID = " + obj.ID);
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "a3");
				if(child == null)
				{
					obj.a3 = 0;
					StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 a3 的XML数据不存在. ID = " + obj.ID);
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.a3 = 0;
						StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 a3 的XML数据里面是空的,没有写数据.  ID = " + obj.ID);
					}
					else
					{
						if(!System.Int32.TryParse(innerText, out obj.a3))
						{
							StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 a3 的数据格式不对. ID = " + obj.ID);
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "b3");
				if(child == null)
				{
					obj.b3 = "";
					StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 b3 的XML数据不存在. ID = " + obj.ID);
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.b3 = "";
					}
					else
					{
						obj.b3 = innerText;
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "c3");
				if(child == null)
				{
					obj.c3 = 0;
					StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 c3 的XML数据不存在. ID = " + obj.ID);
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.c3 = 0;
						StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 c3 的XML数据里面是空的,没有写数据.  ID = " + obj.ID);
					}
					else
					{
						if(!System.Int64.TryParse(innerText, out obj.c3))
						{
							StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 c3 的数据格式不对. ID = " + obj.ID);
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "platform3");
				if(child == null)
				{
					obj.platform3 = default(UnityEngine.RuntimePlatform);
					StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 platform3 的XML数据不存在. ID = " + obj.ID);
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.platform3 = default(UnityEngine.RuntimePlatform);
						StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 platform3 的XML数据里面是空的,没有写数据.  ID = " + obj.ID);
					}
					else
					{
						if (!System.Enum.TryParse<UnityEngine.RuntimePlatform>(XmlSerializerUtility.GetInnerText(child), out obj.platform3))
						{
							StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 platform3 的数据格式不对.  ID = " + obj.ID);
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "v33");
				if(child == null)
				{
					obj.v33 = default(UnityEngine.Vector3);
					StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 v33 的XML数据不存在. ID = " + obj.ID);
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.v33 = default(UnityEngine.Vector3);
						StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 v33 的XML数据里面是空的,没有写数据.  ID = " + obj.ID);
					}
					else
					{
						Read(child ,out obj.v33);
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "id");
				if(child == null)
				{
					obj.id = "";
					StaticDataUtility.LogDesinerError("类型为StaticData.TemplateTestEditor 的字段 id 的XML数据不存在. ID = " + obj.ID);
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.id = "";
					}
					else
					{
						obj.id = innerText;
					}
				}
			}
		}
		private static void Write(XmlDocument doc, XmlNode father,StaticData.TemplateTestEditor obj, string label = "")
		{
			XmlNode element = XmlSerializerUtility.CreateElement(doc, string.IsNullOrEmpty(label)?"Template":label.Replace("[", "").Replace("]", ""));
			XmlSerializerUtility.AddAttribute(doc, element, "Type", StaticDataUtility.GetGenerateClassName(obj.GetType(), Constant.Suffix, false));
			XmlSerializerUtility.AppendChild(father, element);
			XmlNode child = null;
			child = XmlSerializerUtility.CreateElement(doc, "ID");
			XmlSerializerUtility.SetInnerText(child, obj.ID.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "a3");
			XmlSerializerUtility.SetInnerText(child, obj.a3.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "b3");
			XmlSerializerUtility.SetInnerText(child, obj.b3);
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "c3");
			XmlSerializerUtility.SetInnerText(child, obj.c3.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "platform3");
			XmlSerializerUtility.SetInnerText(child, obj.platform3.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			Write(doc, element, obj.v33, "v33");
			child = XmlSerializerUtility.CreateElement(doc, "id");
			XmlSerializerUtility.SetInnerText(child, obj.id);
			XmlSerializerUtility.AppendChild(element, child);
		}
	}
}
