using Code.Core.Tasks;

namespace GameLib.Main.Tasks
{
    public interface ITaskQueue
    {
        void AddTask(AbstractTask task);
        void Update();
        void Destroy();
        public int CountByTag(string tag);
    }
}