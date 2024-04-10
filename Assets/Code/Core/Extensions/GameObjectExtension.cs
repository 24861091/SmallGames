using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Core.Extensions
{
    public static class GameObjectExtension
    {
        public static string GetPath(this GameObject go)
        {
            var path = "/" + go.name;
            var parent = go.transform.parent;
            while (parent)
            {
                path = "/" + parent.name + path;
                parent = parent.parent;
            }

            return path;
        }
        
        internal static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (!comp)
                comp = go.AddComponent<T>();
            return comp;
        }

        internal static Component GetOrAddComponent(this GameObject go, Type type)
        {
            var comp = go.GetComponent(type);
            if (!comp)
                comp = go.AddComponent(type);
            return comp;
        }

        public static void SetDark(this GameObject go, bool setDark)
        {
            var renderers = go.GetComponentsInChildren<Graphic>();

            foreach (var r in renderers)
                if (setDark)
                    r.color = new Color(0.6132f, 0.6132f, 0.6132f, r.color.a);
                else
                    r.color = new Color(1, 1, 1, r.color.r);
        }

        public static void SetEmissionEnabled(this ParticleSystem particleSystem, bool value)
        {
            var emission = particleSystem.emission;
            emission.enabled = value;
        }

        public static bool IsNull(this Object o) // 或者名字叫IsDestroyed等等
        {
            return o == null;
        }
    }
}