using System;
using Code.Core.Utils;

namespace Code.Core.Tasks
{
    public class CallFuncTask : AbstractTask
    {
        private Action _callFunc;
        
        public CallFuncTask(Action callFunc)
        {
            _callFunc = callFunc;
        }

        protected override void OnStart()
        {
            try
            {
                _callFunc?.Invoke();
            }
            catch (Exception e)
            {
                CoreLog.LogError(MiscUtil.GetFullExceptionInfo(e));
            }
            finally
            {
                Status = TaskStatus.Done;
            }
        }
    }
}