using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticDataEditor
{
    public class CodesMerger
    {
        private string sourceFolder = "";
        private string targetFolder = "";

        public CodesMerger(string source, string target)
        {
            ReInit(source, target);
        }
        public void ReInit(string source, string target)
        {
            sourceFolder = source;
            targetFolder = target;
        }

        public void Merge()
        {
            List<string> sourceFiles = CalculateFiles(sourceFolder);
            List<string> targetFiles = CalculateFiles(targetFolder);
            if(sourceFiles != null && targetFiles != null)
            {
                for (int i = 0; i < sourceFiles.Count; i++)
                {
                    string sourceFile = sourceFiles[i];
                    string targetFile = sourceFile.Replace(sourceFolder, targetFolder);
                    if (targetFiles.Contains(targetFile))
                    {
                        // both
                        string sourceText = File.ReadAllText(sourceFile).Trim();
                        string targetText = File.ReadAllText(targetFile).Trim();
                        if (sourceText != targetText)
                        {
                            File.Delete(targetFile);
                            string folder = Path.GetDirectoryName(targetFile);
                            MakeSure(folder);
                            File.Copy(sourceFile, targetFile);
                        }
                    }
                    else
                    {
                        //source contains, target not
                        string folder = Path.GetDirectoryName(targetFile);
                        MakeSure(folder);
                        File.Copy(sourceFile, targetFile);
                    }
                }
                for (int i = 0; i < targetFiles.Count; i++)
                {
                    string targetFile = targetFiles[i];
                    if (targetFile.EndsWith(".meta"))
                    {
                        continue;
                    }
                    string sourceFile = targetFile.Replace(targetFolder, sourceFolder);
                    if (!sourceFiles.Contains(sourceFile))
                    {
                        //target contains, source not
                        File.Delete(targetFiles[i]);
                    }
                }
            }
        }

        private List<string> CalculateFiles(string directory)
        {
            if(string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                return null;
            }
            List<string> r = new List<string>();
            Stack<string> stack = new Stack<string>();
            stack.Push(directory);
            while(stack.Count > 0)
            {
                string folder = stack.Pop();
                string[] files = Directory.GetFiles(folder);
                string[] folders = Directory.GetDirectories(folder);
                if(folders != null)
                {
                    for(int i = 0; i < folders.Length; i ++)
                    {
                        stack.Push(folders[i]);
                    }
                }
                if(files != null)
                {
                    for(int i = 0; i < files.Length; i ++)
                    {
                        r.Add(files[i]);
                    }
                }
            }
            return r;
        }
        public static void MakeSure(string folder)
        {
            if (!string.IsNullOrEmpty(folder))
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }

    }
}