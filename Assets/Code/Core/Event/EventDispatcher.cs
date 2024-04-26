using System;
using System.Collections.Generic;
using Code.Core.Utils;
using UnityEngine;

namespace Code.Core.Event
{
    public class EventDispatcher
    {
        private Dictionary<Type, LinkedList<object>> _handlers =
            new Dictionary<Type, LinkedList<object>>();

        public void Subscribe<T>(Action<T> handler)
        {
            Subscribe<T>(typeof(T), handler);
        }
        public void Subscribe<T>(Type type, Action<T> handler)
        {
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new LinkedList<object>();
                _handlers.Add(type, list);
            }

            list.AddLast(handler);
        }
        public void Unsubscribe<T>(Action<T> handler)
        {
            Unsubscribe<T>(typeof(T), handler);
        }
        public void Unsubscribe<T>(Type type, Action<T> handler)
        {
            if (_handlers.TryGetValue(type, out var list))
            {
                while (list.Remove(handler)) ;
            }
        }

        public void Clear()
        {
            _handlers.Clear();
        }
        
        public void Trigger<T>(T obj)
        {
            Trigger<T>(typeof(T), obj);
        }

        public void Trigger<T>(Type type, T obj)
        {
            if (_handlers.TryGetValue(type, out var list))
            {
                if (list.Count == 0) return;
                var cur = list.First;
                while (cur != null)
                {
                    var next = cur.Next;
                    try
                    {
                        if (cur.Value != null)
                        {
                            ((Action<T>)cur.Value).Invoke(obj);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(MiscUtil.GetFullExceptionInfo(e));
                    }
                    finally
                    {
                        cur = next;
                    }
                }
            }
        }
    }
}