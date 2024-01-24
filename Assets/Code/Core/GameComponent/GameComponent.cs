using System;
using System.Collections.Generic;
using Code.Core.Utils;

namespace Code.Core.GameComponent
{
    public abstract class GameComponent
    {
        protected GameComponent Parent;
        protected RootComponent Root;

        public GameComponent GetParent() => Parent;
        public RootComponent GetRoot() => Root;

        protected virtual RootComponent GetRootForChild()
        {
            return Root;
        }

        protected Dictionary<Type, List<GameComponent>> SubComponents = new();

        public void AddComponent(Type type, GameComponent comp)
        {
            if (!SubComponents.TryGetValue(type, out var list))
            {
                list = new List<GameComponent>();
                SubComponents.Add(type, list);
            }
            
            list.Add(comp);
            comp.Parent = this;
            comp.Root = GetRootForChild();
            comp.Awake();
        }


        public T AddComponent<T>(Action<T> awake = null) where T : GameComponent, new()
        {
            var component = new T();
            awake?.Invoke(component);
            var type = component.GetType();
            if (!SubComponents.TryGetValue(type, out var list))
            {
                list = new List<GameComponent>();
                SubComponents.Add(type, list);
            }
            
            list.Add(component);
            component.Parent = this;
            component.Root   = GetRootForChild();
            component.Awake();
            return component;
        }

        public void RemoveComponent(GameComponent component) 
        {
            if (SubComponents.TryGetValue(component.GetType(), out var list))
            {
                CoreLog.Log($"[GameComponent] RemoveComponent {component.GetType().Name}");
                list.Remove(component);
                component.Parent = null;
                component.OnDestroy();
            }
        }

        public void RemoveAllComponents(Type type)
        {
            if (SubComponents.TryGetValue(type, out var list))
            {
                CoreLog.Log($"[GameComponent] RemoveAllComponents {type.Name}");
                SubComponents.Remove(type);
                for (int i = 0; i < list.Count; i++)
                {
                    var comp = list[i];
                    comp.Parent = null;
                    comp.OnDestroy();
                }
                list.Clear();
            }
        }

        public void RemoveAllComponents<T>() where T : GameComponent
        {
            var type = typeof(T);
            RemoveAllComponents(type);
        }
        
        public GameComponent GetComponent(Type type)
        {
            if (SubComponents.TryGetValue(type, out var list))
            {
                if (list.Count > 0) return list[0];
            }

            return default;
        }

        public T GetComponent<T>() where T : GameComponent
        {
            if (SubComponents.TryGetValue(typeof(T), out var list))
            {
                if (list.Count > 0) return (T)list[0];
            }

            return default;
        }

        public T[] GetComponents<T>() where T : GameComponent
        {
            if (SubComponents.TryGetValue(typeof(T), out var list))
            {
                if (list.Count > 0) return (T[])list.ToArray();
            }

            return Array.Empty<T>();
        }

        public static void Destroy(GameComponent component)
        {
            var parent = component.Parent;
            parent.RemoveComponent(component);
        }

        protected virtual void OnDestroy()
        {
            CoreLog.Log($"[{nameof(GameComponent)}] {GetType().Name} OnDestroy");
            
            foreach (var list in SubComponents.Values)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var comp = list[i];
                    comp.Parent = null;
                    comp.Root   = null;
                    comp.OnDestroy();
                }
            }
            SubComponents.Clear();
        }

        protected abstract void Awake();
    }
}