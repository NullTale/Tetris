using System;
using UnityEngine;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SoundKeyAttribute : PropertyAttribute
    {
    }
}