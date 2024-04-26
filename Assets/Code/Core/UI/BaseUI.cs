using Code.Core.UI.SciptMono;
using Code.Core.Utils;
using UnityEngine;

namespace Code.Core.UI
{
    public class BaseUI : ScriptMono
    {
        protected RectTransform _rectTransform;
        
        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = gameObject.GetComponent<RectTransform>();
                return _rectTransform;
            }
        }
        
        public void StopAllAnimation() {
            var anim = GetComponent<Animation>();
            if (anim != null)
            {
                anim.Stop();
            }
        }
        
        public void StopAnimation(string name) {
            var anim = GetComponent<Animation>();
            if (anim != null)
            {
                anim.Stop(name);
            }
        }
        
        public float PlayAnimation(string name, System.Action callback, bool customTimescale = false)
        {
            var anim = GetComponent<Animation>();
            if (anim == null)
            {
                callback?.Invoke();
                return 0F;
            }

            if (string.IsNullOrEmpty(name))
            {
                callback?.Invoke();
                return 0F;
            }

            var clip = anim.GetClip(name);
            if (clip == null)
            {
                CoreLog.LogError($"AnimationClip {name} not found!");
                callback?.Invoke();
                return 0F;
            }
        
            var thisTimeScale = 1f;
            if (customTimescale)
            {
                thisTimeScale = 1.0f;
            }
        
            var state = anim[name];
            if (state != null)
            {
                state.speed = 1 / thisTimeScale;
            }
        
            anim.Play(name, PlayMode.StopAll);
            var duration = clip.length * thisTimeScale;
            if (callback != null)
                this.Delay(duration, callback);
            return duration;
        }
    }
}