using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace StaticDataEditor
{
    public class StaticDataOverViewEditor : EditorWindow
    {
        public static void Open()
        {
            StaticDataOverViewEditor window = StaticDataOverViewEditor.GetWindow<StaticDataOverViewEditor>(false, "StaticData Overview Editor", true);
            if(window != null)
            {
                
                window.Show();
            }
        }
        private void OnEnable()
        {
            if(_state == null)
            {
                _state = new TreeViewState();
            }
            if(_treeview == null)
            {
                _treeview = new OverviewTreeView(_state);
            }
            _treeview.Reload();
        }
        private void OnGUI()
        {
            _treeview.OnGUI(new Rect(0, 0, position.width / 2f, position.height / 2f));
        }
        [SerializeField]
        private TreeViewState _state = new TreeViewState();
        private OverviewTreeView _treeview = null;
    }
}