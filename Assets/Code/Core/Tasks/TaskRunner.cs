using System;

namespace Code.Core.Tasks
{
    public class TaskRunner
    {
        private static TaskRunner _instance;
        public static TaskRunner Instance => _instance ??= new TaskRunner();

        private TaskQueue _taskQueue = new TaskQueue();
        public bool Empty => _taskQueue.Empty;

        public bool IsTagEmpty(string tag)
        {
            return _taskQueue.CountByTag(tag) == 0;
        }

        public int CountByTag(string tag)
        {
            return _taskQueue.CountByTag(tag);
        }

        public void RemoveTasksByTag(string tag)
        {
            _taskQueue.RemoveTasksByTag(tag);
        }
        
        public void WhenAll(Action onComplete, params AbstractTask[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                _taskQueue.AddTask(tasks[i]);
            }
            var taskWhenAll = new TaskWhenAll(tasks, onComplete);
            _taskQueue.AddTask(taskWhenAll);
        }

        public void WhenUntil(Action onComplete, Func<bool> predicate)
        {
            var taskUntil = new TaskUntil(predicate, onComplete);
            _taskQueue.AddTask(taskUntil);
        }
        
        public void WhenAny(Action onComplete, params AbstractTask[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                _taskQueue.AddTask(tasks[i]);
            }
            var taskWhenAny = new TaskWhenAny(tasks, onComplete);
            _taskQueue.AddTask(taskWhenAny);
        }

        public void Run(AbstractTask abstractTask)
        {
            _taskQueue.AddTask(abstractTask);
        }

        public void RemoveTask(AbstractTask abstractTask)
        {
            _taskQueue.RemoveTask(abstractTask);
        }
        
        public void Update()
        {
            _taskQueue.Update();
        }
    }
}