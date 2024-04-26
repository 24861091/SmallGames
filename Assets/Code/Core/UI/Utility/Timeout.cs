using System;
using System.Collections;
using System.Collections.Generic;
using Code.Core.Coroutine;
using Code.Core.GameComponent;
using Code.Core.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Core.UI.Utility
{
    public class Timeout : GameComponent.GameComponent
    {
        private uint _id;
        private uint NextId => ++_id;

        private static Timeout                       _instance;
        private        Dictionary<uint, IEnumerator> _timeouts = new();

        public static uint SetDelay(float seconds, Action onTimeout, MonoBehaviour host = null)
        {
            return _instance._SetDelay(seconds, onTimeout, false, host);
        }

        public static uint SetDelayRealTime(float seconds, Action onTimeout, MonoBehaviour host = null)
        {
            return _instance._SetDelay(seconds, onTimeout, true, host);
        }
        
        private uint _SetDelay(float seconds, Action onTimeout, bool isRealTime = true, MonoBehaviour host = null)
        {
            var id        = NextId;
            var coroutine = SetTimeOut(id, seconds, onTimeout, isRealTime);
            _timeouts.Add(id, coroutine);
            GameComponentRoot.Instance.GetComponent<CoroutineComponent>().StartCoroutine(coroutine);
            return id;
        }

        private IEnumerator SetTimeOut(uint id, float seconds, Action onTimeout, bool isRealTime)
        {
            yield return isRealTime ? new WaitForSecondsRealtime(seconds) : new WaitForSeconds(seconds);
            onTimeout?.Invoke();
            if (_timeouts.ContainsKey(id)) _timeouts.Remove(id);
        }

        public static uint SetTs(long ts, Action onTimeout, MonoBehaviour host = null)
        {
            return _instance._SetTs(ts, onTimeout, host);
        }

        private uint _SetTs(long ts, Action onTimeout, MonoBehaviour host = null)
        {
            var id        = NextId;
            var coroutine = SetTimestamp(id, ts, onTimeout);
            _timeouts.Add(id, coroutine);
            GameComponentRoot.Instance.GetComponent<CoroutineComponent>().StartCoroutine(coroutine);
            return id;
        }

        private IEnumerator SetTimestamp(uint id, long ts, Action onTimeOut)
        {
            while (TimeUtil.GetTimeStamp() != ts)
            {
                yield return null;
            }

            onTimeOut?.Invoke();
            if (_timeouts.ContainsKey(id)) _timeouts.Remove(id);
        }

        public static uint SetRepeat(float interval, int times, Action<int> onTick, MonoBehaviour host = null)
        {
            return _instance._SetRepeat(interval, times, onTick, host);
        }

        private uint _SetRepeat(float interval, int times, Action<int> onTick, MonoBehaviour host = null)
        {
            var id        = NextId;
            var coroutine = SetRepeatLoop(id, interval, times, onTick);
            _timeouts.Add(id, coroutine);
            GameComponentRoot.Instance.GetComponent<CoroutineComponent>().StartCoroutine(coroutine);
            return id;
        }

        private IEnumerator SetRepeatLoop(uint id, float interval, int times, Action<int> onTick)
        {
            var remain = times > 0 ? times : 1;
            var wait   = new WaitForSeconds(interval);
            var index  = 0;
            while (remain > 0)
            {
                yield return wait;
                if (times > 0) remain--;
                onTick?.Invoke(index++);
            }

            if (_timeouts.ContainsKey(id)) _timeouts.Remove(id);
        }

        public static void Cancel(uint id, MonoBehaviour host = null)
        {
            _instance._Cancel(id, host);
        }

        private void _Cancel(uint id, MonoBehaviour host = null)
        {
            if (_timeouts.TryGetValue(id, out var coroutine))
            {
                GameComponentRoot.Instance.GetComponent<CoroutineComponent>().StartCoroutine(coroutine);
                _timeouts.Remove(id);
            }
        }

        protected override void Awake()
        {
            _instance = this;
        }

        public static UnityAction WrapClick(Action action, float delay = 0.5f)
        {
            var clickable = true;
            return () =>
            {
                if (!clickable) return;
                clickable = false;
                SetDelay(delay, () => clickable = true);
                action();
            };
        }
    }
}