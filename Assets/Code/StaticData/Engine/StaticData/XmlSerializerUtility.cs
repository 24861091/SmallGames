/************************************************
 created : 2018.8
 author : caiming
************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class XmlSerializerUtility
{
    public static int GetChildrenNum(XmlNode node)
    {
        return node.ChildNodes.Count;
    }

    public static XmlNode GetChildrenNode(XmlNode node, int index)
    {
        
        return node.ChildNodes.Item(index);
    }
    public static XmlNode GetChildrenNode(XmlNode node, string name)
    {
        if (node == null)
        {
            //StaticData.StaticDataUtility.LogWarning("xml node is null!");
            return null;
        }
        int n = node.ChildNodes.Count;
        for (int i = 0; i < n; i++)
        {
            XmlNode child = node.ChildNodes.Item(i);
            if (child != null)
            {
                if (child.Name == name)
                {
                    return child;
                }
            }

        }
        //StaticData.StaticDataUtility.LogWarning(string.Format("xml node:{0}'s child node:{1} is not found!", node.Name, name));
        return null;
    }


    public static string GetInnerText(XmlNode node)
    {
        if(node == null)
        {
            StaticData.StaticDataUtility.LogWarning(("xml node is null!"));
        }
        return node.InnerText.Trim();
    }

    public static void SetInnerText(XmlNode node, string text)
    {
        if (node == null)
        {
            StaticData.StaticDataUtility.LogWarning(string.Format("xml node:content={0} is null!", text));
        }

        node.InnerText = text;
    }

    public static XmlNode CreateElement(XmlDocument doc,string name)
    {
        return doc.CreateElement(name);
    }
    public static void AddAttribute(XmlDocument doc, XmlNode node, string key, string value)
    {
        if (node != null)
        {
            XmlAttribute attribute = null;

            int n = node.Attributes.Count;
            for (int i = 0; i < n; i++)
            {
                XmlAttribute att = node.Attributes[i];
                if (att != null)
                {
                    if (att.Name == key)
                    {
                        attribute = att;
                        break;
                    }
                }
            }
            if (attribute == null)
            {
                attribute = doc.CreateAttribute(key);
                node.Attributes.Append(attribute);
            }
            attribute.Value = value;
        }
        else
        {
            StaticData.StaticDataUtility.LogWarning(string.Format("xml node is null! adding attribute:{0}={1}",key, value));
        }
    }

    public static void AppendChild(XmlNode father, XmlNode child)
    {
        father.AppendChild(child);
    }

}