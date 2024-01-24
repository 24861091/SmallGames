using System;
using System.Collections.Generic;
using Code.Core.Utils;
using GameLib.Main.Tasks;
using UnityEngine;

namespace Code.Core.Tasks
{
    public class TaskChain : ITaskQueue
    {
        private LinkedList<AbstractTask> _running = new();

        public void Update()
        {
            if (_running.Count == 0) return;

            var node = _running.First;
            while (node != null)
            {
                var next = node.Next;
                try
                {
                    var task = node.Value;
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
                        _running.Remove(node);
                        task.InternalComplete();
                    }
                    else if (task.Status == TaskStatus.Aborted)
                    {
                        _running.Remove(node);
                        task.InternalAbort();
                    }
                    else if (task.Status == TaskStatus.Running) // still Running
                    {
                        // keep only 1 task running
                        break; // in case there's some logic outside the loop
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(MiscUtil.GetFullExceptionInfo(e));
                }
                finally
                {
                    node = next;
                }
            }
        }

        public void AddTask(AbstractTask task)
        {
            _running.AddLast(task);
        }
        
        public void Destroy()
        {
            foreach (var task in _running)
                task.InternalAbort();
        }

        public int CountByTag(string tag)
        {
            var ret     = 0;
            var current = _running.First;
            while (current != null)
            {
                if (current.Value.Tag == tag) ret++;
                current = current.Next;
            }

            return ret;
        }
    }
}