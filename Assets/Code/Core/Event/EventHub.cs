using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Code.Core.Utils;

namespace Code.Core.Event
{
    public static class EventHub
    {
        private static EventDispatcher _eventDispatcher = new EventDispatcher();
        private static Action          _updateFunc;
        private static Action          _lateUpdateFunc;
        private static Action<bool>    _appPauseFunc;
        
        public static void Trigger<T>(T arg)
        {
            _eventDispatcher.Trigger(arg);
        }

        public static void Subscribe<T>(Action<T> handler)
        {
            _eventDispatcher.Subscribe(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            _eventDispatcher.Unsubscribe(handler);
        }

        private static LinkedList<Action> _secondTickHandler = new LinkedList<Action>();

        public static void SubscribeSecondTick(Action handler)
        {
            _secondTickHandler.AddLast(handler);
        }

        public static void UnsubscribeSecondTick(Action handler)
        {
            _secondTickHandler.Remove(handler);
        }

        private static void OnSecondTick()
        {
            ExecuteHandlers(_secondTickHandler);
        }

        private static void ExecuteHandlers(LinkedList<Action> handlers, [CallerMemberName] string caller = "")
        {
            if (handlers.Count == 0) return;
            var node = handlers.First;
            while (node != null)
            {
                try
                {
                    var next = node.Next;
                    node.Value.Invoke();
                    node = next;
                }
                catch (Exception e)
                {
                    CoreLog.LogError($"EventHub {caller} caught exception:\n" +
                                     $"{MiscUtil.GetFullExceptionInfo(e)}");
                }
            }
        }

        //private static float _lastUnscaledTickTime;
        private static long _nextSecond;

        //每秒Tick时机应保证在真实时间的整数秒之后，确保不会出现因为时间误差导致的显示错误
        public static void Update()
        {
            _updateFunc?.Invoke();
            var nowTime = NowTime.Milliseconds;
            if (nowTime >= _nextSecond)
            {
                _nextSecond = (nowTime / 1000 + 1) * 1000;//计算下一秒的整数秒
                OnSecondTick();
            }
            /*var unscaledTime = Time.unscaledTime;
            if (unscaledTime - _lastUnscaledTickTime >= 1.0f)
            {
                _lastUnscaledTickTime = unscaledTime - (unscaledTime - _lastUnscaledTickTime) % 1.0f;
                OnSecondTick();
            }*/
        }

        public static void LateUpdate()
        {
            _lateUpdateFunc?.Invoke();
        }
        
        public static void SubscribeUpdate(Action handler)
        {
            _updateFunc += handler;
        }

        public static void UnsubscribeUpdate(Action handler)
        {
            _updateFunc -= handler;
        }
        
        public static void SubscribeLateUpdate(Action handler)
        {
            _lateUpdateFunc += handler;
        }

        public static void UnsubscribeLateUpdate(Action handler)
        {
            _lateUpdateFunc -= handler;
        }

        public static void OnApplicationPause(bool isPause)
        {
            _appPauseFunc?.Invoke(isPause);
        }
        
        public static void SubscribeOnApplicationPause(Action<bool> handler)
        {
            _appPauseFunc += handler;
        }

        public static void UnsubscribeOnApplicationPause(Action<bool> handler)
        {
            _appPauseFunc -= handler;
        }
    }
}