using UnityEngine;

namespace Core.UI
{
    public abstract class BaseUIMono : MonoBehaviour
    {
        protected RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }
    }
}