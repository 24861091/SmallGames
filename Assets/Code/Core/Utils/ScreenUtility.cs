using System;
using UnityEngine;

namespace Code.Core.Utils
{
    public class ScreenUtility
    {
        public static Rect CameraRenderRect = new Rect(0, 0, 1, 1);
        
        public static void SetRectTransformSafeAreaConform(RectTransform rectTransform, bool conformYTop,
            bool conformYBottom)
        {
            var width = (float)Screen.width;
            var height = (float)Screen.height;
            var renderRect = new Rect(CameraRenderRect.x * width, CameraRenderRect.y * height, CameraRenderRect.width * width,
                CameraRenderRect.height * height);
            var safeArea = Screen.safeArea;
            var leftX = Math.Max(safeArea.x, renderRect.x);
            var rightX = Math.Min(safeArea.x + safeArea.width, renderRect.x + renderRect.width);
            var topY = 0f; 
            if (conformYTop)
            {
                topY = Math.Min(safeArea.y + safeArea.height, renderRect.y + renderRect.height);
            }
            else
            {
                topY = renderRect.y + renderRect.height;
            }
            
            var bottomY = 0f; 
            if (conformYBottom)
            {
                bottomY = Math.Max(safeArea.y, renderRect.y);
            }
            else
            {
                bottomY = renderRect.y;
            }

            var newSafeArea = new Rect(leftX, bottomY, rightX - leftX, topY - bottomY);
            
            newSafeArea.x -= renderRect.x;
            newSafeArea.y -= renderRect.y;

            renderRect.x = 0;
            renderRect.y = 0;
            
            var anchorMin = new Vector2(newSafeArea.x / renderRect.width, newSafeArea.y / renderRect.height);
            var anchorMax = new Vector2((newSafeArea.x + newSafeArea.width) / renderRect.width,
                (newSafeArea.y + newSafeArea.height) / renderRect.height);

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.sizeDelta = new Vector2();
        }

        private static bool _cameraRenderRectInited;
        public static void InitCameraRenderRect(bool force = false)
        {
            if (_cameraRenderRectInited && !force) return;
            
            _cameraRenderRectInited = true;
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float targetAspect = Mathf.Clamp(screenAspect, 9f / 21f, 3f / 4f);
            float scaleHeight = screenAspect / targetAspect;

            if (scaleHeight < 1.0f)
            {
                CameraRenderRect = new Rect
                {
                    width = 1.0f,
                    height = scaleHeight,
                    x = 0,
                    y = (1.0f - scaleHeight) / 2.0f
                };
            }
            else // add pillarbox
            {
                float scalewidth = 1.0f / scaleHeight;

                CameraRenderRect = new Rect
                {
                    width = scalewidth,
                    height = 1.0f,
                    x = (1.0f - scalewidth) / 2.0f,
                    y = 0
                };
            }
        }

        public static void SetCameraAspectRatio(Camera camera)
        {
            camera.rect = CameraRenderRect;
        }
    }
}