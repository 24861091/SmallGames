using System;
using System.Collections.Generic;
using Code.Core.Utils;
using UnityEngine;

namespace Code.Core.Tasks
{
    public class SequentialTask : AbstractTask
    {
        private int                _index = 0;
        private List<AbstractTask> _innerTasks = new List<AbstractTask>();
        private Action             _onFinish;

        public void Add(AbstractTask task)
        {
            _innerTasks.Add(task);
        }

        public void Add(IEnumerable<AbstractTask> tasks)
        {
            _innerTasks.AddRange(tasks);
        }

        public SequentialTask OnFinish(Action onFinish)
        {
            _onFinish = onFinish;
            return this;
        }

        protected override void OnUpdate()
        {
            if (_innerTasks.Count == 0)
            {
                Status = TaskStatus.Done;
                return;
            }
            
            while (_index < _innerTasks.Count)
            {
                var curTask = _innerTasks[_index];
                try
                {
                    if (curTask.Status == TaskStatus.Pending)
                    {
                        curTask.InternalStart();
                    }
                    
                    if (curTask.Status == TaskStatus.Running)
                    {
                        curTask.InternalUpdate();
                    }

                    if (curTask.Status == TaskStatus.Running) break;

                    if (curTask.Status == TaskStatus.Done)
                    {
                        curTask.InternalComplete();
                    }
                    else
                    {
                        curTask.InternalAbort();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("stack trace");
                    Debug.LogError($"{e.StackTrace}");

                    Debug.LogError(MiscUtil.GetFullExceptionInfo(e));
                }

                _index++;
                if (_index >= _innerTasks.Count)
                {
                    Status = TaskStatus.Done;
                    break;
                }
            }
        }

        protected override void OnComplete()
        {
            _onFinish?.Invoke();
        }
    }
}