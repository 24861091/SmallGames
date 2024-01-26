using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace StaticDataEditor
{
    public class ProgressBarEditorWindow
    {
        private float secs = 0.3f;
        private float startVal = 0f;
        private float progress = 0f;
        private string title;
        private string decription;

        public void Start(string title, string decription)
        {
            startVal = (float)EditorApplication.timeSinceStartup;
            this.title = title;
            this.decription = decription;
        }

        public bool OnGUI()
        {
            progress = (float)(EditorApplication.timeSinceStartup - startVal);
            if (progress < secs)
            {
                EditorUtility.DisplayProgressBar(this.title, this.decription, progress / secs);
            }
            else
            {
                EditorUtility.ClearProgressBar();
                return true;
            }
            return false;
        }
    }
}