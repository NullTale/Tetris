using UnityEngine;
using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowStateAttribute : PropertyAttribute
    {
    }
}