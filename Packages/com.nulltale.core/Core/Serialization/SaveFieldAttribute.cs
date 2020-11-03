using System;
using UnityEngine;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SaveFieldAttribute : PropertyAttribute
    {
    }
}