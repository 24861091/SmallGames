using System.Diagnostics;
using Code.Core.Utils;
using GameLib.Main.Tasks;

namespace Code.Core.Tasks
{
    public abstract class AbstractTask
    {
        private static ulong _taskId;
        private static ulong NextTaskId => _taskId++;

        protected readonly ulong _id;

        public string Tag;

#if DEBUG
        protected Stopwatch _stopwatch = new Stopwatch();
        protected bool      _isLongTask;
#endif

        public TaskStatus Status { get; protected set; } = TaskStatus.Pending;

        protected AbstractTask()
        {
            _id = NextTaskId;
        }

        public void InternalUpdate()
        {
#if DEBUG
            if (_stopwatch.ElapsedMilliseconds > 10 * 1000)
            {
                _isLongTask = true;
                CoreLog.LogWarning(
                    $"Long task warning: ID: {_id} Type: {GetType().FullName} " +
                    $"lived for {_stopwatch.ElapsedMilliseconds / 1000}s");
                _stopwatch.Reset();
                _stopwatch.Restart();
            }
#endif
            OnUpdate();
        }

#if DEBUG
        private void StopStopWatch()
        {
            _stopwatch.Stop();
        }
#endif
        

        public void InternalComplete()
        {
#if DEBUG
            if (_isLongTask) CoreLog.LogWarning($"Long task completed: ID: {_id} Type: {GetType().FullName}");
            StopStopWatch();
#endif
            OnComplete();
        }
        
        protected virtual void OnComplete()
        {
        }

        public void InternalStart()
        {
#if DEBUG
            CoreLog.Log($"{this.GetType().FullName } OnStart");
            _stopwatch.Start();
#endif
            Status = TaskStatus.Running;
            OnStart();
        }
        
        protected virtual void OnStart()
        {
            
        }

        protected virtual void OnUpdate()
        {

        }

        public void InternalAbort()
        {
#if DEBUG
            StopStopWatch();
#endif
            OnAbort();
        }
        
        protected virtual void OnAbort()
        {
        }
    }
}