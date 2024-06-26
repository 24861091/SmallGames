using System.Linq;

namespace Code.Core.GameComponent
{
    /// <summary>
    /// 所有GameComponent的根节点
    /// </summary>
    public class GameComponentRoot : RootComponent
    {
        private static GameComponentRoot _instance;
        public static GameComponentRoot Instance => _instance ??= new GameComponentRoot();

        public void Destroy()
        {
            var keys = SubComponents.Keys.ToArray();
            foreach (var k in keys)
            {
                RemoveAllComponents(k);
            }

            _instance = null;
        }

        protected override void Awake()
        {
            
        }
    }
}