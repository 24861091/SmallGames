using UnityEngine;

namespace Code.Core.Utils
{
    public static class BlurUtil
    {
        public static Sprite BlurSprite(Sprite sprite, float blurFactor = 3, int downScale = 2)
        {
            var offset = sprite.textureRect.position - sprite.textureRectOffset;
            var size = sprite.rect.size;
            var rect = new Rect(offset, size);
            var tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
            var destWidth = (int)(size.x / downScale);
            var destHeight = (int)(size.y / downScale);
            var pixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            tex.SetPixels(pixels);
            tex.Apply();
            var rt = RenderTexture.GetTemporary(destWidth, destHeight, 0, RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.Default);
            var shader = Shader.Find("UI/SpriteBlur");
            var mat = new Material(shader);
            mat.SetFloat("_BlurFactor", blurFactor);
            var ort = RenderTexture.active;
            Graphics.Blit(tex, rt, mat);
            tex.Reinitialize(destWidth, destHeight);
            tex.ReadPixels(new Rect(0, 0, destWidth, destHeight), 0, 0);
            tex.Apply();
            RenderTexture.active = ort;
            Object.Destroy(mat);
            RenderTexture.ReleaseTemporary(rt);
            var blurredSprite = Sprite.Create(tex, new Rect(0, 0, destWidth, destHeight), Vector2.one * 0.5f);
            blurredSprite.name = sprite.name + "(Blurred)";
            return blurredSprite;
        }
    }
}