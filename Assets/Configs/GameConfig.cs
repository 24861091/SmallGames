namespace Metadata
{
#if UNITY_EDITOR
    [Export(ExportFlags.ExportAll)]
    [System.Serializable]
#endif

    public class GameConfig : Config
    {
        public string IP;
        public int Port;
    }

}