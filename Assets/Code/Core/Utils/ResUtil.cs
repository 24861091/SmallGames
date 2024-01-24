using System.IO;
using System.Threading.Tasks;

namespace Code.Core.Utils
{
    public static class ResUtil
    {
        public static string WrapStreamingAssetPathForUwr(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return path;
#else
            return "file:///" + path.TrimStart('/');
#endif
        }

        public static string Combine(params string[] paths)
        {
            return paths == null || paths.Length <= 0 ? "" : Path.Combine(paths).Replace("\\", "/");
        }

        public static void EnsureDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        public static async Task<bool> FileExists(string path)
        {
            var ret = false;
            await Task.Run(() => ret = File.Exists(path));
            return ret;
        }
    }
}