using System.Collections;
using UnityEngine;

namespace Code.Core.UI
{
    public static class MonoBehaviourExt
    {
        private static IEnumerator DoDelay(float time, System.Action callback) {
            yield return new WaitForSeconds(time);
            if(callback != null)
                callback();
        }

        private static IEnumerator DoDelay0(System.Action callback) {
            yield return null;
            if(callback != null)
                callback();
        }

        public static UnityEngine.Coroutine Delay(this MonoBehaviour owner, float time, System.Action callback)
        {
            if (callback == null)
                return null;

            if (owner.gameObject.activeSelf)
            {
                if (time == 0)
                {
                    return owner.StartCoroutine(DoDelay0(callback));
                }
                else
                {
                    return owner.StartCoroutine(DoDelay(time, callback));
                }
            }

            return null;
        }

        public static bool PlayObjectAnimation(this MonoBehaviour owner, GameObject go, string name, System.Action callback = null) {
            if (go == null)
                return false;

            if (owner == null)
                return false;

            var anim = go.GetComponent<Animation> ();
            if (anim == null) {
                if (callback != null) {
                    callback ();
                }
                return false;
            }
            var clip = anim.GetClip (name);
            if (clip == null) {
                if (callback != null) {
                    callback ();
                }
                return false;
            }
            anim.Play (name, PlayMode.StopAll);
            if (owner.gameObject.activeInHierarchy) {
                owner.StartCoroutine (DoDelay (clip.length, callback));
            }
            return true;
        }

        public static bool StopObjectAnimation(this MonoBehaviour owner, GameObject go)
        {
            if (go == null)
                return false;

            if (owner == null)
                return false;

            var anim = go.GetComponent<Animation>();
            if (anim == null)
            {
                return false;
            }

            anim.Stop();
            return true;
        }
    }
}