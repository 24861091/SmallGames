using System;

namespace Code.Core.Tasks
{
    public class TaskUntil : AbstractTask
    {
        private Func<bool> _predicate;
        private Action _onComplete;

        public TaskUntil(Func<bool> predicate, Action onComplete)
        {
            _predicate = predicate;
            _onComplete = onComplete;
            Status = TaskStatus.Pending;
        }

        protected override void OnComplete()
        {
            _onComplete?.Invoke();
        }

        protected override void OnUpdate()
        {
            if (_predicate.Invoke())
            {
                Status = TaskStatus.Done;
            }
        }
    }
}