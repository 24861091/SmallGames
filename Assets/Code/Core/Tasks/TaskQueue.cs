using System;
using System.Collections.Generic;
using Code.Core.Utils;
using UnityEngine;

namespace Code.Core.Tasks
{
    public sealed class TaskQueue : ITaskQueue
    {
        private LinkedList<AbstractTask> _runningTasks = new LinkedList<AbstractTask>();


        public bool Empty => _runningTasks.Count == 0;

        public void Update()
        {
            if (_runningTasks.Count == 0) return;
            
            var current = _runningTasks.First;
            while (current != null)
            {
                var next = current.Next;
                var task = current.Value;

                try
                {
                    if (task.Status == TaskStatus.Pending)
                    {
                        task.InternalStart();
                    }
                    
                    if (task.Status == TaskStatus.Running)
                    {
                        task.InternalUpdate();
                    }

                    if (task.Status == TaskStatus.Done)
                    {
                        _runningTasks.Remove(current);
                        task.InternalComplete();
                    }
                    else if (task.Status == TaskStatus.Aborted)
                    {
                        _runningTasks.Remove(task);
                        task.InternalAbort();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(MiscUtil.GetFullExceptionInfo(e));
                }
                finally
                {
                    current = next;
                }
            }
        }
        
        public void AddTask(AbstractTask abstractTask)
        {
            _runningTasks.AddLast(abstractTask);
        }
        
        public void Destroy()
        {
            foreach (var task in _runningTasks)
            {
                task.InternalAbort();
            }
        }

        public int CountByTag(string tag)
        {
            var ret     = 0;
            var current = _runningTasks.First;
            while (current != null)
            {
                if (current.Value.Tag == tag) ret++;
                current = current.Next;
            }

            return ret;
        }

        public void RemoveTask(AbstractTask abstractTask)
        {
            _runningTasks.Remove(abstractTask);
        }

        public void RemoveTasksByTag(string tag)
        {
            var current = _runningTasks.First;
            while (current != null)
            {
                var next = current.Next;
                if (current.Value.Tag == tag)
                {
                    _runningTasks.Remove(current);
                }
                
                current = next;
            }

        }
    }
}