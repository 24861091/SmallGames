using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.Extensions
{
    public static class ComponentExtension
    {
        public static void SetParent(this Component component, Transform parentTransform,
            bool worldPositionStays = false)
        {
            component.transform.SetParent(parentTransform, worldPositionStays);
        }

        public static void SetLocalPosition(this Component component, Vector3 position)
        {
            component.transform.localPosition = position;
        }

        public static void SetPosition(this Component component, Vector3 position)
        {
            component.transform.position = position;
        }

        public static void SetLocalRotation(this Component component, Quaternion rotation)
        {
            component.transform.localRotation = rotation;
        }

        public static void SetRotation(this Component component, Quaternion rotation)
        {
            component.transform.rotation = rotation;
        }

        public static void SetLocalScale(this Component component, Vector3 scale)
        {
            component.transform.localScale = scale;
        }

        public static void SafeStopCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine)
        {
            if (coroutine == null) return;
            monoBehaviour.StopCoroutine(coroutine);
            coroutine = null;
        }
        
        public static void SafeStopCoroutine(this MonoBehaviour monoBehaviour, ref IEnumerator coroutine)
        {
            if (coroutine == null) return;
            monoBehaviour.StopCoroutine(coroutine);
            coroutine = null;
        }

        public static Vector2 CenterChild(this ScrollRect scrollRect, Transform child)
        {
            Canvas.ForceUpdateCanvases();
            Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
            Vector2 childLocalPosition    = scrollRect.content.InverseTransformPoint(child.position);
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );
            return result;
        }
    }
}