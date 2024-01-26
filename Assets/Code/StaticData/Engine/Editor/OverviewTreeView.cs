using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace StaticDataEditor
{
    public class OverviewTreeView : TreeView
    {
        public struct Item
        {
            public string path;
            public TreeViewItem item;
            public Item(string path, int id, string display, TreeViewItem father = null, int depth = 0)
            {
                this.path = path;
                item = new TreeViewItem(id, depth, display);
                if (father != null)
                {
                    father.AddChild(item);
                }
            }

        }
        private static int _counter = 0;
        public OverviewTreeView(TreeViewState state) : base(state)
        {

        }
        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(1, -1, "folder");
            TranverseElementXML(root);
            return root;
        }

        private void BuildTree(TreeViewItem root)
        {
            _counter = 0;
        }


        private void TranverseElementXML(TreeViewItem root)
        {
            Stack<Item> stack = new Stack<Item>();
            for (int j = 0; j < StaticDataEditor.FacadeEditor.Instance.XMLStaticDataPaths.Length; j++)
            {
                Item item = new Item(StaticDataEditor.FacadeEditor.Instance.XMLStaticDataPaths[j], _counter++, /*Path.GetDirectoryName*/(StaticDataEditor.FacadeEditor.Instance.XMLStaticDataPaths[j]), root, root.depth + 1);
                stack.Push(item);
            }
            bool quit = false;
            while (stack.Count > 0)
            {
                if (quit)
                {
                    break;
                }
                Item it = stack.Pop();
                string path = it.path;
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
                if (path != null && Directory.Exists(path))
                {
                    //string name = Path.GetDirectoryName(path);

                    string[] folders = Directory.GetDirectories(path);
                    if (folders != null)
                    {
                        for (int m = 0; m < folders.Length; m++)
                        {
                            Item folderitem = new Item(folders[m], _counter++, /*Path.GetDirectoryName*/(folders[m]), it.item, it.item.depth + 1);
                            stack.Push(folderitem);
                        }
                    }
                    string[] files = Directory.GetFiles(path);

                    for (int j = 0; j < files.Length; j++)
                    {
                        if (quit)
                        {
                            break;
                        }

                        string file = files[j];
                        string fileName = Path.GetFileName(file);
                        Item fileItem = new Item(file, _counter++, fileName, it.item, it.item.depth + 1);


                        XmlDocument doc = new XmlDocument();
                        XmlElement firstChild = null;
                        doc.Load(file);
                        firstChild = StaticDataUtilityEditor.GetBaseElement(doc) as XmlElement;
                        if (firstChild == null)
                        {
                            throw new Exception(string.Format("Error, {0} 是空的.", file));
                        }
                        else if (firstChild.HasChildNodes)
                        {
                            int n = firstChild.ChildNodes.Count;
                            for (int i = 0; i < n; i++)
                            {
                                if (quit)
                                {
                                    break;
                                }

                                XmlNode template = firstChild.ChildNodes[i];
                                if (template.Attributes.Count <= 0)
                                {
                                    continue;
                                }
                                if (template.Attributes["Type"] == null && template.Attributes["type"] == null)
                                {
                                    continue;
                                }
                                string v = null;
                                if (template.Attributes["Type"] != null)
                                {
                                    v = template.Attributes["Type"].Value;
                                }
                                else
                                {
                                    v = template.Attributes["type"].Value;
                                }
                                XmlElement element = template as XmlElement;
                                if (element != null)
                                {
                                    IStaticData data = null;
                                    StaticDataEditor.FacadeEditor.Instance.Read(element, out data, v + StaticData.Constant.Suffix);
                                    if (data != null)
                                    {
                                        if (data is Template)
                                        {
                                            Template tem = data as Template;
                                            new Item("", _counter++, v + tem.ID.ToString(), fileItem.item, fileItem.item.depth + 1);
                                        }
                                        else if (data is Config)
                                        {
                                            new Item("", _counter++, v, fileItem.item, fileItem.item.depth + 1);
                                        }
                                    }
                                }
                            }
                        }
                        doc = null;
                        firstChild = null;

                    }
                }
            }


        }
    }
}