using System;

namespace GameLib.Main.UI
{
    public class PanelConfigAttribute : Attribute
    {
        public UIType     UIType     = UIType.PopUp;
        public UIMaskMode UIMaskMode = UIMaskMode.ShowMask;
        public UIPopMode  UIPopMode  = UIPopMode.Overlay;
        public UICaching  Caching    = UICaching.None;
    }
}