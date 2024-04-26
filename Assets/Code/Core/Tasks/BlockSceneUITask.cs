
namespace Code.Core.Tasks
{
    public abstract class BlockSceneUITask : AbstractTask
    {
        protected override void OnStart()
        {
            //UIManager.BlockSceneUIClick(true);
        }

        protected override void OnComplete()
        {
            //UIManager.BlockSceneUIClick(false);
        }

        protected override void OnAbort()
        {
            //UIManager.BlockSceneUIClick(false);
        }
    }
}