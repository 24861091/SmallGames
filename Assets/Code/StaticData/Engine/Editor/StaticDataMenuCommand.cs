using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;
using EchoEngine;
using StaticData;


namespace StaticDataEditor
{
    [InitializeOnLoad]
    public static partial class StaticDataMenuCommand
    {
        //static StaticDataMenuCommand()
        //{
        //    PlayerPrefs.SetInt(Constant.UseXmlKey, 0);
        //}


        [MenuItem("Tools/Clear Empty Directories")]
        public static void DeleteEmptyDirectories()
        {
            string baseFolder = "";
            string path = Path.Combine(Directory.GetCurrentDirectory(), baseFolder);
            List<string> deleted = new List<string>();
            if (Directory.Exists(path))
            {
                Stack<string> stack = new Stack<string>();
                stack.Push(path);

                while (stack.Count > 0)
                {
                    string folder = stack.Pop();
                    if (!string.IsNullOrEmpty(folder))
                    {
                        string[] folders = Directory.GetDirectories(folder);
                        if (folders != null)
                        {
                            for (int i = 0; i < folders.Length; i++)
                            {
                                stack.Push(folders[i]);
                            }
                        }
                        string[] files = Directory.GetFiles(folder);
                        if (files == null || files.Length <= 0)
                        {
                            deleted.Add(folder);
                        }
                    }
                }

                for (int i = 0; i < deleted.Count; i++)
                {
                    try
                    {
                        Directory.Delete(deleted[i]);
                        StaticDataUtility.Log("delete " + deleted[i]);
                    }
                    catch (Exception)
                    {

                    }

                }
            }
        }

        //[MenuItem("StaticData/Test")]
        //public static void Test()
        //{
        //    //EffectListTemplate config = TemplateManager.Instance.GetTemplate(typeof(EffectListTemplate),10101) as EffectListTemplate;
        //    //if(config != null)
        //    //{
        //    //    EffectListTemplate clone = StaticDataUtilityEditor.Clone<EffectListTemplate>(config);
        //    //    if(clone != null)
        //    //    {
        //    //        clone.effects[0] = null;
        //    //    }
        //    //}

        //}

        [MenuItem("StaticData/Open Overview")]
        public static void OpenOverview()
        {
            StaticDataEditor.StaticDataOverViewEditor.Open();
        }

        [MenuItem("StaticData/Binary/Generate Binary")]
        public static void CreateBinary()
        {
            //long begin = DateTime.Now.Ticks;

            //CreateOriginBinary(StaticData.Constant.TempBinaryPath, StaticDataEditor.FacadeEditor.Instance.BinaryStaticDataPath);
            //_ShowStatusBar();

            //long end = DateTime.Now.Ticks;
            //TimeSpan span = new TimeSpan(end - begin);

            //Debug.LogFormat("++++++++++++++++++++++++++++++++++" + span.TotalMilliseconds);
            var starTick = System.DateTime.Now.Ticks;
            CreateOriginBinary(StaticData.Constant.TempBinaryPath, StaticDataEditor.FacadeEditor.Instance.BinaryStaticDataPath);
            _ShowStatusBar();
            Debug.Log("[StaticData]    Generate Binary cost: " + (DateTime.Now.Ticks - starTick) / 10000000.0f);
        }

        private static void _ShowStatusBar()
        {
            StaticDataEditor.ProgressBarEditorWindow window = new StaticDataEditor.ProgressBarEditorWindow();
            window.Start("Build 二进制文件", "正在Build中...");
            while (true)
            {
                if (window.OnGUI())
                {
                    break;
                }
            }

        }

        public static bool CreateOriginBinary(string temp, string target)
        {
            bool success = true;
            string lastFile = string.Empty;
            try
            {
                StaticDataEditor.BinaryManager.Instance.Clear();
                StaticDataEditor.BinaryManager.Instance.SetVersion(StaticDataSummary.Version);
                StaticDataEditor.BinaryManager.Instance.Construct(StaticDataEditor.StaticDataUtilityEditor.LoadXML(out lastFile, StaticData.Constant.Suffix), temp);
                StaticDataEditor.BinaryManager.Instance.Save();

                if (StaticDataEditor.BinaryManager.Instance.IsExisted())
                {
                    StaticData.StaticDataUtility.Log("Binary Created Success. path:" + target);
                    StaticDataEditor.BinaryManager.Instance.DeleteDirectory(target);
                    Directory.CreateDirectory(target);
                    StaticDataEditor.BinaryManager.Instance.Copy(temp, target);
                    StaticDataEditor.BinaryManager.Instance.DeleteDirectory(temp);
                }
                else
                {
                    throw new Exception("Binary Created failed. path:" + target);
                }
            }
            catch (Exception ex)
            {
                success = false;
                StaticData.StaticDataUtility.LogErrorAndThrowException("File: " + lastFile + "\n" + ex.Message + "\n" + ex.StackTrace);
                return success;
            }
            finally
            {
                //EchoEngine.ApplicationPathTool.SetCSharpBuildTagFile(success);
            }
            return success;
        }

        [MenuItem("StaticData/Code Generate/Generate Code")]
        public static void GenerateCommand()
        {
            if (RuntimePlatform.WindowsEditor == Application.platform)
            {
                Register();
                clearCount = 0;


                //string targetCSharpPath = StaticDataEditor.ConcretDependency.rootPath;
                ////string targetLuaPath = StaticDataEditor.ConcretDependency.rootLuaPath;

                //StaticDataEditor.ConcretDependency.rootPath = Path.Combine(Directory.GetCurrentDirectory(), @"_tempcsharp\");
                ////StaticDataEditor.ConcretDependency.rootLuaPath = Path.Combine(Directory.GetCurrentDirectory(), @"_templua\");

                //string tempCSharp = StaticDataEditor.ConcretDependency.rootPath;
                ////string tempLua = StaticDataEditor.ConcretDependency.rootLuaPath;
                
                GenerateConcret();

                //CodesMerger csharp = new CodesMerger(tempCSharp, targetCSharpPath);
                ////CodesMerger lua = new CodesMerger(tempLua, targetLuaPath);
                //csharp.Merge();
                ////lua.Merge();
                ////Directory.Delete(tempCSharp, true);
                ////Directory.Delete(tempLua, true);

                //StaticDataEditor.ConcretDependency.rootPath = targetCSharpPath;
                ////StaticDataEditor.ConcretDependency.rootLuaPath = targetLuaPath;
            }
            else
            {
                Register();
                clearCount = 0;
                ClearGenerated();
                GenerateConcret();
            }
            RefreshAfterGenerate();

        }
        //[MenuItem("StaticData/Force Code Generate/Force Generate Code(runtime 编译报错也可以生成)")]
        public static void ForceClearCommand()
        {
            if (RuntimePlatform.WindowsEditor == Application.platform)
            {
                //D:\develop\echo\Tools\读表器生成器\IStaticDataGenerator\IStaticDataGenerator\bin\Release\
                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\BattleCore\StaticData\Generate\");
                string rootLuaPath = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\Code\LuaScripts\StaticData\Generate\");
                string pre = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\Code\Client\StaticData\pre");
                string root = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\Code\Client\StaticData\Editor\Definition");

                System.Diagnostics.Process.Start(@"Tools\compile\IStaticDataGeneratorWithDLL.exe", string.Format("{0} {1} {2} {3} {4} {5} {6}", "generate", rootPath, rootLuaPath, pre, root, Directory.GetCurrentDirectory(),EditorApplication.applicationPath.Replace(" ","#")));
            }
        }
        [MenuItem("StaticData/Binary/Use Binary")]
        public static void UseBinary()
        {
            int xml = PlayerPrefs.GetInt(StaticData.Constant.UseXmlKey, 0);

            if (xml == 0)
            {
                xml = 1;
            }
            else
            {
                xml = 0;
            }
            PlayerPrefs.SetInt(StaticData.Constant.UseXmlKey, xml);
            Menu.SetChecked(@"StaticData/Binary/Use Binary", StaticDataUtility.UseBinary);
        }

        //[MenuItem("StaticData/Force Code Generate/Force Clear (runtime 编译报错也可以Clear)")]
        //public static void GenerateEmpty()
        //{
        //    if (RuntimePlatform.WindowsEditor == Application.platform)
        //    {
        //        System.Diagnostics.Process.Start(@"Tools\compile\IStaticDataGeneratorWithDLL.exe", string.Format("{0} {1}", "clear", Directory.GetCurrentDirectory()));
        //        //System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
        //        ////start.WorkingDirectory = Directory.GetCurrentDirectory();
        //        ////start.CreateNoWindow = false;
        //        ////start.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        //        //start.FileName = @"Tools\compile\IStaticDataGeneratorWithDLL.exe";
        //        //start.Arguments = string.Format("{0}", "clear");
        //        ////start.UseShellExecute = false;
        //        //System.Diagnostics.Process.Start(start);
        //    }

        //}
    }
}