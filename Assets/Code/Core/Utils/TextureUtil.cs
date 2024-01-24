using UnityEngine;

namespace Core.Utils
{
    public static class TextureUtil
    {
        public static Texture2D CutTextureFromAtlas(this Sprite s, TextureFormat textureFormat = TextureFormat.RGBA32)
        {
            var textureRect       = s.textureRect;
            var textureRectOffset = s.textureRectOffset;
            var rect              = s.rect;
            var spriteWidth       = (int)rect.width;
            var spriteHeight      = (int)rect.height;
            var offsetX           = (int)textureRectOffset.x;
            var offsetY           = (int)textureRectOffset.y;
            var texX              = (int)textureRect.x;
            var texY              = (int)textureRect.y;
            var texWidth          = (int)textureRect.width;
            var texHeight         = (int)textureRect.height;

            var ret = new Texture2D(spriteWidth, spriteHeight, textureFormat, false, false);
            ret.SetPixels(0, 0, spriteWidth, spriteHeight, new Color[spriteWidth * spriteHeight]);
            ret.SetPixels(offsetX, offsetY, texWidth, texHeight, s.texture.GetPixels(texX, texY, texWidth, texHeight));
            ret.Apply();
            return ret;
        }

        public static Sprite CutFromAtlas(this Sprite s, TextureFormat textureFormat = TextureFormat.RGBA32)
        {
            var tex = CutTextureFromAtlas(s, textureFormat);
            var ret = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            return ret;
        }
    }
}