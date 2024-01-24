using System;

namespace Code.Core.Tasks
{
    public class TaskWhenAny : AbstractTask
    {
        private AbstractTask[] _tasks;
        private Action         _onComplete;

        public TaskWhenAny(AbstractTask[] tasks, Action onComplete)
        {
            _tasks      = tasks;
            _onComplete = onComplete;
            Status      = TaskStatus.Pending;
        }

        protected override void OnComplete()
        {
            _onComplete?.Invoke();
        }

        protected override void OnUpdate()
        {
            for (int i = 0; i < _tasks.Length; i++)
            {
                var status = _tasks[i].Status;
                if (status == TaskStatus.Done || status == TaskStatus.Aborted)
                {
                    Status = TaskStatus.Done;
                    return;
                }
            }
        }
    }
}