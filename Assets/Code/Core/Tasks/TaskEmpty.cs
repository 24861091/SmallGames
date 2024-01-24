using GameLib.Main.Tasks;

namespace Code.Core.Tasks
{
    public class TaskEmpty : AbstractTask
    {
        protected override void OnStart()
        {
            Status = TaskStatus.Done;
        }
    }
}