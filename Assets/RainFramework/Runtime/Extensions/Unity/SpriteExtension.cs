﻿using UnityEngine;
namespace Rain.Core
{
    public static class SpriteExtension
    {
        public static Sprite ConvertToSprite(this Texture2D @this)
        {
            Sprite sprite = Sprite.Create(@this, new Rect(0, 0, @this.width, @this.height), Vector2.zero);
            return sprite;
        }
    }
}
