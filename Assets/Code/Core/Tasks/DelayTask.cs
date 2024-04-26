using UnityEngine;

namespace Code.Core.Tasks
{
    public class DelayTask : AbstractTask
    {
        private float _duration;
        private float _passed;
        
        public DelayTask(float seconds)
        {
            _duration = seconds;
        }

        protected override void OnUpdate()
        {
            _passed += Time.deltaTime;
            if (_passed >= _duration)
            {
                Status = TaskStatus.Done;
            }
        }
    }
}