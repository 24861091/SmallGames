using System;
using System.Collections.Generic;

namespace Code.Core.Tasks
{
    public class ParallelTask : AbstractTask
    {
        private LinkedList<AbstractTask> _innerTasks = new LinkedList<AbstractTask>();
        private Action _onFinish;

        public void Add(AbstractTask task)
        {
            _innerTasks.AddLast(task);
        }

        public void OnFinish(Action onFinish)
        {
            _onFinish = onFinish;
        }

        protected override void OnUpdate()
        {
            var cur = _innerTasks.First;
            while (cur != null)
            {
                var next = cur.Next;
                var task = cur.Value;
                if (task.Status == TaskStatus.Pending)
                {
                    task.InternalStart();
                }

                if (task.Status == TaskStatus.Running)
                {
                    task.InternalUpdate();
                }
                else
                {
                    if (task.Status == TaskStatus.Done)
                    {
                        task.InternalComplete();
                        _innerTasks.Remove(cur);
                    }
                    else
                    {
                        task.InternalAbort();
                        _innerTasks.Remove(cur);
                    }
                }

                cur = next;
            }

            if (_innerTasks.Count == 0)
            {
                Status = TaskStatus.Done;
            }
            
            // for (int i = 0; i < _innerTasks.Count; i++)
            // {
            //     var task = _innerTasks[i];
            //     try
            //     {
            //         if (task.Status == TaskStatus.Pending)
            //         {
            //             task.InternalStart();
            //         }
            //
            //         if (task.Status == TaskStatus.Running)
            //         {
            //             task.InternalUpdate();
            //         }
            //
            //         if (task.Status == TaskStatus.Running)
            //         {
            //             running.Add(task);
            //         }
            //         else
            //         {
            //             if (task.Status == TaskStatus.Done)
            //             {
            //                 task.InternalComplete();
            //             }
            //             else
            //             {
            //                 task.InternalAbort();
            //             }
            //         }
            //     }
            //     catch (Exception e)
            //     {
            //         Debug.LogError(MiscUtil.GetFullExceptionInfo(e));
            //     }
            // }
            
            
            //
            // if (allFinished)
            // {
            //     Status = TaskStatus.Done;
            // }
        }

        protected override void OnComplete()
        {
            _onFinish?.Invoke();
        }
    }
}