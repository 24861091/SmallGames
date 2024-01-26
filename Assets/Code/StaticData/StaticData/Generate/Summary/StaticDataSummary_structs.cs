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
		private static void Read(BinaryFileReader reader ,out Vector3 obj)
		{
			obj = new Vector3();
			obj.x = reader.ReadSingle();
			obj.y = reader.ReadSingle();
			obj.z = reader.ReadSingle();
		}
		private static void Read(XmlNode node ,out Vector3 obj)
		{
			obj = new Vector3();
			string innerText = null;
			if (node != null)
			{
				XmlNode child = null;
				child = XmlSerializerUtility.GetChildrenNode(node, "x");
				if(child == null)
				{
					obj.x = 0f;
					StaticDataUtility.LogDesinerError("类型为Vector3 的字段 x 的XML数据不存在. ");
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.x = 0f;
						StaticDataUtility.LogDesinerError("类型为Vector3 的字段 x 的XML数据里面是空的,没有写数据.  ");
					}
					else
					{
						if(!System.Single.TryParse(innerText, out obj.x))
						{
							StaticDataUtility.LogDesinerError("类型为Vector3 的字段 x 的数据格式不对. ");
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "y");
				if(child == null)
				{
					obj.y = 0f;
					StaticDataUtility.LogDesinerError("类型为Vector3 的字段 y 的XML数据不存在. ");
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.y = 0f;
						StaticDataUtility.LogDesinerError("类型为Vector3 的字段 y 的XML数据里面是空的,没有写数据.  ");
					}
					else
					{
						if(!System.Single.TryParse(innerText, out obj.y))
						{
							StaticDataUtility.LogDesinerError("类型为Vector3 的字段 y 的数据格式不对. ");
						}
					}
				}
				child = XmlSerializerUtility.GetChildrenNode(node, "z");
				if(child == null)
				{
					obj.z = 0f;
					StaticDataUtility.LogDesinerError("类型为Vector3 的字段 z 的XML数据不存在. ");
				}
				else
				{
					innerText = XmlSerializerUtility.GetInnerText(child);
					if(string.IsNullOrEmpty(innerText))
					{
						obj.z = 0f;
						StaticDataUtility.LogDesinerError("类型为Vector3 的字段 z 的XML数据里面是空的,没有写数据.  ");
					}
					else
					{
						if(!System.Single.TryParse(innerText, out obj.z))
						{
							StaticDataUtility.LogDesinerError("类型为Vector3 的字段 z 的数据格式不对. ");
						}
					}
				}
			}
		}
		private static void Write(XmlDocument doc, XmlNode father,Vector3 obj, string label = "")
		{
			XmlNode element = XmlSerializerUtility.CreateElement(doc, string.IsNullOrEmpty(label)?"":label.Replace("[", "").Replace("]", ""));
			XmlSerializerUtility.AddAttribute(doc, element, "Type", StaticDataUtility.GetGenerateClassName(obj.GetType(), Constant.Suffix, false));
			XmlSerializerUtility.AppendChild(father, element);
			XmlNode child = null;
			child = XmlSerializerUtility.CreateElement(doc, "x");
			XmlSerializerUtility.SetInnerText(child, obj.x.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "y");
			XmlSerializerUtility.SetInnerText(child, obj.y.ToString());
			XmlSerializerUtility.AppendChild(element, child);
			child = XmlSerializerUtility.CreateElement(doc, "z");
			XmlSerializerUtility.SetInnerText(child, obj.z.ToString());
			XmlSerializerUtility.AppendChild(element, child);
		}
	}
}
