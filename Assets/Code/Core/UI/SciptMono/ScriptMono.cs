using System;
using UnityEngine;

namespace UI
{
    public partial class ScriptMono : MonoBehaviour
    {
        private bool _isQuiting;
    
        public bool HandleOnDestroyWhenQuiting { get; set; }
        
        public virtual void OnDestroy()
        {
            if (!HandleOnDestroyWhenQuiting && _isQuiting) return;
    
        }
    
        private void OnApplicationQuit()
        {
            _isQuiting = true;
        }
    }
}
