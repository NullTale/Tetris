using System;
using UnityEngine;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SpriteDrawlerAttribute : PropertyAttribute
    {
        public const int c_DefaultHeight = 64;
        public int Height { get; set; }

        public SpriteDrawlerAttribute(int height = c_DefaultHeight)
        {
            Height = height;
        }

    }
}